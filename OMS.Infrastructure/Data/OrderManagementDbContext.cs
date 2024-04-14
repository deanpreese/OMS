
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using OMS.Application.Models;

namespace OMS.Infrastructure.Data;

public class OrderManagementDbContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options)
        : base(options)
    { }

    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options, IConfiguration configuration) : base(options)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }


    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<ClosedTrade> ClosedTrades { get; set; }
    public DbSet<LiveOrder> LiveOrders { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<ScoreCard> ScoreCards { get; set; }
    public DbSet<OrderLog> OrderLog { get; set; }
    public DbSet<ScoreCardLog> ScoreCardLog { get; set; }
    public DbSet<ModelOrderLog> ModelOrderLog { get; set; }
    public DbSet<ClosedTradeLog> ClosedTradeLog { get; set; }


    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClosedTrade>()
            .HasIndex(b => b.UserID);

        modelBuilder.Entity<ScoreCard>()
            .HasIndex(b => b.UserID);

        modelBuilder.Entity<UserProfile>()
            .HasOne(u => u.ScoreCard)
            .WithOne(s => s.UserProfile)
            .HasForeignKey<ScoreCard>(s => s.UserID);


    }

}


