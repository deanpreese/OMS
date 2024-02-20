using System.Threading.Channels;
using OMS.Core.Models;

namespace Algo.Algorithms.Services;

public class AlgoOrderQueue
{
 private readonly Channel<OrderInfo> _channel;
 private readonly List<Channel<OrderInfo>> _subscribers = new List<Channel<OrderInfo>>();


    public AlgoOrderQueue()
    {
        // Create a bounded channel with a capacity limit to prevent out-of-memory issues in case of high load
        _channel = Channel.CreateBounded<OrderInfo>(new BoundedChannelOptions(5000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false, // Set to true if only one consumer will read from the channel
            SingleWriter = true  // Set to true if only one producer will write to the channel
        });
        Task.Run(async () => await Distribute());
    }

    public ChannelReader<OrderInfo> Subscribe()
    {
        var channel = Channel.CreateUnbounded<OrderInfo>();
        lock (_subscribers)
        {
            _subscribers.Add(channel);
        }
        return channel.Reader;
    }


    public async Task WriteAsync(OrderInfo orderInfo, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(orderInfo, cancellationToken);
    }

    public IAsyncEnumerable<OrderInfo> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }

    private async Task Distribute()
    {
        await foreach (var item in _channel.Reader.ReadAllAsync())
        {
            List<Channel<OrderInfo>> subscribersSnapshot;
            lock (_subscribers)
            {
                subscribersSnapshot = new List<Channel<OrderInfo>>(_subscribers);
            }

            foreach (var subscriber in subscribersSnapshot)
            {
                await subscriber.Writer.WriteAsync(item);
            }
        }
    }
}
