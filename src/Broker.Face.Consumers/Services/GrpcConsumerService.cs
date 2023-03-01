using System.Text;
using Broker.Brokers.Grpc;
using Grpc.Core;

namespace Broker.Face.Consumers.Services;

/// <summary>
/// This is test service for GrpcBroker
/// </summary>
public class GrpcConsumerService : BrokerEvent.BrokerEventBase
{
    private readonly ILogger<GrpcConsumerService> _logger;

    public GrpcConsumerService(ILogger<GrpcConsumerService> logger)
    {
        _logger = logger;
    }

    public override Task<BrokerEventReply> Send(BrokerEventRequest request, ServerCallContext context)
    {
        var body = Encoding.UTF8.GetString(request.Data.ToByteArray());
        _logger.LogInformation($"GrpcConsumer:BrokerEvent - Data received:{Environment.NewLine}{body}");
        return Task.FromResult(new BrokerEventReply());
    }
}