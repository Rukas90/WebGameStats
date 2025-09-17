using System.Data.Entity;
using Core.Services;
using GameApi.Data;
using GameApi.Models;

namespace GameApi.Repositories;

public interface IScoreRepository
{
    public Task AddAsync(Highscore highscore);
    public Task<Highscore> GetByIdAsync(Guid userId);
    public Task SaveChangesAsync();
}
[AppService<IScoreRepository>]
public class ScoreRepository(GameDbContext dbContext) 
    : IScoreRepository
{
    public async Task AddAsync(Highscore highscore) 
        => await dbContext.AddAsync(highscore);
    
    public async Task<Highscore> GetByIdAsync(Guid userId)
        => await dbContext.Highscores
            .SingleOrDefaultAsync(highscore => highscore.Id == userId);
    
    public async Task SaveChangesAsync() 
        => await dbContext.SaveChangesAsync();
}