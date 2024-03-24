using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

using System.Text.Json;

using Npgsql;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using PgOutput2Json;
using ReplicationTest;

public class TestWatcher : BackgroundService
{
    private readonly ILoggerFactory _loggerFactory;

    public TestWatcher(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
        string publication = "liveupdatespub";

        using var pgOutput2Json = PgOutput2JsonBuilder.Create()
            .WithLoggerFactory(_loggerFactory)
            .WithPgConnectionString("Host=localhost;Database=orders;Username=trading;Password=abc")
            .WithPgPublications(publication)
	        .WithMessageHandler((json, table, key, partition) =>
            {
                if( table == "public.ModelOrderLog")
                {

                    var orderModelResult = JsonSerializer.Deserialize<ModelOrderLogJSON>(json, options);

                    Console.WriteLine($"ModelOrderLog {orderModelResult._ct}   UserID: {orderModelResult.UserID}  {orderModelResult.ModelOrderLogID}  ");

                    Console.WriteLine(orderModelResult.ModelFeatureData);
                    Console.WriteLine(orderModelResult.ScoreCardJSON);
                    
                }

                if( table == "public.LiveOrder")
                {
                    var liveResult = JsonSerializer.Deserialize<LiveOrderJSON>(json, options);
                    //Console.WriteLine($"LiveOrder Action {liveResult._ct}   UserID: {liveResult.UserID}");

                    if(liveResult._ct == "I")
                    {
                        //Console.WriteLine($" +++++++ LiveOrder New {liveResult._ct}   UserID: {liveResult.UserID}");
                    }

                    if(liveResult._ct == "D")
                    {
                        //Console.WriteLine($" ------- LiveOrder Delete {liveResult._ct} ");
                    }
                }

                if( table == "public.ScoreCard")
                {
                    var scoreCardResult = JsonSerializer.Deserialize<ScoreCardJSONData>(json, options);
                    //Console.WriteLine($" ^^^^ ScoreCard Action {scoreCardResult._ct}   UserID: {scoreCardResult.UserID}");
                }

                if( table == "public.ClosedTrades")
                {
                    var closedTradeJSON = JsonSerializer.Deserialize<ClosedTradeJSON>(json, options);
                    //Console.WriteLine($" xxxx ClosedTrades Action {closedTradeJSON._ct}   UserID: {closedTradeJSON.UserID}  {closedTradeJSON.OpenLiveOrderID}  ");
                }
                
            })
            .Build();

        await pgOutput2Json.Start(stoppingToken);
    }
}