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

        string baseUrl = "http://10.0.0.147:9999";
        string uri = "/api/strategy/evaluate";
        
        endpoints.MapPost("api/ml/process-order", async (OrderManagerService orderManagerService, NewOrderDTO order ) =>
        {
            ModelOrderLog mol =  await orderManagerService.ProcessNewTraderOrder(order);

            var _httpClient = new HttpClient();
            var response = await _httpClient.PostAsJsonAsync(baseUrl + uri, mol);
            response.EnsureSuccessStatusCode();
            var dto = await response.Content.ReadFromJsonAsync<NewOrderDTO>();

            return Results.Ok(mol.LiveOrderIDReference);
        })
        .WithName("ProcessOrderX")
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
