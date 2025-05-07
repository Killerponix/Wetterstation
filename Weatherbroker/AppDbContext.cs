using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Weatherbroker.Models.weather_data;

public class AppDbContext : DbContext
{
    public DbSet<weather_raw> weather_raw { get; set; }
    public DbSet<weather_day> weather_day { get; set; }
    public DbSet<weather_stats> weather_stats { get; set; }

    private readonly IConfiguration _configuration;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Hole die Verbindungszeichenfolge aus der appsettings.json
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var serverVersion = new MariaDbServerVersion(new Version(10,11));
        optionsBuilder.UseMySql(connectionString, serverVersion);
    }
}

// namespace Weatherbroker.Models
// {
//     public class AppDbContext : DbContext
//     {
//         
//     }    
// }
