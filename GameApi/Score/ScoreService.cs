using Core.Results;
using Core.Services;
using GameApi.Models;
using GameApi.Repositories;

namespace GameApi.Services.Score;

public interface IScoreService
{
    public Task<Result<int>> UpdateScoreAsync(Guid userId, int amount);
    public Task<Result<int>> GetScoreAsync(Guid userId);
}
[AppService<IScoreService>]
public class ScoreService(IScoreRepository repository) : IScoreService
{
    public async Task<Result<int>> UpdateScoreAsync(Guid userId, int amount)
    {
        if (amount < 0)
        {
            return Failure.BadRequest("Score amount cannot be negative.");
        }
        var highscore = await GetHighscoreAsync(userId) ?? await CreateHighscoreAsync(userId, amount);

        var newScore = highscore.Score + amount;
        highscore.Score = newScore;
        
        await repository.SaveChangesAsync();
        return newScore;
    }
    public async Task<Result<int>> GetScoreAsync(Guid userId)
    {
        var highscore = await GetHighscoreAsync(userId);

        if (highscore == null)
        {
            return Failure.NotFound("User highscore not found.");
        }
        return highscore.Score;
    }
    private async Task<Highscore?> GetHighscoreAsync(Guid userId)
    {
        return await repository.GetByIdAsync(userId);
    }
    private async Task<Highscore> CreateHighscoreAsync(Guid userId, int currentScore)
    {
        var highscore = new Highscore { Id = userId, Score = 0 };
        
        await repository.AddAsync(highscore);
        await repository.SaveChangesAsync();
        
        return highscore;
    }
}