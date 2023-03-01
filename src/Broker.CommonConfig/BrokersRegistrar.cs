using Broker.Brokers;
using Broker.Brokers.Grpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Broker.CommonConfig;

public static class BrokersRegistrar
{
    private class BrokerOptions
    {
        public string Type { get; init; } = null!;
        public IConfigurationSection Options { get; init; } = null!;
        public BrokerOptions InnerBroker { get; init; } = null!;
        public List<BrokerOptions> InnerBrokers { get; init; } = null!;
    }

    public static void RegisterAll(IConfigurationSection brokersSection, IServiceCollection services)
    {
        var brokersOptions = brokersSection.Get<Dictionary<string, BrokerOptions>>()!;
        foreach (var brokersOption in brokersOptions)
        {
            services.AddSingleton(s =>
                CreateBroker(s, brokersOption.Key, brokersOption.Value));
        }

        services.AddHttpClient();
        services.AddSingleton<IBrokerMetrics, BrokerMetrics>();
        services.AddSingleton<IBrokerGroup, BrokerGroup>();
        services.AddHostedService<BrokerGroupStarterService>();
    }
    
    private static IBroker CreateBroker(IServiceProvider sp, string id, BrokerOptions brokerOptions)
    {
        var type = brokerOptions.Type;
        var options = brokerOptions.Options;
        return type switch
        {
            nameof(LogBroker)
                => new LogBroker(id, sp.GetRequiredService<ILogger<LogBroker>>()),
            nameof(HttpBroker)
                => new HttpBroker(id, options.Get<HttpBroker.Options>()!, sp.GetRequiredService<IHttpClientFactory>()),
            nameof(FileBroker)
                => new FileBroker(id, options.Get<FileBroker.Options>()!),
            nameof(ConsoleBroker)
                => new ConsoleBroker(id),
            nameof(AzureBroker)
                => new AzureBroker(id, options.Get<AzureBroker.Options>()!),
            nameof(KafkaBroker)
                => new KafkaBroker(id, options.Get<KafkaBroker.Options>()!),
            nameof(GrpcBroker)
                => new GrpcBroker(id, options.Get<GrpcBroker.Options>()!),
            nameof(BatchBrokerDecorator)
                => new BatchBrokerDecorator(id, options.Get<BatchBrokerDecorator.Options>()!, CreateBroker(sp, id, brokerOptions.InnerBroker)),
            nameof(CompositeBrokerDecorator)
                => new CompositeBrokerDecorator(id, brokerOptions.InnerBrokers.Select(b => CreateBroker(sp, id, b))),
            _
                => throw new NotSupportedException($"Broker type '{type}' is not supported")
        };
    }
}