
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

using OMS.SharedKernel.Common;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OMS.SharedKernel;
using Strategy.SharedKernel;


using DotPulsar;
using DotPulsar.Extensions;
using DotPulsar.Abstractions;



namespace Pulsar.Watcher;


public class PulsarStrategyOrderWatcherService : BackgroundService
{
    private readonly ILogger<PulsarStrategyOrderWatcherService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    ScreenColorBase scb  = new ScreenColorBase();
    IPulsarClient  _pulsarStrategyClient;
    IPulsarClient  _pulsarModelClient;
    IConsumer<string> _strategyConsumer;
    IConsumer<string> _modelConsumer;
    
    public PulsarStrategyOrderWatcherService(ILogger<PulsarStrategyOrderWatcherService> logger, ILoggerFactory loggerFactory )
    {
        _logger = logger;
        _loggerFactory = loggerFactory;

        _logger.LogInformation("Starting PulsarWatcher...");
        System.Uri uri = new System.Uri(PlatformConstants.pulsar_uri_string);
        _pulsarStrategyClient = PulsarClient.Builder().ServiceUrl(uri).Build();
        _pulsarModelClient = PulsarClient.Builder().ServiceUrl(uri).Build();


        _strategyConsumer = _pulsarStrategyClient.NewConsumer(Schema.String)
            .SubscriptionName("PulsarStrategyWatcherService")
            .Topic(PlatformConstants.PULSAR_STRATEGY_ORDER_TOPIC)
            .InitialPosition(SubscriptionInitialPosition.Earliest)
            .Create();

        _modelConsumer = _pulsarModelClient.NewConsumer(Schema.String)
            .SubscriptionName("PulsarModelWatcherService")
            .Topic(PlatformConstants.PULSAR_MODEL_ORDER_LOG_TOPIC)
            .InitialPosition(SubscriptionInitialPosition.Earliest)
            .Create();


        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      await _strategyConsumer.Process(ProcessStrategyMessage, stoppingToken);
      await _modelConsumer.Process(ProcessModelMessage, stoppingToken);
    }

    private async ValueTask ProcessModelMessage(IMessage<string> message, CancellationToken token)
    {
        JsonSerializerOptions options = new JsonSerializerOptions {
        //NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
        Converters ={
            new JsonStringEnumConverter(),
            new CustomDoubleConverter()
        },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyProperties = true
        };

        try
        {
            ModelOrderLogDTO modelOrderLogDTO = JsonSerializer.Deserialize<ModelOrderLogDTO>(message.Value());
            Console.WriteLine($"ModelOrderLog Message: {modelOrderLogDTO.UserID} {modelOrderLogDTO.GroupID}  {modelOrderLogDTO.OrderAction} " );
            //Console.WriteLine($"{scb.CYAN} {modelOrderLogDTO.LiveOrderJson} " );
            //Console.WriteLine($" {scb.GREEN}{modelOrderLogDTO.ClosedOrderJson} " );
            //Console.WriteLine($" {scb.YELLOW}{modelOrderLogDTO.ScoreCardJson} " );
            Console.ResetColor();
            
        }catch(Exception ex)
        {
            Console.WriteLine($"Received: {ex.Message} ");
        }
        
        await ValueTask.CompletedTask;
    }

    private async ValueTask ProcessStrategyMessage(IMessage<string> message, CancellationToken cancellationToken)
    {
        JsonSerializerOptions options = new JsonSerializerOptions {
        //NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
        Converters ={
            new JsonStringEnumConverter(),
            new CustomDoubleConverter()
        },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyProperties = true
        };

        try
        {
            NewOrderDTO newOrderDTO = JsonSerializer.Deserialize<NewOrderDTO>(message.Value(), options);
            Console.WriteLine($"Strategy Message: {newOrderDTO.UserID} {newOrderDTO.GroupID}  {newOrderDTO.UserName}  {newOrderDTO.OrderAction} {newOrderDTO.OrderTime} {newOrderDTO.Instrument} {newOrderDTO.OrderType} {newOrderDTO.OrderPX} {newOrderDTO.Quantity} " );

            
        }catch(Exception ex)
        {
            Console.WriteLine($"Received: {ex.Message} ");
        }
        
        await ValueTask.CompletedTask;
    }
}

