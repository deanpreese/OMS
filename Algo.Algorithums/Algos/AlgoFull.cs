using OMS.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OMS.Grains.Interfaces;
using System.Threading.Channels;
using System.Text.Json;

namespace Algo.Algorithums.Algos;

public class AlgoFull : BackgroundService
{  
    private readonly AlgoOrderQueue _algoOrderQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AlgoFull> _logger;
    private readonly IClusterClient _clusterClient;
    private readonly IGrainFactory _grainFactory;
    private readonly ChannelReader<OrderInfo> _reader;
    private AlgoData _algoData; 
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30); // Setting the interval to 30 seconds

    public AlgoFull(ILogger<AlgoFull> logger, 
            AlgoOrderQueue algoOrderQueue,
            IServiceScopeFactory scopeFactory,
            IServiceProvider serviceProvider, IClusterClient clusterClient, 
            IGrainFactory grainFactory)
    {
        _algoOrderQueue = algoOrderQueue;
        _scopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _clusterClient = clusterClient;
        _grainFactory = grainFactory;
    
        _reader = _algoOrderQueue.Subscribe();
        _algoData = new AlgoData();
       
    }
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        string filePath = "Full.json"; 

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File {filePath} does not exist.");
        }

        string jsonString = File.ReadAllText(filePath);
        _algoData = JsonSerializer.Deserialize<AlgoData>(jsonString)??
            throw new ArgumentNullException($"File {filePath} is empty.");

        Console.WriteLine("AlgoFull Started" + jsonString) ;

        return base.StartAsync(cancellationToken);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

       await Task.Run(async () =>
        {
            await foreach (var orderInfo in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    //Console.WriteLine("Order Info: " + orderInfo.UserID + "  " + orderInfo.GroupNumber + "  " + orderInfo.InfoType);
                    string g_k = orderInfo.UserID + "_" + orderInfo.GroupNumber;
                    ITraderGrain trader =  _grainFactory.GetGrain<ITraderGrain>(g_k);
                    await trader.Update(g_k);
                    UserProfile u = await trader.GetProfileAsync();
                    Console.WriteLine(" <Full> Processing order for : " + g_k + "  " + u.UserID + "  " +  "  " + orderInfo.InfoType);
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        });

        
        await Task.CompletedTask;
    }

}
