namespace GameApi.Models;

public record Highscore
{
    public Guid Id    { get; init; } // <-- aka. UserId
    public int  Score { get; set;  }
}