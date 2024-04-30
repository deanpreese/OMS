using System.Threading.Channels;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using Strategy.Trader.Abstractions;
using Strategy.Trader;
using Strategy.Runner.Utility;

namespace Strategy.Runner.Services;

public class StrategyService : BackgroundService
{
    public IStrategy loadedStrategy ;
    public IStrategyConnection _strategyConnection;
    public IServiceScopeFactory _serviceScopeFactory;
    private StrategyRunnerService _strategyRunnerService;

     public StrategyService(StrategyRunnerService strategyRunnerService) 
    {
        _strategyRunnerService = strategyRunnerService;
    }
    
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _strategyRunnerService.LoadService();
        await base.StartAsync(cancellationToken);
    }

    
   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.CompletedTask;
    }


}
