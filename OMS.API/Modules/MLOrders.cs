using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OMS.Application.Models;
using OMS.Infrastructure.Interfaces;
using OMS.Infrastructure.Queue;
using OMS.Infrastructure.Services;
using OMS.SharedKernel.DTO;

public static class MLOrders
{

    public static IEndpointRouteBuilder MapMLOrdersEndpoints(this IEndpointRouteBuilder endpoints)
    {

        
        endpoints.MapPost("api/ml/process-order", async (OrderManagerService orderManagerService, NewOrderDTO order ) =>
        {
            ModelOrderLog mol =  await orderManagerService.ProcessNewTraderOrder(order);
            return Results.Ok(mol.LiveOrderIDReference);
        })
        .WithName("ProcessOrderX")
        .WithOpenApi();
        

        endpoints.MapPost("api/ml/process-orderx", async (NewOrderDTO order, NewOrderChannelService newOrderChannelService) =>
        {
            int om_id = 987654321;
            await newOrderChannelService.WriteAsync(order);
            return Results.Ok(om_id);
        })
        .WithName("ProcessOrder")
        .WithOpenApi();


        endpoints.MapPost("api/ml/verify-model-trader", async (NewTraderDTO newTrader, IUserService userService) =>
        {
            int oid = await userService.VerifyAndAddByDisplayName(newTrader);
            return Results.Ok(oid);
        })
        .WithName("VerifyModelTrader")
        .WithOpenApi();

        return endpoints;
    }

}
