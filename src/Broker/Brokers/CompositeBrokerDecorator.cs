using Broker.Utils;

namespace Broker.Brokers;

public class CompositeBrokerDecorator : BrokerBase
{
    private readonly List<IBroker> _innerBrokers;

    public CompositeBrokerDecorator(string id, IEnumerable<IBroker> innerBrokers)
        : base(id)
    {
        _innerBrokers = innerBrokers.ToList();
    }

    public override async Task StartAsync(CancellationToken token)
    {
        await Task.WhenAll(_innerBrokers.Select(b => b.StartAsync(token)));
    }

    public override async Task HandleAsync(Stream payloadStream, CancellationToken token)
    {
        var payload = await payloadStream.ToByteArrayAsync();
        await Task.WhenAll(_innerBrokers.Select(async b =>
        {
            using var ms = new MemoryStream(payload);
            await b.HandleAsync(ms, token);
        }));
    }

    public override async Task StopAsync(CancellationToken token)
    {
        await Task.WhenAll(_innerBrokers.Select(b => b.StopAsync(token)));
    }
}