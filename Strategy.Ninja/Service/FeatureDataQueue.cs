using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Threading.Channels;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;




namespace Strategy.Ninja.Service;


public class FeatureDataDataQueue
{
    private readonly Channel<FeatureDataDTO> _channel;
    private ILogger<FeatureDataDataQueue> _logger;


    public FeatureDataDataQueue(ILogger<FeatureDataDataQueue> logger)
    {
        _logger = logger;

        // Create a bounded channel with a capacity limit to prevent out-of-memory issues in case of high load
        _channel = Channel.CreateBounded<FeatureDataDTO>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true, // Set to true if only one consumer will read from the channel
            SingleWriter = false  // Set to true if only one producer will write to the channel
        });
    }

    public async Task WriteAsync(FeatureDataDTO data, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(data, cancellationToken);
        await Task.CompletedTask;
    }

    public IAsyncEnumerable<FeatureDataDTO> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }

    
}
