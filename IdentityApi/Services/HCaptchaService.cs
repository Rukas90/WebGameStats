using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Services;

namespace IdentityApi.Services;

internal sealed class HCaptchaSettings
{
    public string  SecretKey { get; init; } = null!;
    public string? SiteKey   { get; init; }
}
internal class HCaptchaVerificationResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("challenge_ts")]
    public DateTime? ChallengeTimestamp { get; init; }

    [JsonPropertyName("hostname")]
    public string? Hostname { get; init; }

    [JsonPropertyName("error-codes")]
    public string[]? ErrorCodes { get; init; }
}

internal interface IHCaptchaService
{
    Task<bool> VerifyAsync(string token, string? userIP = null);
}
[AppService<IHCaptchaService>]
internal class HCaptchaService(
    HttpClient httpClient, IConfiguration configuration, ILogger<HCaptchaService> logger
): IHCaptchaService
{
    private readonly string secretKey = configuration["HCaptcha:SecretKey"]!;
    
    public async Task<bool> VerifyAsync(string token, string? userIP = null)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }
        try
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new("secret", secretKey),
                new("response", token)
            };
            AppendRemoteIp(userIP, in parameters);
            
            var content  = new FormUrlEncodedContent(parameters);
            var response = await httpClient.PostAsync(configuration["HCaptcha:VerificationUrl"], content);

            Console.WriteLine(JsonSerializer.Serialize(response));
            
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            Console.WriteLine("----------------------------------------------------");
            
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<HCaptchaVerificationResponse>(jsonResponse);

            Console.WriteLine(JsonSerializer.Serialize(result));
            
            return result?.Success == true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "HCaptcha verification failed");
            return false;
        }
    }

    private static void AppendRemoteIp(string? userIP, in List<KeyValuePair<string, string>> parameters)
    {
        const string GOOGLE_PUBLIC_DNS = "8.8.8.8";
        
        if (!string.IsNullOrEmpty(userIP) && !IPAddress.IsLoopback(IPAddress.Parse(userIP)))
        {
            parameters.Add(new("remoteip", userIP));
            return;
        }
        parameters.Add(new("remoteip", GOOGLE_PUBLIC_DNS));
    }
}