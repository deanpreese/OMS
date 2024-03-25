using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using Strategy.Server.Models;

namespace Strategy.Server.Services;

public class IncomingOrderQueue
{
    private readonly Channel<ModelOrderLogDTO> _channel;
    private readonly List<Channel<ModelOrderLogDTO>> _subscribers = new List<Channel<ModelOrderLogDTO>>();

    BufferBlock<ModelOrderLogDTO> flowBuffer;

    public IncomingOrderQueue()
    {
        flowBuffer = new BufferBlock<ModelOrderLogDTO>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded });

        // Create a bounded channel with a capacity limit to prevent out-of-memory issues in case of high load
        _channel = Channel.CreateBounded<ModelOrderLogDTO>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true, // Set to true if only one consumer will read from the channel
            SingleWriter = true  // Set to true if only one producer will write to the channel
        });

        // OrderCapture and OrderBroadcast will run in parallel using flowbuffer
        Task.Run(async () => await OrderCapture());
        Task.Run(async () => await OrderBroadcast());

        // Distribute will is a straight through process
        //Task.Run(async () => await Distribute());

    }

    public ChannelReader<ModelOrderLogDTO> Subscribe()
    {
        var channel = Channel.CreateUnbounded<ModelOrderLogDTO>();
        lock (_subscribers)
        {
            _subscribers.Add(channel);
        }

        return channel.Reader;
    }


    public async Task WriteAsync(ModelOrderLogDTO orderInfo, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(orderInfo, cancellationToken);
    }

    public IAsyncEnumerable<ModelOrderLogDTO> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }


    private async Task Distribute()
    {
        await Task.Run(async () =>
        {
            await foreach (ModelOrderLogDTO newLiveOrder in _channel.Reader.ReadAllAsync())
            {
                foreach (Channel<ModelOrderLogDTO> subscriber in _subscribers)
                {
                    ModelOrderLogDTO order = newLiveOrder;
                    await subscriber.Writer.WriteAsync(order);
                    //await Task.Delay(10);    
                }
            }
        });
        await Task.CompletedTask;
    }




    private async Task OrderCapture()
    {
        await Task.Run(async () =>
        {
            await foreach (ModelOrderLogDTO item in _channel.Reader.ReadAllAsync())
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
            //await Task.Delay(5);       
            ModelOrderLogDTO newLiveOrder = flowBuffer.Receive();
            List<Channel<ModelOrderLogDTO>> subscribersSnapshot;

            Console.WriteLine(newLiveOrder.UserID + " " + newLiveOrder.LiveOrderIDReference+ " " + newLiveOrder.OrderAction + " " + newLiveOrder.OrderType);

            /*
            lock (_subscribers)
            {
                subscribersSnapshot = new List<Channel<LiveOrderDTO>>(_subscribers);
            }
            
            foreach (Channel<LiveOrderDTO> subscriber in subscribersSnapshot)
            {
                LiveOrderDTO order = newLiveOrder;
                await subscriber.Writer.WriteAsync(order);
                //await Task.Delay(10);    
            }
            */
        }
    }




}

