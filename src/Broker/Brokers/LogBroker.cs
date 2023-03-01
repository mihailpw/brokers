using Microsoft.Extensions.Logging;

namespace Broker.Brokers;

public class LogBroker : BrokerBase
{
    private readonly ILogger<LogBroker> _logger;

    public LogBroker(string id, ILogger<LogBroker> logger)
        : base(id)
    {
        _logger = logger;
    }

    public override async Task HandleAsync(Stream payloadStream, CancellationToken token)
    {
        using var streamReader = new StreamReader(payloadStream);
        var data = await streamReader.ReadToEndAsync();
        _logger.LogInformation(data);
    }
}