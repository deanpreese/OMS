using OMS.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Threading.Channels;
using System.Text.Json;

using System.Text;
using OMS.Core.Common;
using Strategy.Trader.Models;
using Strategy.Trader.Utility;
using Strategy.Trader.Strategy;
using OMS.Core.Interfaces;

namespace Strategy.Server.Services;

public class FollowService : AbstractStrategyService
{  
    
    public FollowService(ILogger<FollowService> logger, StrategyOrderQueue strategyOrderQueue, IClusterClient client) 
            : base(logger, strategyOrderQueue, client)
    {
    }
    
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        string strategy_to_load = "NG2.json";
        StrategyConfig configLoader = new StrategyConfig(strategy_to_load, clusterClient);
        strategyAccount= configLoader.GetStrategyData().Result;

        loadedStrategy = new BaseFollowStrategy(clusterClient, strategyAccount);

        await base.StartAsync(cancellationToken);
    }
}
