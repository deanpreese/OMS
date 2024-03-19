using Microsoft.AspNetCore.Mvc;
using OMS.Core.Common;
using OMS.Core;
using OMS.Core.Models;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Services.Queue;
using OMS.Infrastructure.Services.Trading;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using OMS.Infrastructure.Interfaces;


namespace OMS.API;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{

    private ILogger<DataController> _logger;
    private OrderManagementDbContext _context;
    private FeatureDataDataQueue _featureDataQueue;

    public DataController(ITradingService tradingService,
        ILogger<DataController> logger, OrderManagementDbContext context, 
         FeatureDataDataQueue featureDataDataQueue)
    {
        _logger = logger;
        _context = context;
        _featureDataQueue = featureDataDataQueue;
        
    }


    [HttpPost("add-tick")]
    public async Task<int> AddTick([FromBody] LastTickDTO lastTick)
    {
        return await Task.FromResult(0);
        
    }

    [HttpPost("add-feature-data")]
    public async Task<string> AddFeatureData([FromBody] FeatureDataDTO featureData)
    {
        await _featureDataQueue.WriteAsync(featureData);

        return await Task.FromResult(featureData.TimeTicks.ToString());
    }

}
