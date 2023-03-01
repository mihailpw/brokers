using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Broker.Utils;

namespace Broker.Brokers;

public class AzureBroker : BrokerBase
{
    public class Options
    {
        public string ConnectionString { get; init; } = null!;
        public string EventHubName { get; init; } = null!;
        public string? PartitionId { get; init; }
        public IReadOnlyDictionary<string, object>? Metadata { get; init; }
    }

    private readonly Options _options;
    private readonly EventHubProducerClient _producer;

    public AzureBroker(string id, Options options)
        : base(id)
    {
        _options = options;
        _producer = new EventHubProducerClient(options.ConnectionString, options.EventHubName);
    }

    public override async Task HandleAsync(Stream payloadStream, CancellationToken token)
    {
        var partitionId = _options.PartitionId ?? (await _producer.GetPartitionIdsAsync(token)).First();
        var batchOptions = new CreateBatchOptions
        {
            PartitionId = partitionId
        };

        using var eventBatch = await _producer.CreateBatchAsync(batchOptions, token);

        var payload = await payloadStream.ToByteArrayAsync();
        var eventData = new EventData(payload);
        if (_options.Metadata != null)
        {
            foreach (var o in _options.Metadata)
                eventData.Properties.Add(o);
        }

        if (!eventBatch.TryAdd(eventData))
        {
            throw new Exception($"The event at could not be added.");
        }

        await _producer.SendAsync(eventBatch, token);
    }

    public override async Task StopAsync(CancellationToken token)
    {
        await _producer.CloseAsync(token);
    }
}