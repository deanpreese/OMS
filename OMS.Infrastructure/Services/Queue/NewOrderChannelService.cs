using System.Threading.Channels;
using OMS.Core.Models;
using OMS.Core.DTO;


namespace OMS.Infrastructure.Services.Queue;


public class NewOrderChannelService 
{
    private readonly Channel<NewOrderDTO> _channel;

    public NewOrderChannelService()
    {
        // Create a bounded channel with a capacity limit to prevent out-of-memory issues in case of high load
        _channel = Channel.CreateBounded<NewOrderDTO>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true, // Set to true if only one consumer will read from the channel
            SingleWriter = false  // Set to true if only one producer will write to the channel
        });
    }

    public async Task WriteAsync(NewOrderDTO order, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(order, cancellationToken);
    }

    public IAsyncEnumerable<NewOrderDTO> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
