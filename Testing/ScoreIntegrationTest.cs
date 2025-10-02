using GameApi.Data;
using GameApi.Repositories;
using GameApi.Services.Score;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Testcontainers.PostgreSql;
using Xunit;

namespace Testing.Integration.Score;

public class ScoreIntegrationTest : IAsyncLifetime
{
    private PostgreSqlContainer? container;
    private GameDbContext?       context;
    private ScoreRepository?     repository;
    private ScoreService?        service;
    
    public async Task InitializeAsync()
    {
        container = new PostgreSqlBuilder()
            .WithDatabase("test-db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
        
        await container.StartAsync();
        
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseNpgsql(container.GetConnectionString())
            .Options;

        context = new GameDbContext(options);
        await context.Database.EnsureCreatedAsync();
        
        repository = new ScoreRepository(context);
        service    = new ScoreService(repository);
    }
    public async Task DisposeAsync()
    {
        await context!.DisposeAsync();
        await container!.DisposeAsync();
    }
    [Fact]
    public async Task UpdateScore_ShouldPersistHighscore_WhenNewUser()
    {
        var userId = Guid.NewGuid();
        var result = await service!.UpdateScoreAsync(userId, amount: 1);
        
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(1);
        
        var highscore = await repository!.GetByIdAsync(userId);
        
        highscore.ShouldNotBeNull();
        highscore.Id.ShouldBe(userId);
        highscore.Score.ShouldBe(1);
    }
    [Fact]
    public async Task UpdateScore_ShouldFail_WhenUserIdIsEmpty()
    {
        var result = await service!.UpdateScoreAsync(Guid.Empty, amount: 1);

        result.IsFailure.ShouldBeTrue();
        result.Problem.Status.ShouldBe(StatusCodes.Status404NotFound);
    }
    [Fact]
    public async Task UpdateScore_ShouldFail_WhenAmountIsNegative()
    {
        var result = await service!.UpdateScoreAsync(Guid.NewGuid(), amount: -1);

        result.IsFailure.ShouldBeTrue();
        result.Problem.Status.ShouldBe(StatusCodes.Status400BadRequest);
    }
    [Fact]
    public async Task GetScore_ShouldFail_WhenUserDoesNotExist()
    {
        var result = await service!.GetScoreAsync(Guid.NewGuid());

        result.IsFailure.ShouldBeTrue();
        result.Problem.Status.ShouldBe(StatusCodes.Status404NotFound);
    }
}