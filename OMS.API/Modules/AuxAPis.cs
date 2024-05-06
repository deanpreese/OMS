using OMS.Application.Models;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Interfaces;

using OMS.Infrastructure.Services;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace OMS.API;

public static class AuxAPIs
{

    public static IEndpointRouteBuilder MapAuxEndpoints(this IEndpointRouteBuilder endpoints)
    {

        endpoints.MapGet(PlatformConstants.GET_LIVE_ORDERS_BY_TRADER, async (string profileKey, IDataService dataService ) =>
        {
            List<LiveOrderDTO> orders = await dataService.GetLiveOrdersByTrader(int.Parse(profileKey.Split('_')[0]), int.Parse(profileKey.Split('_')[1]));
            return orders;            
        }
        )
        .WithName("GetLiveOrdersByTrader")
        .WithOpenApi();



        endpoints.MapGet(PlatformConstants.GET_LAST_CLOSED_TRADE_BY_OPEN_PLATFORM_ID, async (string profileKey, int platformId, IDataService dataService) =>
        {
            ClosedTradeDTO trade = await dataService.GetLastClosedTradeByOpenPlatformID(int.Parse(profileKey.Split('_')[0]), int.Parse(profileKey.Split('_')[1]), platformId);
            return trade;    
        }
        )
        .WithName("GetLastClosedTradeByOpenPlatformID")
        .WithOpenApi();



        endpoints.MapGet(PlatformConstants.GET_SCORE_CARD, async (string profileKey, IAnalyticsService analyticsService) =>
        {
            ScoreCardDTO sc_dto = await analyticsService.GetTraderScoreCardDTO(int.Parse(profileKey.Split('_')[0]), int.Parse(profileKey.Split('_')[1]));
            return sc_dto;
        }    
        )
        .WithName("GetScoreCard")
        .WithOpenApi();



        endpoints.MapPost(PlatformConstants.ADD_TRADER_URI, async (NewTraderDTO newTrader, IUserService userService) => 
        {
            int traderID = await userService.AddNewTrader(newTrader);
            return traderID;
        }
        )
        .WithName("AddNewStrategyTrader")
        .WithOpenApi();



        endpoints.MapPost(PlatformConstants.VERIFY_AND_ADD_BY_DISPLAY_NAME_URI, async (NewTraderDTO newTrader, IUserService userService) =>
        {
            int traderID = await userService.VerifyAndAddByDisplayName(newTrader);
            return traderID;
        }
        )
        .WithName("AuthByDisplayName")
        .WithOpenApi();


        endpoints.MapPost(PlatformConstants.VERIFY_MODEL_TRADER_URI, async (NewTraderDTO newTrader, IUserService userService) =>
        {
            int oid = await userService.VerifyAndAddByDisplayName(newTrader);
            return Results.Ok(oid);
        })
        .WithName("VerifyModelTrader")
        .WithOpenApi();




        return endpoints;   
    }


}
