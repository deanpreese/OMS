using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using OMS.SharedKernel.DTO;
using Strategy.Runner.Services;

namespace Strategy.Runner.Modules;

public static class StrategyRunnerAPI
{
 
    public static IEndpointRouteBuilder MapStrategyRunnerEndpoints(this IEndpointRouteBuilder endpoints)
    {

        endpoints.MapPost("api/strategy/evaluate", async (StrategyRunnerService strategyRunnerService, ModelOrderLogDTO traderOrderLogDTO) => 
        {
            List<NewOrderDTO> strategyOrder = new List<NewOrderDTO>();
            strategyOrder = await strategyRunnerService.EvaluateStrategy(traderOrderLogDTO);
            return strategyOrder;
        }
        )
        .WithName("EvaluateStrategy")
        .WithOpenApi();

        return endpoints;   
    }


}
