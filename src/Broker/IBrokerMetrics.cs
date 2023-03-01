namespace Broker;

public interface IBrokerMetrics
{
    void HandleAttempt(string brokerId);
    void HandleSuccess(string brokerId);
    void HandleError(string brokerId);
    Action HandleDuration(string brokerId);
}