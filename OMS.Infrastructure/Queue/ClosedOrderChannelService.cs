using System.Threading.Channels;
using OMS.Core;
using OMS.Core.Models;

namespace OMS.Infrastructure.Queue;

public class ClosedOrderChannelService
{
 private readonly Channel<LogDataDTO> _channel;

    public ClosedOrderChannelService()
    {
        // Create a bounded channel with a capacity limit to prevent out-of-memory issues in case of high load
        _channel = Channel.CreateBounded<LogDataDTO>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true, // Set to true if only one consumer will read from the channel
            SingleWriter = false  // Set to true if only one producer will write to the channel
        });
    }

    public async Task WriteAsync(LogDataDTO logDataDTO, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(logDataDTO, cancellationToken);
    }

    public IAsyncEnumerable<LogDataDTO> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
