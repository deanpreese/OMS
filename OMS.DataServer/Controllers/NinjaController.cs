using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OMS.Core.Models;
using OMS.Infrastructure.Data;

namespace OMS.DataServer;

[ApiController]
[Route("api/data")]
public class NinjaController : ControllerBase
{
    private ILogger<NinjaController> _logger;
    private OrderManagementDbContext _context;
    private DataQueue _dataQueue;


    public NinjaController(ILogger<NinjaController> logger, OrderManagementDbContext context, DataQueue dataQueue)
    {
        _logger = logger;
        _context = context;
        _dataQueue = dataQueue;
    }


    [HttpPost("add-feature-data")]
    public async Task<string> AddFeatureData([FromBody] FeatureData featureData)
    {
        await _dataQueue.WriteAsync(featureData);

        return await Task.FromResult(featureData.TimeTicks.ToString());
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
