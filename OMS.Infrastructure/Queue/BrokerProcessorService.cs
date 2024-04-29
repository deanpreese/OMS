using OMS.Application.Models;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Services;
using OMS.Infrastructure.Interfaces;
using OMS.Application;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

using Confluent.Kafka;

namespace OMS.Infrastructure.Queue;

public class BrokerProcessorService : BackgroundService
{
    private readonly BrokerChannelService _brokerChannelService;
    private readonly ILogger<BrokerProcessorService> _logger;
    private  IProducer<string, string> _producer;   

    public BrokerProcessorService(ILogger<BrokerProcessorService> logger,
            BrokerChannelService brokerChannelService)
    {
        _logger = logger;
        _brokerChannelService = brokerChannelService;

        var config = new ProducerConfig
        {
            BootstrapServers = PlatformConstants.KAFKA_BOOTSTRAP_SERVERS,
        };
        _producer = new ProducerBuilder<string, string>(config).Build();

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var liveOrder in _brokerChannelService.ReadAllAsync(stoppingToken))
        {
            try
            {
                _producer.Produce(PlatformConstants.MODEL_ORDER_TOPIC_NAME, new Message<string, string> { Value = JsonSerializer.Serialize(liveOrder) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

}
