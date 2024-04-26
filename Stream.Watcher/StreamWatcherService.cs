
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

using Confluent.Kafka;

namespace Stream.Watcher;


public class StreamWatcherService : BackgroundService
{
    private readonly ILogger<StreamWatcherService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    ScreenColorBase scb  = new ScreenColorBase();

    private  string topic;
    private  IConsumer<string, string> kafkaConsumer;
    
    public StreamWatcherService(ILogger<StreamWatcherService> logger, ILoggerFactory loggerFactory )
    {
        _logger = logger;
        _loggerFactory = loggerFactory;

        _logger.LogInformation("Starting Stream Watcher...");
        

        topic = PlatformConstants.KAFKA_TOPIC_NAME;

        var config = new ConsumerConfig
        {
            BootstrapServers = PlatformConstants.KAFKA_BOOTSTRAP_SERVERS,
            GroupId = "foo",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        kafkaConsumer = new ConsumerBuilder<string, string>(config).Build();

        
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
            return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
    }
        
    private void StartConsumerLoop(CancellationToken cancellationToken)
    {
        kafkaConsumer.Subscribe(topic);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var cr = kafkaConsumer.Consume(cancellationToken);

                // Handle message...
                Console.WriteLine($"{cr.Message.Value}");
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ConsumeException e)
            {
                // Consumer errors should generally be ignored (or logged) unless fatal.
                Console.WriteLine($"Consume error: {e.Error.Reason}");

                if (e.Error.IsFatal)
                {
                    // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e}");
                break;
            }
        }
    }
        
    public override void Dispose()
    {
        kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
        kafkaConsumer.Dispose();

        base.Dispose();
    }

    
}

