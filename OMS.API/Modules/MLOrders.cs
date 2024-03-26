using OMS.Infrastructure.Interfaces;
using OMS.Infrastructure.Queue;
using OMS.SharedKernel.DTO;

public static class MLOrders
{

    public static IEndpointRouteBuilder MapMLOrdersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        
        endpoints.MapPost("api/mlorders/process-order", async (NewOrderDTO order, NewOrderChannelService newOrderChannelService) =>
        {
            int om_id = 0;
            await newOrderChannelService.WriteAsync(order);
            return Results.Ok(om_id);
        })
        .WithName("ProcessOrder")
        .WithOpenApi();


        endpoints.MapPost("api/mlorders/verify-model-trader", async (NewTraderDTO newTrader, IUserService userService) =>
        {
            int oid = await userService.VerifyAndAddByDisplayName(newTrader);
            return Results.Ok(oid);
        })
        .WithName("VerifyModelTrader")
        .WithOpenApi();

        return endpoints;
    }

}
