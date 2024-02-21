using System.Threading.Channels;
using OMS.Core.Models;

namespace Algo.Algorithms.Services;

public class AlgoOrderQueue
{
 private readonly Channel<LiveOrder> _channel;
 private readonly List<Channel<LiveOrder>> _subscribers = new List<Channel<LiveOrder>>();


    public AlgoOrderQueue()
    {
        // Create a bounded channel with a capacity limit to prevent out-of-memory issues in case of high load
        _channel = Channel.CreateBounded<LiveOrder>(new BoundedChannelOptions(5000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false, // Set to true if only one consumer will read from the channel
            SingleWriter = true  // Set to true if only one producer will write to the channel
        });
        Task.Run(async () => await Distribute());
    }

    public ChannelReader<LiveOrder> Subscribe()
    {
        var channel = Channel.CreateUnbounded<LiveOrder>();
        lock (_subscribers)
        {
            _subscribers.Add(channel);
        }
        return channel.Reader;
    }


    public async Task WriteAsync(LiveOrder orderInfo, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(orderInfo, cancellationToken);
    }

    public IAsyncEnumerable<LiveOrder> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }

    private async Task Distribute()
    {
        await foreach (var item in _channel.Reader.ReadAllAsync())
        {
            List<Channel<LiveOrder>> subscribersSnapshot;
            lock (_subscribers)
            {
                subscribersSnapshot = new List<Channel<LiveOrder>>(_subscribers);
            }

            foreach (var subscriber in subscribersSnapshot)
            {
                await subscriber.Writer.WriteAsync(item);
            }
        }
    }
}
