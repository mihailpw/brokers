using Confluent.Kafka;

namespace Broker.Face.Consumers.Services;

public class KafkaConsumerService : BackgroundService
{
    public class Options
    {
        public string Topic { get; init; } = null!;
        public string GroupId { get; init; } = null!;
        public string BootstrapServers { get; init; } = null!;
    }

    private readonly Options _options;
    private readonly ILogger<KafkaConsumerService> _logger;

    public KafkaConsumerService(Options options, ILogger<KafkaConsumerService> logger)
    {
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = _options.GroupId,
            BootstrapServers = _options.BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        try
        {
            using var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build();
            consumerBuilder.Subscribe(_options.Topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumer = await Task.Run(
                        () => consumerBuilder.Consume(stoppingToken), stoppingToken);
                    var message = consumer.Message.Value;
                    _logger.LogInformation($"KafkaConsumer:{_options.Topic} - Data received:{Environment.NewLine}{message}");
                }
                catch
                {
                    if (!stoppingToken.IsCancellationRequested)
                        await Task.Delay(1000, stoppingToken);
                }
            }

            consumerBuilder.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Consuming error");
        }
    }
}
