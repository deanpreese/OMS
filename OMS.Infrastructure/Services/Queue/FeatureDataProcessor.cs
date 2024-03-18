using OMS.Core.Models;
using OMS.Core.Common;
using OMS.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Data;
using System.Text;
using OMS.Core.WebAPIClient;


namespace OMS.Infrastructure.Services.Queue;

public class FeatureDataProcessor : BackgroundService
{
    private readonly FeatureDataDataQueue _dataChannelService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FeatureDataProcessor> _logger;
    
    public FeatureDataProcessor( ILogger<FeatureDataProcessor> logger, 
            FeatureDataDataQueue dataChannelService,
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
        DateTime time = new DateTime(featureData.TimeTicks);
        string iso8601String = time.ToString("o");

        string csv_data = iso8601String + "," + featureData.FeatureSetData;

        string o_s = featureData.Instrument + "  " + featureData.FeatureSetName + "  " +  new DateTime(featureData.TimeTicks) + "  " + featureData.Close + "  " + featureData.FeatureSetData;        

        if (featureData.TimeTicks < DateTime.UtcNow.Ticks - 150000000 )
        {
<<<<<<< HEAD:OMS.NinjaTrader/Queue/DataProcessor.cs
            await OMSClient.SendToMLForPrediction(csv_data, "http://10.0.147:8888/predict");    
            Console.WriteLine("Hist: " + featureData.Instrument + "  " + featureData.FeatureSetName + "  " +  new DateTime(featureData.TimeTicks));        
        }else
        {
           await OMSClient.SendToMLForPrediction(csv_data, "http://10.0.0.147:8888/predict");        
            Console.WriteLine("RT: " + featureData.Instrument + "  " + featureData.FeatureSetName + "  " +new DateTime(featureData.TimeTicks) );    
=======
            //await OMSClient.SendToMLForPrediction(csv_data, "http://localhost:8888/predict");    
            Console.WriteLine("Hist: " + o_s);        
        }else
        {
            //await OMSClient.SendToMLForPrediction(csv_data, "http://localhost:8888/predict");        
            Console.WriteLine("RT: " + o_s);        
>>>>>>> 1785b034a2e02d13ef13fc59dc7fde4f86159b15:OMS.Infrastructure/Services/Queue/FeatureDataProcessor.cs
        }
        
        await Task.CompletedTask; 
    }
   
}
