
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using OMS.Core.Models;

namespace OMS.Data
{
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
        public DbSet<LiveOrder> LiveOrder { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<OrderFlow> OrderFlow { get; set; }
        public DbSet<ScoreCard> ScoreCard { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserProfile>().HasData(
                new UserProfile
                {
                    UserID = 999999,
                    DisplayName = "User1",
                    UserPwd = "abc",
                    FirstName = "User",
                    LastName = "One",
                    Email = "admin",
                    DateRegistered = DateTime.Now,
                    Enabled = 1,
                    EnabledLive = 0,
                    GroupID = 0
                }); 

            modelBuilder.Entity<UserProfile>().HasData(
                new UserProfile
                {
                    UserID = 999998,
                    DisplayName = "User2",
                    UserPwd = "abc",
                    FirstName = "User",
                    LastName = "One",
                    Email = "admin",
                    DateRegistered = DateTime.Now,
                    Enabled = 1,
                    EnabledLive = 0,
                    GroupID = 0
                }); 

            
            modelBuilder.Entity<ScoreCard>().HasData(
                new ScoreCard
                {
                    ScoreCardID = 1,
                    UserID = 999999,
                    GroupID =0,
                    Trades =10,
                    Winners =3,
                    Losers =7,
                    Longs =0,
                    Shorts =0,
                    NetProfitLong =0,
                    NetProfitShort =0,
                    GrossProfit =0,
                    GrossLoss =0,
                    LargestWinner =0,
                    LargestLoser =0,
                    LargestWinningStreak =0,
                    LargestLosingStreak =0,
                    TotalNetProfit =0,
                    TradeXML = "",
                    WinLossRatio =0.3,
                    AveWin =0,
                    AveLoss =0,
                    LastUpdate = DateTime.UtcNow
                });

                modelBuilder.Entity<ScoreCard>().HasData(
                new ScoreCard
                {
                    ScoreCardID = 2,
                    UserID = 999998,
                    GroupID =0,
                    Trades =10,
                    Winners =0,
                    Losers =0,
                    Longs =0,
                    Shorts =0,
                    NetProfitLong =0,
                    NetProfitShort =0,
                    GrossProfit =0,
                    GrossLoss =0,
                    LargestWinner =0,
                    LargestLoser =0,
                    LargestWinningStreak =0,
                    LargestLosingStreak =0,
                    TotalNetProfit =0,
                    TradeXML = "",
                    WinLossRatio =0.7,
                    AveWin =0,
                    AveLoss =0,
                    LastUpdate = DateTime.UtcNow
                });

            

        }


    }
}
