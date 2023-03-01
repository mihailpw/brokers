using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;

namespace Broker.Brokers.Grpc;

public class GrpcBroker : StartableBrokerBase
{
    public class Options
    {
        public string Address { get; init; } = null!;
        public Dictionary<string, string>? Metadata { get; init; } = null!;
    }

    private readonly Options _options;
    private GrpcChannel? _channel;
    private BrokerEvent.BrokerEventClient? _client;

    public GrpcBroker(string id, Options options)
        : base(id)
    {
        _options = options;
    }

    protected override async Task StartInternalAsync(CancellationToken token)
    {
        _channel = GrpcChannel.ForAddress(_options.Address);
        await _channel.ConnectAsync(token);
        _client = new BrokerEvent.BrokerEventClient(_channel);
    }

    protected override async Task HandleInternalAsync(Stream payloadStream, CancellationToken token)
    {
        var data = await ByteString.FromStreamAsync(payloadStream, token);
        var eventRequest = new BrokerEventRequest { Data = data };
        var meta = new Metadata();
        if (_options.Metadata != null)
        {
            foreach (var item in _options.Metadata)
            {
                meta.Add(item.Key, item.Value);
            }
        }
        
        await _client!.SendAsync(eventRequest, meta);
    }

    protected override async Task StopInternalAsync(CancellationToken token)
    {
        await _channel!.ShutdownAsync();
        _channel.Dispose();
    }
}