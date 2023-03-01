namespace Broker.Benchmarks.Stubs;

public class NullBrokerMetrics : IBrokerMetrics
{
    private readonly Action _emptyAction = () => { };

    public void HandleAttempt(string brokerId)
    {
    }

    public void HandleSuccess(string brokerId)
    {
    }

    public void HandleError(string brokerId)
    {
    }

    public Action HandleDuration(string brokerId)
    {
        return _emptyAction;
    }
}