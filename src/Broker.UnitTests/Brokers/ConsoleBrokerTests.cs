using System.Text;
using Broker.Brokers;
using Broker.UnitTests.TestingCommon;

namespace Broker.UnitTests.Brokers;

[TestOf(typeof(ConsoleBroker))]
public class ConsoleBrokerTests : TestClassBase
{
    [Test]
    public async Task Handle()
    {
        const string data = "test-data-string";
        var unit = Mocker.CreateInstance<ConsoleBroker>();
        await unit.StartAsync(default);
        var brokerInputStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        var consoleOutputStream = new MemoryStream();
        Console.SetOut(new StreamWriter(consoleOutputStream) { AutoFlush = true, });

        await unit.HandleAsync(brokerInputStream, default);

        var actualData = Encoding.UTF8.GetString(consoleOutputStream.ToArray());
        Assert.That(actualData, Is.EqualTo(data));
    }

    protected override void OnSetup()
    {
        base.OnSetup();
        
        Mocker.Use("broker-1");
    }
}