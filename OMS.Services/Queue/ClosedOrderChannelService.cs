using System.Threading.Channels;
using OMS.Core.Models;

namespace OMS.Services.Queue;

public class ClosedOrderChannelService
{
 private readonly Channel<UserInfo> _channel;

    public ClosedOrderChannelService()
    {
        // Create a bounded channel with a capacity limit to prevent out-of-memory issues in case of high load
        _channel = Channel.CreateBounded<UserInfo>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true, // Set to true if only one consumer will read from the channel
            SingleWriter = false  // Set to true if only one producer will write to the channel
        });
    }

    public async Task WriteAsync(UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(userInfo, cancellationToken);
    }

    public IAsyncEnumerable<UserInfo> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
