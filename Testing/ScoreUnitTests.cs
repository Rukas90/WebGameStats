using GameApi.Models;
using GameApi.Repositories;
using GameApi.Services.Score;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using Shouldly;

namespace Testing.Unit.Score;

public class ScoreServiceTests
{
    private readonly Mock<IScoreRepository> repositoryMock;
    private readonly ScoreService           service;
    
    public ScoreServiceTests()
    {
        repositoryMock = new Mock<IScoreRepository>();
        service        = new ScoreService(repositoryMock.Object);
    }

    [Fact]
    public async Task UpdateScoreAsync_ShouldReturnBadRequest_WhenAmountIsNegative()
    {
        var userId = Guid.NewGuid();
        var result = await service.UpdateScoreAsync(userId, amount: -1);
        
        result.IsFailure.ShouldBeTrue();
        result.Problem.ShouldNotBeNull();
        result.Problem.Status.ShouldBe(StatusCodes.Status400BadRequest);
    }
    
    [Fact]
    public async Task UpdateScoreAsync_ShouldHandleOverflow_WhenAddingToMaxValue()
    {
        var userId   = Guid.NewGuid();
        var existing = new Highscore { Id = userId, Score = int.MaxValue };

        repositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(existing);
        
        var result = await service.UpdateScoreAsync(userId, 1);
        
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(int.MaxValue);
    }

    [Fact]
    public async Task UpdateScoreAsync_ShouldFail_WhenUserIdIsEmpty()
    {
        var userId = Guid.Empty;
        var result = await service.UpdateScoreAsync(userId, amount: 1);
        
        result.IsFailure.ShouldBeTrue();
        result.Problem.ShouldNotBeNull();
        result.Problem.Status.ShouldBe(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task UpdateScoreAsync_ShouldCreateWithZero_WhenNewUserAndAmountIsZero()
    {
        var userId = Guid.NewGuid();
        
        repositoryMock.Setup(repo => repo.GetByIdAsync(userId))!.ReturnsAsync((Highscore?)null);
        
        var result = await service.UpdateScoreAsync(userId, amount: 0);
        
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(0);
        
        repositoryMock.Verify(repo => repo.AddAsync(
            It.Is<Highscore>(highscore => highscore.Id == userId && highscore.Score == 0)), Times.Once);
    }
}