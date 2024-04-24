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
using DotPulsar.Abstractions;
using DotPulsar;
using DotPulsar.Extensions;
using System.Text.Json;

namespace OMS.Infrastructure.Queue;

public class NewOrderProcessorService : BackgroundService
{
    private readonly NewOrderChannelService _orderChannelService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NewOrderProcessorService> _logger;
    private IPlatformOrderIDGen _platformOrderIDGen;
    
    private OrderManagerService _oms;

    //IPulsarClient  _pulsarClient;
    //IProducer<string> _producer;


    public NewOrderProcessorService(ILogger<NewOrderProcessorService> logger,
            NewOrderChannelService newOrderChannelService,
            IServiceScopeFactory scopeFactory,
                IPlatformOrderIDGen platformOrderIDGen,
                OrderManagerService oms
              )
    {
        _orderChannelService = newOrderChannelService;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _platformOrderIDGen = platformOrderIDGen;
        _oms = oms;

        //System.Uri uri = new System.Uri(PlatformConstants.pulsar_uri_string);
        //_pulsarClient = PulsarClient.Builder().ServiceUrl(uri).Build();
        //_producer = _pulsarClient.NewProducer(Schema.String).Topic(PlatformConstants.PULSAR_MODEL_ORDER_LOG_TOPIC).Create();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var liveOrder in _orderChannelService.ReadAllAsync(stoppingToken))
        {
            try
            {
                await _oms.ProcessNewTraderOrder(liveOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

}
