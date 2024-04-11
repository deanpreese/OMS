using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    ActivityID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    GroupID = table.Column<int>(type: "integer", nullable: false),
                    LoginTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SessionID = table.Column<int>(type: "integer", nullable: false),
                    SessionGuid = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.ActivityID);
                });

            migrationBuilder.CreateTable(
                name: "ClosedTrades",
                columns: table => new
                {
                    StorerID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    GroupID = table.Column<int>(type: "integer", nullable: false),
                    Instrument = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Leverage = table.Column<double>(type: "double precision", nullable: false),
                    OppositeTrader = table.Column<bool>(type: "boolean", nullable: false),
                    OpenLiveOrderID = table.Column<int>(type: "integer", nullable: false),
                    OpenPlatformOrderID = table.Column<int>(type: "integer", nullable: false),
                    OpenOrderMangerID = table.Column<int>(type: "integer", nullable: false),
                    OpenAuthToken = table.Column<int>(type: "integer", nullable: false),
                    OpenExecutionID = table.Column<int>(type: "integer", nullable: false),
                    OpenRelatedOrderID = table.Column<int>(type: "integer", nullable: false),
                    OpenOrderTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OpenOrderPX = table.Column<double>(type: "double precision", nullable: false),
                    OpenOrderType = table.Column<int>(type: "integer", nullable: false),
                    OpenOrderAction = table.Column<int>(type: "integer", nullable: false),
                    ClosePlatformOrderID = table.Column<int>(type: "integer", nullable: false),
                    ClosedOrderMangerID = table.Column<int>(type: "integer", nullable: false),
                    CloseAuthToken = table.Column<int>(type: "integer", nullable: false),
                    CloseExecutionID = table.Column<int>(type: "integer", nullable: false),
                    CloseRelatedOrderID = table.Column<int>(type: "integer", nullable: false),
                    CloseOrderTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CloseOrderPX = table.Column<double>(type: "double precision", nullable: false),
                    CloseOrderType = table.Column<int>(type: "integer", nullable: false),
                    CloseOrderAction = table.Column<int>(type: "integer", nullable: false),
                    PNL = table.Column<double>(type: "double precision", nullable: false),
                    MAE = table.Column<double>(type: "double precision", nullable: false),
                    MFE = table.Column<double>(type: "double precision", nullable: false),
                    NetChange = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClosedTrades", x => x.StorerID);
                });

            migrationBuilder.CreateTable(
                name: "LiveOrders",
                columns: table => new
                {
                    LiveOrderID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlatformOrderID = table.Column<int>(type: "integer", nullable: false),
                    ExecutedOrderID = table.Column<int>(type: "integer", nullable: false),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    GroupID = table.Column<int>(type: "integer", nullable: false),
                    Leverage = table.Column<double>(type: "double precision", nullable: false),
                    Opposite = table.Column<int>(type: "integer", nullable: false),
                    AuthToken = table.Column<int>(type: "integer", nullable: false),
                    OrderManagerID = table.Column<int>(type: "integer", nullable: false),
                    RelatedOrderID = table.Column<int>(type: "integer", nullable: false),
                    OrderTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Instrument = table.Column<string>(type: "text", nullable: true),
                    OrderPX = table.Column<double>(type: "double precision", nullable: false),
                    OrderType = table.Column<int>(type: "integer", nullable: false),
                    OrderAction = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    MAE = table.Column<double>(type: "double precision", nullable: false),
                    MFE = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveOrders", x => x.LiveOrderID);
                });

            migrationBuilder.CreateTable(
                name: "ModelOrderLog",
                columns: table => new
                {
                    ModelOrderLogID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    GroupID = table.Column<int>(type: "integer", nullable: false),
                    LiveOrderIDReference = table.Column<int>(type: "integer", nullable: false),
                    OrderType = table.Column<int>(type: "integer", nullable: false),
                    OrderAction = table.Column<int>(type: "integer", nullable: false),
                    LiveOrderJSON = table.Column<string>(type: "text", nullable: true),
                    ModelFeatureData = table.Column<string>(type: "text", nullable: true),
                    ScoreCardJSON = table.Column<string>(type: "text", nullable: true),
                    ClosedOrderDTOJSON = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelOrderLog", x => x.ModelOrderLogID);
                });

            migrationBuilder.CreateTable(
                name: "OrderLog",
                columns: table => new
                {
                    OrderFlowId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlatformOrderID = table.Column<int>(type: "integer", nullable: false),
                    ExecutedOrderID = table.Column<int>(type: "integer", nullable: false),
                    AuthToken = table.Column<int>(type: "integer", nullable: false),
                    OrderManagerID = table.Column<int>(type: "integer", nullable: false),
                    Leverage = table.Column<double>(type: "double precision", nullable: false),
                    Opposite = table.Column<int>(type: "integer", nullable: false),
                    RelatedOrderID = table.Column<int>(type: "integer", nullable: false),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    GroupID = table.Column<int>(type: "integer", nullable: false),
                    OrderTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Instrument = table.Column<string>(type: "text", nullable: true),
                    OrderPX = table.Column<double>(type: "double precision", nullable: false),
                    OrderType = table.Column<int>(type: "integer", nullable: false),
                    OrderAction = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLog", x => x.OrderFlowId);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    UserPwd = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    DateRegistered = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Enabled = table.Column<int>(type: "integer", nullable: false),
                    Leverage = table.Column<double>(type: "double precision", nullable: false),
                    IsOpposite = table.Column<int>(type: "integer", nullable: false),
                    EnabledLive = table.Column<int>(type: "integer", nullable: false),
                    GroupRank = table.Column<int>(type: "integer", nullable: false),
                    GroupID = table.Column<int>(type: "integer", nullable: false),
                    TraderRole = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "ScoreCardLog",
                columns: table => new
                {
                    ScoreCardID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    GroupID = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    SortinoRank = table.Column<int>(type: "integer", nullable: false),
                    SharpeRank = table.Column<int>(type: "integer", nullable: false),
                    Trades = table.Column<int>(type: "integer", nullable: false),
                    Winners = table.Column<int>(type: "integer", nullable: false),
                    Losers = table.Column<int>(type: "integer", nullable: false),
                    Longs = table.Column<int>(type: "integer", nullable: false),
                    Shorts = table.Column<int>(type: "integer", nullable: false),
                    NetProfitLong = table.Column<double>(type: "double precision", nullable: false),
                    NetProfitShort = table.Column<double>(type: "double precision", nullable: false),
                    GrossProfit = table.Column<double>(type: "double precision", nullable: false),
                    GrossLoss = table.Column<double>(type: "double precision", nullable: false),
                    LargestWinner = table.Column<double>(type: "double precision", nullable: false),
                    LargestLoser = table.Column<double>(type: "double precision", nullable: false),
                    LargestWinningStreak = table.Column<int>(type: "integer", nullable: false),
                    LargestLosingStreak = table.Column<int>(type: "integer", nullable: false),
                    TotalNetProfit = table.Column<double>(type: "double precision", nullable: false),
                    WinLossRatio = table.Column<double>(type: "double precision", nullable: false),
                    AveWin = table.Column<double>(type: "double precision", nullable: false),
                    AveLoss = table.Column<double>(type: "double precision", nullable: false),
                    AveTradeDuration = table.Column<double>(type: "double precision", nullable: false),
                    AveWinDuration = table.Column<double>(type: "double precision", nullable: false),
                    AveLossDuration = table.Column<double>(type: "double precision", nullable: false),
                    StdDevAllTrades = table.Column<double>(type: "double precision", nullable: false),
                    StdDevWinTrades = table.Column<double>(type: "double precision", nullable: false),
                    StdDevLossTrades = table.Column<double>(type: "double precision", nullable: false),
                    SharpRatio = table.Column<double>(type: "double precision", nullable: false),
                    SortinoRatio = table.Column<double>(type: "double precision", nullable: false),
                    PNL_Last3 = table.Column<double>(type: "double precision", nullable: false),
                    PNL_Last5 = table.Column<double>(type: "double precision", nullable: false),
                    PNL_Last8 = table.Column<double>(type: "double precision", nullable: false),
                    PNL_Last13 = table.Column<double>(type: "double precision", nullable: false),
                    PNL_Last21 = table.Column<double>(type: "double precision", nullable: false),
                    PNL_Last34 = table.Column<double>(type: "double precision", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserProfileUserID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreCardLog", x => x.ScoreCardID);
                    table.ForeignKey(
                        name: "FK_ScoreCardLog_UserProfiles_UserProfileUserID",
                        column: x => x.UserProfileUserID,
                        principalTable: "UserProfiles",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ScoreCards",
                columns: table => new
                {
                    ScoreCardID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    GroupID = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    SortinoRank = table.Column<int>(type: "integer", nullable: false),
                    SharpeRank = table.Column<int>(type: "integer", nullable: false),
                    Trades = table.Column<int>(type: "integer", nullable: false),
                    Winners = table.Column<int>(type: "integer", nullable: false),
                    Losers = table.Column<int>(type: "integer", nullable: false),
                    Longs = table.Column<int>(type: "integer", nullable: false),
                    Shorts = table.Column<int>(type: "integer", nullable: false),
                    NetProfitLong = table.Column<double>(type: "double precision", nullable: false),
                    NetProfitShort = table.Column<double>(type: "double precision", nullable: false),
                    GrossProfit = table.Column<double>(type: "double precision", nullable: false),
                    GrossLoss = table.Column<double>(type: "double precision", nullable: false),
                    LargestWinner = table.Column<double>(type: "double precision", nullable: false),
                    LargestLoser = table.Column<double>(type: "double precision", nullable: false),
                    LargestWinningStreak = table.Column<int>(type: "integer", nullable: false),
                    LargestLosingStreak = table.Column<int>(type: "integer", nullable: false),
                    TotalNetProfit = table.Column<double>(type: "double precision", nullable: false),
                    WinLossRatio = table.Column<double>(type: "double precision", nullable: false),
                    AveWin = table.Column<double>(type: "double precision", nullable: false),
                    AveLoss = table.Column<double>(type: "double precision", nullable: false),
                    AveTradeDuration = table.Column<double>(type: "double precision", nullable: false),
                    AveWinDuration = table.Column<double>(type: "double precision", nullable: false),
                    AveLossDuration = table.Column<double>(type: "double precision", nullable: false),
                    StdDevAllTrades = table.Column<double>(type: "double precision", nullable: false),
                    StdDevWinTrades = table.Column<double>(type: "double precision", nullable: false),
                    StdDevLossTrades = table.Column<double>(type: "double precision", nullable: false),
                    SharpRatio = table.Column<double>(type: "double precision", nullable: false),
                    SortinoRatio = table.Column<double>(type: "double precision", nullable: false),
                    PNL_Last3 = table.Column<double>(type: "double precision", nullable: false),
                    PNL_Last5 = table.Column<double>(type: "double precision", nullable: false),
                    PNL_Last8 = table.Column<double>(type: "double precision", nullable: false),
                    PNL_Last13 = table.Column<double>(type: "double precision", nullable: false),
                    PNL_Last21 = table.Column<double>(type: "double precision", nullable: false),
                    PNL_Last34 = table.Column<double>(type: "double precision", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreCards", x => x.ScoreCardID);
                    table.ForeignKey(
                        name: "FK_ScoreCards_UserProfiles_UserID",
                        column: x => x.UserID,
                        principalTable: "UserProfiles",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClosedTrades_UserID",
                table: "ClosedTrades",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCardLog_UserProfileUserID",
                table: "ScoreCardLog",
                column: "UserProfileUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCards_UserID",
                table: "ScoreCards",
                column: "UserID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "ClosedTrades");

            migrationBuilder.DropTable(
                name: "LiveOrders");

            migrationBuilder.DropTable(
                name: "ModelOrderLog");

            migrationBuilder.DropTable(
                name: "OrderLog");

            migrationBuilder.DropTable(
                name: "ScoreCardLog");

            migrationBuilder.DropTable(
                name: "ScoreCards");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
