using Microsoft.AspNetCore.Mvc;
using OMS.Core.Common;
using OMS.Core;
using OMS.Data;
using OMS.Core.Models;
using OMS.Services.Trading;
using OMS.Services.Queue;
using OMS.Core.Interfaces;
using OMS.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace OMS.Web;

[ApiController]
[Route("api/order")]
public class OMSController : ControllerBase
{

    private ILogger<OMSController> _logger;
    private OrderManagementDbContext _context;
    private ITradingService _trader_service;
    private IUnitOfWork _unitOfWork;
    private readonly NewOrderChannelService _newOrderChannelService;


    public OMSController(ITradingService tradingService,
        ILogger<OMSController> logger, OrderManagementDbContext context, 
        IUnitOfWork unitOfWork, NewOrderChannelService newOrderChannelService )
    {

        _logger = logger;
        _trader_service = tradingService;
        _context = context;
        _unitOfWork = unitOfWork;
        _newOrderChannelService = newOrderChannelService;
        
    }

    [HttpPost("add-new-trader")]
    public async Task<IActionResult> AddNewTrader([FromBody] NewTrader newTrader)
    {
        int oid = await _trader_service.AddTraderAsync(newTrader);
        return Ok(oid);
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> AuthenticateTrader([FromBody] UserInfo userInfo)
    {

        //var traderGrain = _grainFactory.GetGrain<IAuthenticated>(userInfo.UserID);
        //var isAuthenticated = await traderGrain.AuthenticateTrader(userInfo);
        //return Ok(isAuthenticated);

        int auth_code = await _trader_service.AuthenticateTraderAsync(userInfo);
        return Ok(auth_code);
    }


    [HttpPost("process-order")]
    public async Task<ActionResult> ProcessOrder([FromBody] NewOrder order)
    {
        int om_id = 0;
        try
        {
            await _newOrderChannelService.WriteAsync(order);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine(ex.Message);
        }
        return Ok(om_id);
    }


    //Comment
    [HttpPost("process-MT-order")]
    public async Task<ActionResult> ProcessMTOrder([FromBody] NewOrder order)
    {

        UserInfo userInfo = new UserInfo
        {
            UserID = order.UserID,
            Password = "abc",
            GroupNumber = order.UserGroup
        };

        int auth_code = await _trader_service.AuthenticateTraderAsync(userInfo);
        if( auth_code == 0 )
        {
            // Need to add trader
            NewTrader newTrader = new NewTrader
            {
                Group = order.UserGroup,
                UserId = 0,
                DisplayName = order.UserName,
                UserPwd = "abc",
                FirstName = "MetaTrader",
                LastName = "EA",
                Email = "abc.xyz"

            };

            int trader_id = await _trader_service.AddTraderAsync(newTrader);

             UserInfo newUserInfo = new UserInfo
            {
                UserID = trader_id,
                Password = "abc",
                GroupNumber = order.UserGroup
            };
            auth_code = await _trader_service.AuthenticateTraderAsync(newUserInfo);
        }

        int om_id = 0;
        if (auth_code != 0)
        {
            try
            {
                await _newOrderChannelService.WriteAsync(order);
                om_id = 87654321;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine(ex.Message);
            }
        }
        return Ok(om_id);
    }


}
