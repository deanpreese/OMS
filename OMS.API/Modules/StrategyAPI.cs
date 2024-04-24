using OMS.Application.Models;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Interfaces;
using OMS.Infrastructure.Queue;
using OMS.Infrastructure.Services;
using OMS.SharedKernel.DTO;

namespace OMS.API;

public static class StrategyAPI
{

    public static IEndpointRouteBuilder MapStrategyOrdersEndpoints(this IEndpointRouteBuilder endpoints)
    {

        endpoints.MapPost("api/strategyapi/add-new-strategy-trader", async (NewTraderDTO newTrader, IUserService userService) => 
        {
            int traderID = await userService.AddNewTrader(newTrader);
            return traderID;
        }
        )
        .WithName("AddNewStrategyTrader")
        .WithOpenApi();



        endpoints.MapPost("api/strategyapi/verify-add-by-displayName", async (NewTraderDTO newTrader, IUserService userService) =>
        {
            int traderID = await userService.VerifyAndAddByDisplayName(newTrader);
            return traderID;
        }
        )
        .WithName("AuthByDisplayName")
        .WithOpenApi();

  
        endpoints.MapPost("api/strategyapi/process-order", async (OrderManagerService orderManagerService, NewOrderDTO order ) =>
        {
            ModelOrderLog mol =  await orderManagerService.ProcessNewTraderOrder(order);
            return Results.Ok(mol.LiveOrderIDReference);
        })
        .WithName("ProcessStrategyOrder")
        .WithOpenApi();


        endpoints.MapGet("api/strategyapi/orders/live/{profileKey}", async (string profileKey, IDataService dataService ) =>
        {
            List<LiveOrderDTO> orders = await dataService.GetLiveOrdersByTrader(int.Parse(profileKey.Split('_')[0]), int.Parse(profileKey.Split('_')[1]));
            return orders;            
        }
        )
        .WithName("GetLiveOrdersByTrader")
        .WithOpenApi();



        endpoints.MapGet("api/strategyapi/trades/closed/last/{profileKey}/{platformId}", async (string profileKey, int platformId, IDataService dataService) =>
        {
            ClosedTradeDTO trade = await dataService.GetLastClosedTradeByOpenPlatformID(int.Parse(profileKey.Split('_')[0]), int.Parse(profileKey.Split('_')[1]), platformId);
            return trade;    
        }
        )
        .WithName("GetLastClosedTradeByOpenPlatformID")
        .WithOpenApi();



        endpoints.MapGet("api/strategyapi/scoreCard/{profileKey}", async (string profileKey, IAnalyticsService analyticsService) =>
        {
            ScoreCardDTO sc_dto = await analyticsService.GetTraderScoreCardDTO(int.Parse(profileKey.Split('_')[0]), int.Parse(profileKey.Split('_')[1]));
            return sc_dto;
        }    
        )
        .WithName("GetScoreCard")
        .WithOpenApi();




        return endpoints;   
    }


}
