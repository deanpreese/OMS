using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;


using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using OMS.SharedKernel.Grains;

namespace Strategy.Trader.Services;

public class IncomingOrderQueue
{
    private readonly Channel<LiveOrderDTO> _channel;
    private readonly List<Channel<LiveOrderDTO>> _subscribers = new List<Channel<LiveOrderDTO>>();

    BufferBlock<LiveOrderDTO> flowBuffer;

    public IncomingOrderQueue()
    {
        flowBuffer = new BufferBlock<LiveOrderDTO>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded });

        // Create a bounded channel with a capacity limit to prevent out-of-memory issues in case of high load
        _channel = Channel.CreateBounded<LiveOrderDTO>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true, // Set to true if only one consumer will read from the channel
            SingleWriter = true  // Set to true if only one producer will write to the channel
        });


        //flowBuffer = new BufferBlock<LiveOrderDTO>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded });
        Task.Run(async () => await OrderCapture());
        Task.Run(async () => await OrderBroadcast());

        //Task.Run(async () => await Distribute());
    }

    public ChannelReader<LiveOrderDTO> Subscribe()
    {
        var channel = Channel.CreateUnbounded<LiveOrderDTO>();
        lock (_subscribers)
        {
            _subscribers.Add(channel);
        }

        return channel.Reader;
    }


    public async Task WriteAsync(LiveOrderDTO orderInfo, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(orderInfo, cancellationToken);
    }

    public IAsyncEnumerable<LiveOrderDTO> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }


    private async Task OrderCapture()
    {
        await Task.Run(async () =>
        {
            await foreach (LiveOrderDTO item in _channel.Reader.ReadAllAsync())
            {
                await flowBuffer.SendAsync(item); 
            }
        });
        await Task.CompletedTask;
    }

    private async Task OrderBroadcast()
    {
        while (await flowBuffer.OutputAvailableAsync())
        {
            while (flowBuffer.TryReceive(out LiveOrderDTO newLiveOrder))
            {
            //LiveOrderDTO newLiveOrder = flowBuffer.Receive();
                foreach (Channel<LiveOrderDTO> subscriber in _subscribers)
                {
                    LiveOrderDTO order = newLiveOrder;
                    await subscriber.Writer.WriteAsync(order);
                    await Task.Delay(12);    
                }
            }
        }
    }

}


/*

                await Task.Delay(5); 
                LiveOrderDTO order = newLiveOrder;
                await subscriber.Writer.WriteAsync(order);
                await Task.Delay(10);    

*/