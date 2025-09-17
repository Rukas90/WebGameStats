using System.Data.Entity;
using Core.Data;
using Core.Services;
using IdentityApi.Models;

namespace IdentityApi.Services;

public interface IUserRepository
{
    public Task<User?> GetById(Guid id);
}
[AppService<IUserRepository>]
public class UserRepository(UserDbContext dbContext) 
    : IUserRepository
{
    public async Task<User?> GetById(Guid id)
    {
        return await dbContext.Users
            .SingleOrDefaultAsync(user => user.Id == id);
    }
    public async Task SaveChangesAsync() 
        => await dbContext.SaveChangesAsync();
}