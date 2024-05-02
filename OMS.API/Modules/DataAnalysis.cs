using OMS.Application.Models;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Interfaces;

using OMS.Infrastructure.Services;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace OMS.API;

public static class DataAnalysis
{

    public static IEndpointRouteBuilder MapDataAnalysisEndpoints(this IEndpointRouteBuilder endpoints)
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




        return endpoints;   
    }


}
