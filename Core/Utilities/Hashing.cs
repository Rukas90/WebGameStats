using System.Security.Cryptography;

namespace Core.Utilities;

public struct HashRules
{
    public int               HashSize   { get; init; }
    public int               SaltSize   { get; init; }
    public int               Iterations { get; init; }
    public HashAlgorithmName Algorithm  { get; init; }
}
public class Hashing
{
    public static HashRules DefaultSHA256Rules => new()
    {
        HashSize   = 32,
        SaltSize   = 16,
        Iterations = 2000000,
        Algorithm  = HashAlgorithmName.SHA256
    };
    public static string Hash(string value, HashRules rules)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(value, [], rules.Iterations, rules.Algorithm);
        byte[]    hash   = pbkdf2.GetBytes(rules.HashSize);

        return Convert.ToBase64String(hash);
    }
    public static string Hash(string value, out byte[] salt, HashRules rules)
    {
        salt = new byte[rules.SaltSize];
        
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        
        using var pbkdf2 = new Rfc2898DeriveBytes(value, salt, rules.Iterations, rules.Algorithm);
        byte[] hash = pbkdf2.GetBytes(rules.HashSize);

        return Convert.ToBase64String(hash);
    }
    public static bool Compare(string value, string hash, HashRules rules)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(value, [], rules.Iterations, rules.Algorithm);

        byte[] hashToCheck = pbkdf2.GetBytes(rules.HashSize);
        byte[] hashStored  = Convert.FromBase64String(hash);

        return CryptographicOperations.FixedTimeEquals(hashToCheck, hashStored);
    }
    public static bool Compare(string value, string hash, byte[] salt, HashRules rules)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(value, salt, rules.Iterations, rules.Algorithm);
        
        byte[] hashToCheck = pbkdf2.GetBytes(rules.HashSize);
        byte[] hashStored  = Convert.FromBase64String(hash);

        return CryptographicOperations.FixedTimeEquals(hashToCheck, hashStored);
    }
}