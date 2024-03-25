using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;


using System.Text.Json;

using Npgsql;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using PgOutput2Json;
using OMS.SharedKernel.DTO;
using Strategy.Server.Services;
using Strategy.Server.Models;

public class WatcherService : BackgroundService
{
    private readonly ILogger<WatcherService> _logger;
    private readonly ILoggerFactory _loggerFactory;

    private readonly IncomingOrderQueue _incomingOrderQueue;

    public WatcherService(ILogger<WatcherService> logger, ILoggerFactory loggerFactory, IncomingOrderQueue incomingOrderQueue)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
        _incomingOrderQueue = incomingOrderQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
        string publication = "liveupdates";

        using var pgOutput2Json = PgOutput2JsonBuilder.Create()
            .WithLoggerFactory(_loggerFactory)
            .WithPgConnectionString("Host=localhost;Database=orders;Username=trading;Password=abc")
            .WithPgPublications(publication)
	        .WithMessageHandler(async (json, table, key, partition) =>
            {
                if( table == "public.ModelOrderLog")
                {
                    ModelOrderLogDTO orderModelResult = JsonSerializer.Deserialize<ModelOrderLogDTO>(json, options);
                    //Console.WriteLine($"ModelOrderLog {orderModelResult._ct}    UserID: {orderModelResult.UserID}  ModelOrderLogID: {orderModelResult.ModelOrderLogID} LiveOrderIDReference {orderModelResult.LiveOrderIDReference}  ScorecardID  {orderModelResult.ScoreCardDeserialized.ScoreCardID}  OrderType {orderModelResult.LiveOrderDeserialized.OrderType} ");
                    await _incomingOrderQueue.WriteAsync(orderModelResult);
                }

            })
            .Build();

        await pgOutput2Json.Start(stoppingToken);
    }
}