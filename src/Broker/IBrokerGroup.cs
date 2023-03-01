namespace Broker;

public interface IBrokerGroup
{
    bool HasBroker(string brokerId);

    Task StartAsync(CancellationToken token);
    Task HandleAsync(string brokerId, Stream payloadStream, CancellationToken token);
    Task StopAsync(CancellationToken token);
}