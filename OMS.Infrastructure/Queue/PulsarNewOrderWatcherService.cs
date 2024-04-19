using System.Text.Json;
using System.Text.Json.Serialization;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OMS.SharedKernel;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace OMS.Infrastructure.Queue;

public class PulsarNewOrderWatcherService : BackgroundService
{
    private readonly ILogger<PulsarNewOrderWatcherService> _logger;
    ScreenColorBase scb  = new ScreenColorBase();

    IPulsarClient  _pulsarClient;
    IConsumer<string> _consumer;
    
    public PulsarNewOrderWatcherService(ILogger<PulsarNewOrderWatcherService> logger, ILoggerFactory loggerFactory )
    {
        _logger = logger;

        _logger.LogInformation("Starting ReplWatcher...");
        System.Uri uri = new System.Uri(PlatformConstants.pulsar_uri_string);
        _pulsarClient = PulsarClient.Builder().ServiceUrl(uri).Build();

        _consumer = _pulsarClient.NewConsumer(Schema.String)
            .SubscriptionName("PulsarNewOrderWatcherService")
            .Topic(PlatformConstants.PULSAR_NEW_ORDER_TOPIC)
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

            Console.WriteLine($"{scb.GREEN}  Received: {newOrderDTO.UserID} {newOrderDTO.GroupID}  {newOrderDTO.UserName}  {newOrderDTO.OrderAction} ");
            Console.ResetColor();
            
        }catch(Exception ex)
        {
            Console.WriteLine($"Received: {ex.Message} ");
        }
        
        return ValueTask.CompletedTask;
    }
}

