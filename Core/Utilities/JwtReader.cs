using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Results;

namespace Core.Security;

public static class JwtReader
{
    public static Result<Guid> GetUserId(ClaimsPrincipal principal)
    {
        foreach (var c in principal.Claims)
        {
            Console.WriteLine(c);
        }
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        
        if (claim == null)
        {
            return Failure.Unauthorized("Missing user ID claim.");
        }
        if (!Guid.TryParse(claim.Value, out var userId))
        {
            return Failure.BadRequest("Invalid user ID format.");
        }
        return Result<Guid>.Success(userId);
    }
}