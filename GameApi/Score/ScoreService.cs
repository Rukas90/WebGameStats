using Core.Results;
using Core.Services;
using Core.Utilities;
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
        if (userId == Guid.Empty)
        {
            return Failure.NotFound("Invalid user id.");
        }
        if (amount < 0)
        {
            return Failure.BadRequest("Score amount cannot be negative.");
        }
        var highscore = await GetHighscoreAsync(userId) ?? await CreateHighscoreAsync(userId, amount);

        if (MathUtils.WillOverflow(highscore.Score, amount))
        {
            highscore.Score = int.MaxValue;
        }
        else
        {
            highscore.Score += amount;
        }
        await repository.SaveChangesAsync();
        return highscore.Score;
    }
    public async Task<Result<int>> GetScoreAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return Failure.NotFound("Invalid user id.");
        }
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