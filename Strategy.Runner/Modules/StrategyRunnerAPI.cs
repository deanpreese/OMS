using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using OMS.SharedKernel.DTO;
using Strategy.Runner.Services;
using Strategy.SharedKernel;

namespace Strategy.Runner.Modules;

public static class StrategyRunnerAPI
{
 
    public static IEndpointRouteBuilder MapStrategyRunnerEndpoints(this IEndpointRouteBuilder endpoints)
    {

        endpoints.MapPost("api/strategy/evaluate", async (StrategyRunnerService strategyService, ModelOrderLogDTO traderOrderLogDTO) => 
        {
            NewOrderDTO strategyOrder = new NewOrderDTO();
            strategyOrder = await strategyService.EvaluateStrategy(traderOrderLogDTO);
            return strategyOrder;
        }
        )
        .WithName("EvaluateStrategy")
        .WithOpenApi();

        return endpoints;   
    }


}
