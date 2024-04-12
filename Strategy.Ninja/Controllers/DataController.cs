using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OMS.SharedKernel.DTO;
using Strategy.Ninja.Service;

namespace Strategy.Ninja.Controllers;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{

    private ILogger<DataController> _logger;

    private FeatureDataDataQueue _featureDataQueue;

    public DataController(
        ILogger<DataController> logger, 
         FeatureDataDataQueue featureDataDataQueue)
    {
        _logger = logger;
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
