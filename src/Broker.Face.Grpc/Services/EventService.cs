using Grpc.Core;

namespace Broker.Face.Grpc.Services;

public class EventService : Event.EventBase
{
    private readonly IBrokerGroup _brokerGroup;

    public EventService(IBrokerGroup brokerGroup)
    {
        _brokerGroup = brokerGroup;
    }

    public override async Task<EventReply> Send(EventRequest request, ServerCallContext context)
    {
        if (!_brokerGroup.HasBroker(request.Broker))
            return new EventReply();

        using var requestMemoryStream = new MemoryStream(request.Payload.ToByteArray());
        requestMemoryStream.Seek(0, SeekOrigin.Begin);
        await _brokerGroup.HandleAsync(request.Broker, requestMemoryStream, context.CancellationToken);
        return new EventReply();
    }
}