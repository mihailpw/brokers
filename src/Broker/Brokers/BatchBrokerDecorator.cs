namespace Broker.Brokers;

public class BatchBrokerDecorator : BrokerBase
{
    public class Options
    {
        public int BatchSize { get; init; }
    }

    private readonly Options _options;
    private readonly IBroker _innerBroker;
    private readonly MemoryStream _cache;
    private readonly SemaphoreSlim _cacheSemaphore = new(1);

    public BatchBrokerDecorator(string id, Options options, IBroker innerBroker)
        : base(id)
    {
        _options = options;
        _innerBroker = innerBroker;
        _cache = new MemoryStream();
    }

    public override async Task StartAsync(CancellationToken token)
    {
        await _innerBroker.StartAsync(token);
    }

    public override async Task HandleAsync(Stream payloadStream, CancellationToken token)
    {
        var clonedStream = await CloneCacheIfNeededAsync(payloadStream, token);
        if (clonedStream != null)
        {
            await _innerBroker.HandleAsync(clonedStream, token);
            await clonedStream.DisposeAsync();
        }
    }

    public override async Task StopAsync(CancellationToken token)
    {
        var clonedStream = await CloneCacheIfNeededAsync(null, token);
        if (clonedStream != null)
        {
            await _innerBroker.HandleAsync(clonedStream, token);
            await clonedStream.DisposeAsync();
        }
        
        await _innerBroker.StopAsync(token);
    }

    private async Task<MemoryStream?> CloneCacheIfNeededAsync(Stream? payloadStream, CancellationToken token)
    {
        await _cacheSemaphore.WaitAsync(token);
        try
        {
            if (payloadStream != null)
            {
                await payloadStream.CopyToAsync(_cache, token);
            }

            if ((payloadStream == null
                 || _cache.Length > _options.BatchSize)
                && _cache.Length > 0)
            {
                var clonedStream = new MemoryStream();
                _cache.Seek(0, SeekOrigin.Begin);
                await _cache.CopyToAsync(clonedStream, token);
                clonedStream.Seek(0, SeekOrigin.Begin);
                _cache.SetLength(0);
                return clonedStream;
            }
        }
        finally
        {
            _cacheSemaphore.Release();
        }
        
        return null;
    }
}