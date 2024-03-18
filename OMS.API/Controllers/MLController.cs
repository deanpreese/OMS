using Microsoft.AspNetCore.Mvc;
using OMS.Core.Models;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Services.Queue;
using OMS.Infrastructure.Services.Trading;


namespace OMS.API;

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
        //Console.WriteLine("ML Order Info: " + order.UserID + "  " + order.GroupID + "  " + order.OrderPX + "  "  + order.OrderAction);
        int om_id = 0;
        try
        {
            await _newOrderChannelService.WriteAsync(order);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return Ok(om_id);
    }

    [HttpPost("verify-model-trader")]
    public async Task<IActionResult> VerifyModelTrader([FromBody] NewTrader newTrader)
    {
        int oid = await _user_service.VerifyAndAddByDisplayName(newTrader);
        return Ok(oid);
    }



}
