using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using OMS.SharedKernel.DTO;
using Strategy.Server.StrategyServices;
using Strategy.SharedKernel;

namespace OMS.API;

public static class StrategyAPI
{
 
    public static IEndpointRouteBuilder MapStrategyServerEndpoints(this IEndpointRouteBuilder endpoints)
    {

        endpoints.MapPost("api/strategu/evaluate", async (StrategyService strategyService, ModelOrderLogDTO traderOrderLogDTO) => 
        {
            NewOrderDTO  strategyOrder = await strategyService.EvaluateStrategy(traderOrderLogDTO);
            return strategyOrder;
        }
        )
        .WithName("EvaluateStrategy")
        .WithOpenApi();

        return endpoints;   
    }


}
