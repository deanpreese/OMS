using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.SignalR;

using OMS.Core.Models;
using OMS.Services.Queue;
using OMS.Relay.SignalHub;


namespace OMS.MT4Relay.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly NewOrderQueue _newOrderQueue;
        private ILogger<OrderController> _logger;
        
        public OrderController(ILogger<OrderController> logger, NewOrderQueue newOrderQueue)
        {
            _newOrderQueue = newOrderQueue;
            _logger = logger;
        }

        [HttpPost("process-order")]
        public async Task<int> ProcessOrder([FromBody] NewOrder newOrder)
        {
            int om_id = 87654321;
            if (newOrder == null)
            {
                om_id = 0;
                return  om_id;
            }

            try
            {
                om_id = await _newOrderQueue.WriteAsync(newOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                om_id = 0;
            }
            return om_id;
        }
    }
}
