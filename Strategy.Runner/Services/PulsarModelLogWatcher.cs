
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


namespace Strategy.Runner.Services;

public class PulsarModelLogWatcher : BackgroundService
{
    private readonly ILogger<PulsarModelLogWatcher> _logger;
    ModelOrderMessageBus _messageBus;
    ScreenColorBase scb  = new ScreenColorBase();
    IPulsarClient  _pulsarModelClient;
    IConsumer<string> _modelConsumer;

    
    public PulsarModelLogWatcher(ILogger<PulsarModelLogWatcher> logger , ModelOrderMessageBus messageBus)
    {
        _logger = logger;
        _messageBus = messageBus;

        _logger.LogInformation("Starting PulsarModelLogWatcher...");
        System.Uri uri = new System.Uri(PlatformConstants.PULSAR_URI);
        _pulsarModelClient = PulsarClient.Builder().ServiceUrl(uri).Build();

        _modelConsumer = _pulsarModelClient.NewConsumer(Schema.String)
            .SubscriptionName("PulsarModelLogWatcher")
            .Topic(PlatformConstants.PULSAR_MODEL_ORDER_LOG_TOPIC)
            .InitialPosition(SubscriptionInitialPosition.Earliest)
            .Create();


        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      await _modelConsumer.Process(ProcessModelMessage, stoppingToken);
    }

    private async ValueTask ProcessModelMessage(IMessage<string> message, CancellationToken token)
    {
        try
        {
            ModelOrderLogDTO modelOrderLogDTO = JsonSerializer.Deserialize<ModelOrderLogDTO>(message.Value());
            
            if (modelOrderLogDTO.OrderType == 2)
            {
                Console.WriteLine($" {scb.GREEN}" );
            }

            if (modelOrderLogDTO.OrderType == -2)
            {
                Console.WriteLine($" {scb.MAGENTA}" );
            }

            Console.WriteLine($"ModelOrderLog: {modelOrderLogDTO.UserID} {modelOrderLogDTO.GroupID}  {modelOrderLogDTO.OrderAction} " );
            //Console.WriteLine($"{scb.CYAN} {modelOrderLogDTO.LiveOrderJson} " );
            //Console.WriteLine($" {scb.GREEN}{modelOrderLogDTO.ClosedOrderJson} " );
            //Console.WriteLine($" {scb.YELLOW}{modelOrderLogDTO.ScoreCardJson} " );
            Console.ResetColor();
            
            await _messageBus.PublishAsync(modelOrderLogDTO);

        }catch(Exception ex)
        {
            Console.WriteLine($"Received: {ex.Message} ");
        }
        
        await ValueTask.CompletedTask;
    }

}

