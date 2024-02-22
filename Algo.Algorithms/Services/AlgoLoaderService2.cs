using OMS.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OMS.Grains.Interfaces;
using System.Threading.Channels;
using System.Text.Json;

using Algo.Algorithms.Models;
using Algo.Algorithms.Services;
using System.Text;
using OMS.Services.Trading;
using OMS.Core.Common;


namespace Algo.Algorithms.Services;

public class AlgoLoaderService2 : BackgroundService
{  
    private string algo_to_load = "NG2.json";
    private readonly AlgoOrderQueue _algoOrderQueue;
    private readonly ILogger<AlgoLoaderService2> _logger;
    private readonly IGrainFactory _grainFactory;
    private AlgoData _algoData; 
    ChannelReader<LiveOrder> _reader;
    private readonly IClusterClient _client;
    

    public AlgoLoaderService2(ILogger<AlgoLoaderService2> logger, 
            AlgoOrderQueue algoOrderQueue,
            IGrainFactory grainFactory, IClusterClient client)
    {
        _algoOrderQueue = algoOrderQueue;
        _logger = logger;
        _grainFactory = grainFactory;
        _client = client;

        _algoData = new AlgoData();
        _reader = _algoOrderQueue.Subscribe();
       
    }
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
       AlgoConfig configLoader = new AlgoConfig(algo_to_load, _grainFactory, _client);
       _algoData = configLoader.GetAlgoData().Result;        
       await base.StartAsync(cancellationToken);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

         await Task.Run(async () =>
        {
            //await foreach (var orderInfo in _algoOrderQueue.ReadAllAsync(stoppingToken))
            await foreach (var newOrderInfo in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    //Console.WriteLine("Order Info: " + orderInfo.UserID + "  " + orderInfo.GroupNumber + "  " + orderInfo.InfoType);
                    string g_k = newOrderInfo.UserID + "_" + newOrderInfo.UserGroup;
                    ITraderGrain trader =  _grainFactory.GetGrain<ITraderGrain>(g_k);
                    await trader.Update(g_k);
                    UserProfile u = await trader.GetProfileAsync(g_k);
                    Console.WriteLine(" ---> AlgoNG2 for : " + g_k + "  " + newOrderInfo.OrderPX + "  " +  "  " + newOrderInfo.OrderAction);

                    IOrderGrain orderGrain = _grainFactory.GetGrain<IOrderGrain>(_algoData.algo_grain());    

                    NewOrder n_o = OrderMapping.MapOrder(newOrderInfo);
                    n_o.UserGroup = _algoData.group;
                    n_o.UserID = _algoData.algo_traderId;

                    OrderAction n = n_o.OrderAction;

                    if(n == OrderAction.Sell)
                    {
                        n_o.OrderAction = OrderAction.Buy;
                    }
                    if(n == OrderAction.Buy)
                    {
                        n_o.OrderAction = OrderAction.Sell;
                    }
                    
                    await orderGrain.ProcessOrder(n_o);
                    
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
