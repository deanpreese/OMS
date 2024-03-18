using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using OMS.Core.Models;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;


namespace Strategy.Trader;

public class NewOrderQueue
{
    private readonly Channel<NewOrderDTO> _channel;
    private ILogger<NewOrderQueue> _logger;
    //private readonly BlockingCollection<NewOrder> _LiveOrderCollection = new BlockingCollection<NewOrder>(2000);
    //private readonly BlockingCollection<ClosedTrade> _ClosedOrderCollection = new BlockingCollection<ClosedTrade>(2000);

    private  List<NewOrderDTO> _liveOrderCollection = new List<NewOrderDTO>();
    private  List<ClosedTrade> _closedOrderCollection = new List<ClosedTrade>();
    private readonly object _lock = new object();

public NewOrderQueue(ILogger<NewOrderQueue> logger)
    {
        _logger = logger;

        // Create a bounded channel with a capacity limit to prevent out-of-memory issues in case of high load
        _channel = Channel.CreateBounded<NewOrderDTO>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true, // Set to true if only one consumer will read from the channel
            SingleWriter = false  // Set to true if only one producer will write to the channel
        });
    }

    public async Task<int> WriteAsync(NewOrderDTO order, CancellationToken cancellationToken = default)
    {
        int om_id = 0;
        order.PlatformOrderID = om_id;


        await _channel.Writer.WriteAsync(order, cancellationToken);

        return om_id;
    }

    public IAsyncEnumerable<NewOrderDTO> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }

    public async Task AddToNewOrdersList(NewOrderDTO order)
    {
        ArgumentNullException.ThrowIfNull(order);
        lock (_lock)
        {
            _liveOrderCollection.Add(order);
        }
        await Task.CompletedTask;
    }

    public Task RemoveFromNewOrdersList(int platformOrderID)
    {
        lock (_lock)
        {
            var ordersToRemove = _liveOrderCollection
                .Where(distinct_order => distinct_order.PlatformOrderID == platformOrderID)
                .ToList();

            foreach (var item in ordersToRemove)
            {
                _liveOrderCollection.Remove(item);
            }
        }

        return Task.CompletedTask;
    }

    public async Task AddToClosedOrdersList(ClosedTrade closedTrade)
    {
        ArgumentNullException.ThrowIfNull(closedTrade);
        lock (_lock)
        {
            _closedOrderCollection.Add(closedTrade);
        }
        await Task.CompletedTask;
    }

    public Task<List<ClosedTrade>> GetClosedOrdersList()
    {
        lock (_lock)
        {
            return Task.FromResult(_closedOrderCollection);
        }
    }

    public Task<List<NewOrderDTO>> GetNewOrdersList()
    {
        lock (_lock)
        {
             return Task.FromResult(_liveOrderCollection);
        }
    }

    public Task<List<NewOrderDTO>> GetOrdersList(NewOrderDTO order)
    {
        lock (_lock)
        {
            var filteredOrders = _liveOrderCollection
                .Where(distinct_order => distinct_order.UserName == order.UserName
                                        && distinct_order.GroupID == order.GroupID
                                        && distinct_order.Instrument == order.Instrument
                                        && distinct_order.OrderAction != order.OrderAction)
                .ToList();

            return Task.FromResult(filteredOrders);
        }
    }
    
}
