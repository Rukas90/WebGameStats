using GameApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Data;

public class GameDbContext(DbContextOptions<GameDbContext> options) 
    : DbContext(options)
{
    public DbSet<Highscore> Highscores { get; set; } = null!;
}