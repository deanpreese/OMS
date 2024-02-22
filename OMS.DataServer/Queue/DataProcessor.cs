using OMS.Core.Models;
using OMS.Core.Common;
using OMS.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System.Data;


namespace OMS.DataServer;

public class DataProcessor : BackgroundService
{
    private readonly DataQueue _dataChannelService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataQueue> _logger;

    
    public DataProcessor( ILogger<DataQueue> logger, 
            DataQueue dataChannelService,
            IServiceScopeFactory scopeFactory,
                IServiceProvider serviceProvider 
              )
    {
        _dataChannelService = dataChannelService;
        _scopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(async () =>
        {
            await foreach (var featureData in _dataChannelService.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await ProcessDataAsync(featureData, stoppingToken);   
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        });
    }

    private async Task ProcessDataAsync(FeatureData featureData, CancellationToken cancellationToken)
    {
        Console.WriteLine(featureData.Instrument + "  " + featureData.FeatureSetName + "  " + featureData.TimeTicks + "  " + new DateTime(featureData.TimeTicks) + "  " + featureData.FeatureNameData + "  " + featureData.FeatureSetData);    
        AnsiConsole.MarkupLine(" ");    
        await Task.CompletedTask; 
    }

   
}
