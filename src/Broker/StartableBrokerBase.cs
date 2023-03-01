namespace Broker;

public abstract class StartableBrokerBase : IBroker
{
    private bool _isStarted = false;
    
    protected StartableBrokerBase(string id)
    {
        Id = id;
    }

    public string Id { get; }

    public async Task StartAsync(CancellationToken token)
    {
        if (_isStarted)
            return;

        await StartInternalAsync(token);
        _isStarted = true;
    }

    public async Task HandleAsync(Stream payloadStream, CancellationToken token)
    {
        if (!_isStarted)
        {
            throw new InvalidOperationException("Service is not started");
        }

        await HandleInternalAsync(payloadStream, token);
    }

    public async Task StopAsync(CancellationToken token)
    {
        if (!_isStarted)
        {
            return;
        }

        _isStarted = false;
        await StopInternalAsync(token);
    }

    protected abstract Task StartInternalAsync(CancellationToken token);
    protected abstract Task HandleInternalAsync(Stream payloadStream, CancellationToken token);
    protected abstract Task StopInternalAsync(CancellationToken token);
}