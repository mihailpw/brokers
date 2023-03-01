using Prometheus;

namespace Broker.CommonConfig;

public class BrokerMetrics : IBrokerMetrics
{
    private readonly Counter _attemptCount = Metrics.CreateCounter(
        "broker_handle_attempt_total",
        "Number of handling payloads.");
    private readonly Counter _successCount = Metrics.CreateCounter(
        "broker_handle_success_total",
        "Number of succeed handled payloads.");
    private readonly Counter _errorCount = Metrics.CreateCounter(
        "broker_handle_error_total",
        "Number of failed handled payloads.");
    private readonly Histogram _duration = Metrics.CreateHistogram(
        "broker_handle_duration_seconds",
        "Duration of handle operations.",
        new HistogramConfiguration
        {
            Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.1, count: 10)
        });
    
    public void HandleAttempt(string brokerId)
    {
        _attemptCount.Inc();
    }

    public void HandleSuccess(string brokerId)
    {
        _successCount.Inc();
    }

    public void HandleError(string brokerId)
    {
        _errorCount.Inc();
    }

    public Action HandleDuration(string brokerId)
    {
        var timer = _duration.NewTimer();
        return timer.Dispose;
    }
}