using System.Text;
using BenchmarkDotNet.Attributes;
using Broker.Benchmarks.Stubs;
using Broker.Brokers;
using Microsoft.Extensions.Logging.Abstractions;

namespace Broker.Benchmarks;

[MemoryDiagnoser]
public class BrokerGroupBenchmark
{
    private BrokerGroup _target = null!;
    private byte[] _data = null!;
    
    [Params(10, 100, 100000)]
    public int BrokersCount { get; set; }
    
    [GlobalSetup]
    public async Task GlobalSetup()
    {
        var brokers = Enumerable.Range(0, BrokersCount)
            .Select(i => new LogBroker($"log-{i + 1}", NullLogger<LogBroker>.Instance))
            .ToList();
        _target = new BrokerGroup(
            NullLogger<BrokerGroup>.Instance,
            new NullBrokerMetrics(),
            brokers);
        await _target.StartAsync(default);
        _data = Encoding.UTF8.GetBytes("Test Hello");
    }

    [Benchmark]
    public async Task HandleAsync_ExistingBroker()
    {
        await _target.HandleAsync("log-1", new MemoryStream(_data), default);
    }

    [Benchmark]
    public async Task HandleAsync_NonExistingBroker()
    {
        await _target.HandleAsync("console-1", new MemoryStream(_data), default);
    }
}