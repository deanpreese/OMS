using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

using PgOutput2Json;
using Strategy.Runner.Services;
using OMS.SharedKernel.Common;

using Strategy.SharedKernel;

namespace Strategy.Runner.Services;

public class WatcherService : BackgroundService
{
    private readonly ILogger<WatcherService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    ModelOrderMessageBus _messageBus;
    ScreenColorBase scb  = new ScreenColorBase();

    public WatcherService(ILogger<WatcherService> logger, ILoggerFactory loggerFactory, ModelOrderMessageBus messageBus)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
        _messageBus = messageBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string publication = "liveupdates";

        using var pgOutput2Json = PgOutput2JsonBuilder.Create()
            .WithLoggerFactory(_loggerFactory)
            .WithPgConnectionString(PlatformConstants.CURRENT_CONN)
            .WithPgPublications(publication)
            .WithMessageHandler(async (json, table, key, partition) =>
            {
            
                if (table == "public.ScoreCard")
                {
                    //ScoreCardDTO scoreCardResult = JsonSerializer.Deserialize<ScoreCardDTO>(json);
                    //Console.WriteLine($"{scb.GREEN}");
                    //Console.WriteLine($"ScoreCardJson: {scoreCardResult.ScoreCardJson}");
                    //Console.ResetColor();
                }   

                if( table == "public.ClosedTrades")
                {
                    //ModelTraderDTO traderModelResult = JsonSerializer.Deserialize<ModelTraderDTO>(json);
                    //Console.WriteLine($"{scb.RED}");
                    //Console.WriteLine($"ScoreCardJson: {traderModelResult.ScoreCardJson}");
                    //Console.ResetColor();
                }    

                if( table == "public.ModelOrderLog")
                {
                    if ( json != null )
                    {
                        ModelOrderLogDTO orderModelResult = JsonSerializer.Deserialize<ModelOrderLogDTO>(json);
                        //Console.WriteLine($"{scb.YELLOW}");
                        //Console.WriteLine($"ScoreCardJson: {orderModelResult.ScoreCardJson}");
                        //Console.ResetColor();

                        //Console.ResetColor();

                        /*
                        if ((orderModelResult.ScoreCardJson == "null")  || (orderModelResult.ScoreCardJson == "[]")
                            || orderModelResult.ScoreCardJson.Equals(null) )
                            
                        {
                            
                        }

                        Console.ResetColor();
                        /*
                        Console.WriteLine($"{scb.CYAN}");
                        Console.WriteLine($"ClosedOrderJson: {orderModelResult.ClosedOrderJson}");
                        Console.ResetColor();

                        if ((orderModelResult.ClosedOrderJson == "null")  || (orderModelResult.ClosedOrderJson == "[]")
                            || orderModelResult.ClosedOrderJson.Equals(null) )
                            
                        {
                            cloNulls++;
                        }

                        Console.WriteLine($"{scb.MAGENTA}");                    
                        Console.WriteLine($"LiveOrderJson: {orderModelResult.LiveOrderJson}");
                        Console.ResetColor();

                        if ((orderModelResult.LiveOrderJson == "null")  || (orderModelResult.LiveOrderJson == "[]")
                            || orderModelResult.LiveOrderJson.Equals(null) )
                            
                        {
                            livNulls++;
                        }   
                        Console.WriteLine($"{scoNulls} {cloNulls} {livNulls}");    
                        */

                        //await _messageBus.PublishAsync(orderModelResult);
                    }
                    //await Task.CompletedTask;
                }

            })
            .Build();

        await pgOutput2Json.Start(stoppingToken);
    }
}