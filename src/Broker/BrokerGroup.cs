using Microsoft.Extensions.Logging;

namespace Broker;

public class BrokerGroup : IBrokerGroup
{
    private readonly ILogger<BrokerGroup> _logger;
    private readonly IBrokerMetrics _brokerMetrics;
    private readonly Dictionary<string, IBroker> _brokers;
    private bool _isWorking;

    public BrokerGroup(ILogger<BrokerGroup> logger, IBrokerMetrics brokerMetrics, IEnumerable<IBroker> brokers)
    {
        _logger = logger;
        _brokerMetrics = brokerMetrics;
        _brokers = brokers.ToDictionary(b => b.Id);
    }

    public bool HasBroker(string brokerId)
    {
        return _brokers.ContainsKey(brokerId);
    }

    public async Task StartAsync(CancellationToken token)
    {
        await Task.WhenAll(_brokers.Values.Select(b => b.StartAsync(token)));
        _isWorking = true;
    }

    public async Task HandleAsync(string brokerId, Stream payloadStream, CancellationToken token)
    {
        if (!_brokers.TryGetValue(brokerId, out var broker))
        {
            _logger.LogWarning("Unknown broker id ({BrokerId})", brokerId);
            return;
        }

        if (!_isWorking)
        {
            throw new InvalidOperationException("Brokers are not started.");
        }

        _brokerMetrics.HandleAttempt(brokerId);
        var finishAction = _brokerMetrics.HandleDuration(brokerId);
        try
        {
            await broker.HandleAsync(payloadStream, token);
            _brokerMetrics.HandleSuccess(brokerId);
        }
        catch (Exception ex)
        {
            _brokerMetrics.HandleError(brokerId);
            _logger.LogError(ex, "Broker '{BrokerId}' fail", brokerId);
            throw;
        }
        finally
        {
            finishAction();
        }
    }

    public async Task StopAsync(CancellationToken token)
    {
        _isWorking = false;
        await Task.WhenAll(_brokers.Values.Select(b => b.StopAsync(token)));
    }
}