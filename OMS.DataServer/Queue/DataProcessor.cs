using OMS.Core.Models;
using OMS.Core.Common;
using OMS.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System.Data;
using System.Text;
using OMS.Core.WebAPIClient;


namespace OMS.DataServer;

public class DataProcessor : BackgroundService
{
    private readonly DataQueue _dataChannelService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataQueue> _logger;
    private readonly HttpClient _httpClient;    
    
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
        _httpClient = new HttpClient();
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

        if (featureData.TimeTicks < DateTime.UtcNow.Ticks - 150000000 )
        {
            //await CallExternalWebService(csv_data);
            await OMSClient.SendToMLForPrediction(csv_data);    
            Console.WriteLine("Hist: " + featureData.Instrument + "  " + featureData.FeatureSetName + "  " + featureData.TimeTicks + "  " + new DateTime(featureData.TimeTicks) + "  " + featureData.FeatureNameData + "  " + featureData.FeatureSetData);        
        }else
        {
            //await CallExternalWebService(csv_data);
            await OMSClient.SendToMLForPrediction(csv_data);    
            Console.WriteLine("RT: " + featureData.Instrument + "  " + featureData.FeatureSetName + "  " + featureData.TimeTicks + "  " + new DateTime(featureData.TimeTicks) + "  " + featureData.FeatureNameData + "  " + featureData.FeatureSetData);    
        }
        
        //AnsiConsole.MarkupLine(" ");    
        await Task.CompletedTask; 
    }

    private async Task CallExternalWebServiceX(string csvData)
    {
        // Your API endpoint
        string url = "http://localhost:8888/predict";

        HttpContent content = new StringContent(csvData, Encoding.UTF8, "text/csv");

        // Create an HttpClient instance
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.PostAsync(url, content);

            // Check the response
            if (response.IsSuccessStatusCode)
            {
                // Handle success
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                // Process the response body as needed
            }
            else
            {
                // Handle failure
            }
        }
    }


   
}
