using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OMS.Application.Models;
using OMS.Infrastructure.Interfaces;
using OMS.Infrastructure.Queue;
using OMS.Infrastructure.Services;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;


public static class MLOrders
{

    public static IEndpointRouteBuilder MapMLOrdersEndpoints(this IEndpointRouteBuilder endpoints)
    {

        endpoints.MapPost(PlatformConstants.ML_ORDER_URI, async (OrderManagerService orderManagerService, NewOrderDTO order ) =>
        {
            ModelOrderLog mol =  await orderManagerService.ProcessNewTraderOrder(order);

            var _httpClient = new HttpClient();
            var response = await _httpClient.PostAsJsonAsync(PlatformConstants.STRATEGY_RUNNER_BASE_URL + PlatformConstants.STRATEGY_RUNNER_EVALUATE, mol);
            response.EnsureSuccessStatusCode();
            var dto = await response.Content.ReadFromJsonAsync<NewOrderDTO>();

            return Results.Ok(mol.LiveOrderIDReference);
        })
        .WithName("ProcessOrderX")
        .WithOpenApi();
        

        
        endpoints.MapPost(PlatformConstants.ML_ORDER_URI_Z, async (OrderManagerService orderManagerService, NewOrderDTO order ) =>
        {
            ModelOrderLog mol =  await orderManagerService.ProcessNewTraderOrder(order);
            return Results.Ok(mol.LiveOrderIDReference);
        })
        .WithName("ProcessOrderZ")
        .WithOpenApi();

        return endpoints;
    }

}
