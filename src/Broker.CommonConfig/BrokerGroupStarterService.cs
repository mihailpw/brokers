using Microsoft.Extensions.Hosting;

namespace Broker.CommonConfig;

public class BrokerGroupStarterService : IHostedService
{
    private readonly IBrokerGroup _brokerGroup;

    public BrokerGroupStarterService(IBrokerGroup brokerGroup)
    {
        _brokerGroup = brokerGroup;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var tcs = new CancellationTokenSource();
        tcs.CancelAfter(10000);
        await _brokerGroup.StartAsync(tcs.Token);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _brokerGroup.StopAsync(cancellationToken);
    }
}