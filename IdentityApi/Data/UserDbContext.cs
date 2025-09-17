using IdentityApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options)
    : IdentityDbContext<User, UserRole, Guid>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
}