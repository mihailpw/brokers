using Broker.UnitTests.TestingCommon;
using Moq;

namespace Broker.UnitTests;

[TestOf(typeof(BrokerGroup))]
public class BrokerGroupTests : TestClassBase
{
    [TestCase("existing-broker-1")]
    [TestCase("existing-broker-2")]
    public void HasBroker_Exists_ReturnsTrue(string brokerId)
    {
        Mocker.Use<IEnumerable<IBroker>>(new[] { CreateBroker("existing-broker-1").Object, CreateBroker("existing-broker-2").Object });
        var unit = Mocker.CreateInstance<BrokerGroup>();

        var exists = unit.HasBroker(brokerId);

        Assert.That(exists, Is.True);
    }

    [TestCase("non-existing-broker-1")]
    [TestCase("non-existing-broker-2")]
    public void HasBroker_NotExists_ReturnsFalse(string brokerId)
    {
        Mocker.Use<IEnumerable<IBroker>>(new[] { CreateBroker("existing-broker-1").Object, CreateBroker("existing-broker-2").Object });
        var unit = Mocker.CreateInstance<BrokerGroup>();

        var exists = unit.HasBroker(brokerId);

        Assert.That(exists, Is.False);
    }

    [Test]
    public async Task HandleAsync_ExistingBroker_TriggersBrokersHandle()
    {
        const string broker1Id = "broker-1";
        const string broker2Id = "broker-2";
        var broker1Mock = CreateBroker(broker1Id);
        var broker2Mock = CreateBroker(broker2Id);
        Mocker.Use<IEnumerable<IBroker>>(new[] { broker1Mock.Object, broker2Mock.Object });
        var unit = Mocker.CreateInstance<BrokerGroup>();
        await unit.StartAsync(default);
        var payloadStream = new MemoryStream();

        await unit.HandleAsync(broker1Id, payloadStream, default);

        broker1Mock.Verify(
            x => x.HandleAsync(payloadStream, default),
            Times.Once);
        broker2Mock.Verify(
            x => x.HandleAsync(payloadStream, default),
            Times.Never);
    }

    protected override void OnSetup()
    {
        base.OnSetup();
        Mocker.GetMock<IBrokerMetrics>()
            .Setup(x => x.HandleDuration(It.IsAny<string>()))
            .Returns(() => { });
    }

    private static Mock<IBroker> CreateBroker(string id)
    {
        var brokerMock = new Mock<IBroker>();
        brokerMock.Setup(x => x.Id).Returns(id);
        return brokerMock;
    }
}