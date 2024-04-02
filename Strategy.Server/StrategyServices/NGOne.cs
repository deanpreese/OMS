
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;
using System.Threading.Channels;

using Strategy.Server.Services;
using Strategy.Server.Utility;

//using Strategy.Trader.Strategy;
//using Strategy.Trader.Abstractions;
//using Strategy.Trader;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using Strategy.SharedKernel;
using Strategy.Trader.Abstractions;
using Strategy.Trader;
using Strategy.Trader.Strategy;
using Strategy.Server.Abstractions;

namespace Strategy.Server.StrategyServices;



public class NGOne :  BaseStrategyService
{
    public NGOne(IServiceScopeFactory serviceScopeFactory, ModelOrderMessageBus messageBus) : base(serviceScopeFactory, messageBus)
    {
    }

    public override  async Task StartAsync(CancellationToken cancellationToken)
    {
        string strategy_to_load = "NG1.json";
        StrategyConfig configLoader = new StrategyConfig(strategy_to_load);
        _strategyConnection = await configLoader.GetStrategyConnection();

        loadedStrategy = new OpenCloseStrategy(_strategyConnection );
        await base.StartAsync(cancellationToken);
    }

}
