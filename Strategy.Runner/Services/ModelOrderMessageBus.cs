using System.Threading.Channels;
using Strategy.SharedKernel;

namespace Strategy.Runner.Services;

public class ModelOrderMessageBus
{
    List<Channel<ModelOrderLogDTO>> _subscribers = new List<Channel<ModelOrderLogDTO>>();

    public ChannelReader<ModelOrderLogDTO> Subscribe()
    {
        var channel = Channel.CreateUnbounded<ModelOrderLogDTO>();
        lock (_subscribers)
        {
            _subscribers.Add(channel);
        }

        return channel.Reader;
    }    

    public async Task PublishAsync<T>(
        T integrationEvent,
        CancellationToken cancellationToken = default)
        where T :  ModelOrderLogDTO
    {

        foreach (Channel<ModelOrderLogDTO> subscriber in _subscribers)
        {
                await subscriber.Writer.WriteAsync(integrationEvent, cancellationToken);
        }

    }

}
