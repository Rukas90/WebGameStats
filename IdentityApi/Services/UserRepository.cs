using Core.Data;
using Core.Services;
using IdentityApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Services;

public interface IUserRepository
{
    public Task AddRefreshTokenAsync(RefreshToken token);
    public Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash);
    public Task DeleteUserRefreshTokensAsync(User user);
    public Task<User?> GetByIdAsync(Guid id);
    public Task SaveChangesAsync();
}
[AppService<IUserRepository>]
public class UserRepository(UserDbContext dbContext) : IUserRepository
{
    public async Task AddRefreshTokenAsync(RefreshToken token)
        => await dbContext.RefreshTokens.AddAsync(token);
    public async Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash)
        => await dbContext.RefreshTokens
            .Include(refreshToken => refreshToken.User)
            .FirstOrDefaultAsync(refreshToken => refreshToken.TokenHash == tokenHash);
    public async Task DeleteUserRefreshTokensAsync(User user)
        => await dbContext.RefreshTokens
            .Where(token => token.UserId == user.Id)
            .ExecuteDeleteAsync();
    public async Task<User?> GetByIdAsync(Guid id)
        => await dbContext.Users
            .SingleOrDefaultAsync(user => user.Id == id);
    public async Task SaveChangesAsync() 
        => await dbContext.SaveChangesAsync();
}