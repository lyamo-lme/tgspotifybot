using Microsoft.EntityFrameworkCore;

namespace SpotifyTelegramBot.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}