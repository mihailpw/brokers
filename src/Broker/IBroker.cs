namespace Broker;

public interface IBroker
{
    public string Id { get; }

    Task StartAsync(CancellationToken token);
    Task HandleAsync(Stream payloadStream, CancellationToken token);
    Task StopAsync(CancellationToken token);
}