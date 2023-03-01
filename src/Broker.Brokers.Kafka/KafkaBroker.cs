using Broker.Utils;
using Confluent.Kafka;

namespace Broker.Brokers;

public class KafkaBroker : StartableBrokerBase
{
    public class Options
    {
        public Dictionary<string, string> KafkaConfig { get; init; } = null!;
        public string Topic { get; init; } = null!;
    }

    private readonly Options _options;
    private IProducer<Null, byte[]>? _p;

    public KafkaBroker(string id, Options options)
        : base(id)
    {
        _options = options;
    }

    protected override Task StartInternalAsync(CancellationToken token)
    {
        _p = new ProducerBuilder<Null, byte[]>(_options.KafkaConfig).Build();
        return Task.CompletedTask;
    }

    protected override async Task HandleInternalAsync(Stream payloadStream, CancellationToken token)
    {
        var bytes = await payloadStream.ToByteArrayAsync();
        await _p!.ProduceAsync(_options.Topic, new Message<Null, byte[]> { Value = bytes }, token);
    }

    protected override Task StopInternalAsync(CancellationToken token)
    {
        _p!.Flush(TimeSpan.FromSeconds(10));
        _p!.Dispose();
        return Task.CompletedTask;
    }
}