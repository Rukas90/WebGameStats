using System.ComponentModel.DataAnnotations;

namespace IdentityApi.Models;

public class RefreshToken
{
    [Key]
    public Guid     Id         { get; init; }
    public Guid     UserId     { get; init; }
    [MaxLength(200)]
    public string   TokenHash  { get; init; } = null!;
    public DateTime IssuedAt   { get; init; }
    public DateTime ExpiresAt  { get; init; }
    public bool     Revoked    { get; set; }
    
    public User User { get; init; } = null!;
}