namespace Broker;

public abstract class BrokerBase : IBroker
{
    protected BrokerBase(string id)
    {
        Id = id;
    }

    public string Id { get; }

    public virtual Task StartAsync(CancellationToken token) => Task.CompletedTask;

    public abstract Task HandleAsync(Stream payloadStream, CancellationToken token);

    public virtual Task StopAsync(CancellationToken token) => Task.CompletedTask;
}