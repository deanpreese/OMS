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
[Route("api/data")]
public class DataController : ControllerBase
{

    private ILogger<DataController> _logger;
    private OrderManagementDbContext _context;
    private ITradingService _trader_service;
    private readonly NewOrderChannelService _newOrderChannelService;


    public DataController(ITradingService tradingService,
        ILogger<DataController> logger, OrderManagementDbContext context, 
         NewOrderChannelService newOrderChannelService )
    {
        _logger = logger;
        _trader_service = tradingService;
        _context = context;
        _newOrderChannelService = newOrderChannelService;
        
    }


    [HttpPost("add-tick")]
    public async Task<int> AddTick([FromBody] LastTick lastTick)
    {
        return await Task.FromResult(0);
        
    }

    [HttpPost("add-feature-data")]
    public async Task<int> AddFeatureData([FromBody] FeatureData featureData)
    {
        return await Task.FromResult(0);
    }


    [HttpPost("get-live-orders")]
    public async Task<List<LiveOrder>> GetLiveOrders([FromBody] UserInfo user)
    {
  
        return await Task.FromResult(new List<LiveOrder>());
        
    }
    
    [HttpPost("get-traders")]
    public async Task<List<UserProfile>> GetTraders([FromBody] int userGroup)
    {
        var users = ((from u in _context.UserProfiles
                                where u.GroupID == userGroup
                                select u).Take(50)).ToList();
        
        return await Task.FromResult(users);
    }


    [HttpPost("get-closed-trades")]
    public async Task<List<ClosedTrade>> GetClosedTrades([FromBody] UserInfo userInfo)
    {
        return await Task.FromResult(new List<ClosedTrade>());
        
    }


}
