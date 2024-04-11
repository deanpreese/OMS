
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



namespace Data.Watcher;


public class PulsarStrategyOrderWatcherService : BackgroundService
{
    private readonly ILogger<PulsarStrategyOrderWatcherService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    ScreenColorBase scb  = new ScreenColorBase();

    IPulsarClient  _pulsarClient;
    IProducer<string> _producer;

    IConsumer<string> _consumer;
    
    public PulsarStrategyOrderWatcherService(ILogger<PulsarStrategyOrderWatcherService> logger, ILoggerFactory loggerFactory )
    {
        _logger = logger;
        _loggerFactory = loggerFactory;

        _logger.LogInformation("Starting ReplWatcher...");
        System.Uri uri = new System.Uri(PlatformConstants.pulsar_uri_string);
        _pulsarClient = PulsarClient.Builder().ServiceUrl(uri).Build();

        _consumer = _pulsarClient.NewConsumer(Schema.String)
            .SubscriptionName("PulsarWatcherService")
            .Topic(PlatformConstants.pulsar_strategy_topic)
            .InitialPosition(SubscriptionInitialPosition.Earliest)
            .Create();

        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      await _consumer.Process(ProcessMessage, stoppingToken);
    }

    private ValueTask ProcessMessage(IMessage<string> message, CancellationToken cancellationToken)
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
            Console.WriteLine($"Received: {newOrderDTO.UserID} {newOrderDTO.GroupID}  {newOrderDTO.UserName}  {newOrderDTO.OrderAction} ");

            
        }catch(Exception ex)
        {
            Console.WriteLine($"Received: {ex.Message} ");
        }
        
        return ValueTask.CompletedTask;
    }
}

