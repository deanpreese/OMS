using OMS.Application.Models;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Interfaces;
using OMS.Infrastructure.Queue;
using OMS.Infrastructure.Services;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace OMS.API;

public static class StrategyOrders
{

    public static IEndpointRouteBuilder MapStrategyOrdersEndpoints(this IEndpointRouteBuilder endpoints)
    {
  
        endpoints.MapPost(PlatformConstants.STRATEGY_ORDER_URI, async (OrderManagerService orderManagerService, NewOrderDTO order ) =>
        {
            ModelOrderLog mol =  await orderManagerService.ProcessNewTraderOrder(order);
            return Results.Ok(mol.LiveOrderIDReference);
        })
        .WithName("ProcessStrategyOrder")
        .WithOpenApi();

        return endpoints;   
    }


}
