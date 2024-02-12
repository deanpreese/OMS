using System.Threading.Channels;
using OMS.Core.Models;

namespace OMS.Services.Queue;

public class NewOrderChannelService 
{
    private readonly Channel<NewOrder> _channel;

    public NewOrderChannelService()
    {
        // Create a bounded channel with a capacity limit to prevent out-of-memory issues in case of high load
        _channel = Channel.CreateBounded<NewOrder>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true, // Set to true if only one consumer will read from the channel
            SingleWriter = false  // Set to true if only one producer will write to the channel
        });
    }

    public async Task WriteAsync(NewOrder order, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(order, cancellationToken);
    }

    public IAsyncEnumerable<NewOrder> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
