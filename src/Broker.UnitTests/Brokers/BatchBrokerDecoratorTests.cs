using System.Text;
using Broker.Brokers;
using Broker.UnitTests.TestingCommon;
using Moq;

namespace Broker.UnitTests.Brokers;

[TestOf(typeof(BatchBrokerDecorator))]
public class BatchBrokerDecoratorTests : TestClassBase
{
    [Test]
    public async Task HandleAsync_BatchOversize_TriggersInnerBroker()
    {
        Mocker.Use(new BatchBrokerDecorator.Options { BatchSize = 2 });
        var unit = Mocker.CreateInstance<BatchBrokerDecorator>();
        await unit.StartAsync(default);
        var brokerInputStream = new MemoryStream(Encoding.UTF8.GetBytes("test-data-string"));

        await unit.HandleAsync(brokerInputStream, default);

        Mocker.GetMock<IBroker>().Verify(
            x => x.HandleAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task HandleAsync_SmallData_DoesNotTriggersInnerBroker()
    {
        Mocker.Use(new BatchBrokerDecorator.Options { BatchSize = 1024 });
        var unit = Mocker.CreateInstance<BatchBrokerDecorator>();
        await unit.StartAsync(default);
        var brokerInputStream = new MemoryStream(Encoding.UTF8.GetBytes("test-data-string"));

        await unit.HandleAsync(brokerInputStream, default);

        Mocker.GetMock<IBroker>().Verify(
            x => x.HandleAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    protected override void OnSetup()
    {
        base.OnSetup();
        
        Mocker.Use("batch-1");
    }
}