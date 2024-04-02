
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;

using Strategy.Server.Services;
using Strategy.Server.Utility;

using Strategy.Trader.Strategy;
using Strategy.Trader.Abstractions;
using Strategy.Trader;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;
using Orleans.Configuration;
using Strategy.Server.Abstractions;
using Microsoft.Extensions.DependencyInjection;



namespace Strategy.Server.StrategyServices;

public class NGThree :  BaseStrategyService
{
    public NGThree(IServiceScopeFactory serviceScopeFactory, ModelOrderMessageBus messageBus) 
    : base(serviceScopeFactory, messageBus)
    {
    }
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        string strategy_to_load = "NG3.json";
        StrategyConfig configLoader = new StrategyConfig(strategy_to_load);
        _strategyConnection = configLoader.GetStrategyConnection().Result;

        loadedStrategy = new BaseFadeStrategy(_strategyConnection );
        return base.StartAsync(cancellationToken);
    }

}

