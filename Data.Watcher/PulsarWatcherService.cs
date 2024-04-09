
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


public class PulsarWatcherService : BackgroundService
{
    private readonly ILogger<PulsarWatcherService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    ScreenColorBase scb  = new ScreenColorBase();

    
    int order = 0;
    int closed = 0;
    int score = 0;
    int model = 0;

    const string myTopic = "persistent://public/default/mytopic";
    IPulsarClient  _pulsarClient;
    IProducer<string> _producer;

    IConsumer<string> _consumer;
    
    public PulsarWatcherService(ILogger<PulsarWatcherService> logger, ILoggerFactory loggerFactory )
    {
        _logger = logger;
        _loggerFactory = loggerFactory;

        _logger.LogInformation("Starting ReplWatcher...");
        System.Uri uri = new System.Uri("pulsar://10.0.0.74:6650");
        _pulsarClient = PulsarClient.Builder().ServiceUrl(uri).Build();

        _consumer = _pulsarClient.NewConsumer(Schema.String)
            .SubscriptionName("Sub1")
            .Topic(myTopic)
            .InitialPosition(SubscriptionInitialPosition.Earliest)
            .Create();

        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      await _consumer.Process(ProcessMessage, stoppingToken);
    }

    private ValueTask ProcessMessage(IMessage<string> message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Received: {message.Value()}");
        return ValueTask.CompletedTask;
    }
}

