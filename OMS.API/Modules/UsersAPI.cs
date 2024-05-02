using OMS.Application.Models;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Interfaces;

using OMS.Infrastructure.Services;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace OMS.API;

public static class UsersAPI
{

    public static IEndpointRouteBuilder MapUsersAPIEndpoints(this IEndpointRouteBuilder endpoints)
    {


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
