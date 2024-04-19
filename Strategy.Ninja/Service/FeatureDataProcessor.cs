
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Data;
using System.Text;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using System.Net.Http;




namespace Strategy.Ninja.Service;

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

    private async Task ProcessDataAsync(FeatureDataDTO featureData, CancellationToken cancellationToken)
    {
        DateTime time = new DateTime(featureData.TimeTicks);
        string iso8601String = time.ToString("o");

        string csv_data = iso8601String + "," + featureData.FeatureSetData;

        string o_s = featureData.Instrument + "  " + featureData.FeatureSetName + "  " +  new DateTime(featureData.TimeTicks) + "  " + featureData.Close + "  " + featureData.FeatureSetData;        

        if (featureData.TimeTicks < DateTime.UtcNow.Ticks - 150000000 )
        {
            await SendCsvDataAsync(csv_data, "http://10.0.147:8888/predict");    
            Console.WriteLine("Hist: " + featureData.Instrument + "  " + featureData.FeatureSetName + "  " +  new DateTime(featureData.TimeTicks) + "  UTC " + featureData.FeatureSetData);        
        }else
        {
            //await SendCsvDataAsync(csv_data, "http://10.0.0.147:8888/predict");        
            Console.WriteLine("RT: " + featureData.Instrument + "  " + featureData.FeatureSetName + "   UTC " +new DateTime(featureData.TimeTicks) );    
        }
        
        await Task.CompletedTask; 
    }
   

    public async Task<string> SendCsvDataAsync(string csvData, string url )
    {
        var _httpClient = new HttpClient();
        try
        {
            // Assuming the server expects the content type to be 'text/csv'
            var content = new StringContent(csvData, Encoding.UTF8, "text/csv");

            // Perform the POST request
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            // Read the response body
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
            return e.Message;
        }
    }



}
