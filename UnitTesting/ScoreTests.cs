using GameApi.Data;
using GameApi.Models;
using GameApi.Repositories;
using GameApi.Services.Score;
using Microsoft.EntityFrameworkCore;
using Moq;
using ThrowawayDb;
using Xunit;
using Xunit.Abstractions;

namespace UnitTesting;

public class ThrowawayScoreRepository : IScoreRepository, IDisposable
{
    private readonly ThrowawayDatabase database;

    public ThrowawayScoreRepository()
    {
        
    }

    public Task AddAsync(Highscore highscore)
    {
        throw new NotImplementedException();
    }
    public Task<Highscore> GetByIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
    public void Dispose()
    {
        database?.Dispose();
    }
}
public class ScoreServiceTests(ITestOutputHelper output)
{
    private readonly Mock<IScoreRepository> scoreRepositoryMock = new();
    private readonly Mock<IScoreService>    scoreServiceMock    = new();

    private readonly ITestOutputHelper output = output;

    [SetUp]
    public void Setup()
    {
        
        scoreRepositoryMock.Setup(repo
            => repo.AddAsync(It.IsAny<Highscore>())).Returns(Task.CompletedTask);
    }
    [TearDown]
    public void TearDown()
    {
        
    }
    [Fact]
    private void Score_Update()
    {
        var repository  = new Mock<IScoreRepository>();
        var highscoreId = Guid.NewGuid();

        repository
            .Setup(repo
                => repo.AddAsync(new Highscore { Id = highscoreId, Score = 0 }));
        repository
            .Setup(repo
                => repo.GetByIdAsync(highscoreId))
            .ReturnsAsync(new Highscore { Id = highscoreId, Score = 0 });
    }
}