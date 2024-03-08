using OMS.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading.Channels;
using OMS.Core.Common;
using Strategy.Trader.Strategy;
using Strategy.Trader.Utility;
using Strategy.Trader.Models;
using OMS.Core.Interfaces;
using Strategy.Trader.Abstractions;
using Strategy.Trader;

namespace Strategy.Server.Services;

public class FadeService : AbstractStrategyService
{
    IClusterClient _clusterClient;
    StrategyAccount _strategyAccount;

    public FadeService(ILogger<AbstractStrategyService> logger, StrategyOrderQueue strategyOrderQueue, IClusterClient client) 
    : base(logger, strategyOrderQueue)
    {
        _clusterClient = client;
        _strategyAccount = new StrategyAccount();
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        string strategy_to_load = "NG3.json";
        StrategyConfig configLoader = new StrategyConfig(strategy_to_load, _clusterClient);
        _strategyAccount= configLoader.GetStrategyData().Result;

        loadedStrategy = new BaseFadeStrategy(_clusterClient, _strategyAccount);

        await base.StartAsync(cancellationToken);
    }
}

