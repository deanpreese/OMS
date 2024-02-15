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
[Route("api/ml")]
public class MLController : ControllerBase
{

    private ILogger<MLController> _logger;
    private OrderManagementDbContext _context;
    private ITradingService _trader_service;
    private readonly NewOrderChannelService _newOrderChannelService;
    private IUserService _user_service;


    public MLController(ITradingService tradingService,
        ILogger<MLController> logger, OrderManagementDbContext context, 
         NewOrderChannelService newOrderChannelService, IUserService userService)
    {
        _logger = logger;
        _trader_service = tradingService;
        _context = context;
        _newOrderChannelService = newOrderChannelService;
        _user_service = userService;
    }


    [HttpPost("process-order")]
    public async Task<ActionResult> ProcessOrder([FromBody] NewOrder order)
    {
        Console.WriteLine("ML Order Info: " + order.UserID + "  " + order.UserGroup + "  " + order.OrderPX + "  "  + order.OrderAction);
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

    [HttpPost("verify-model-trader")]
    public async Task<IActionResult> VerifyModelTrader([FromBody] NewTrader newTrader)
    {
        int oid = await _user_service.VerifyByDisplayName(newTrader);
        return Ok(oid);
    }



}
