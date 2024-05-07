using System.Threading.Channels;

namespace OMS.SharedKernel.Common;

public class GenericMessageBus<T>
{
    readonly List<Channel<T>> _subscribers = new();

    public ChannelReader<T> Subscribe()
    {
        var channel = Channel.CreateUnbounded<T>();
        lock (_subscribers)
        {
            _subscribers.Add(channel);
        }

        return channel.Reader;
    }    

    public async Task PublishAsync(
        T integrationEvent,
        CancellationToken cancellationToken = default)
    {

        foreach (Channel<T> subscriber in _subscribers)
        {
                await subscriber.Writer.WriteAsync(integrationEvent, cancellationToken);
        }

    }

}
