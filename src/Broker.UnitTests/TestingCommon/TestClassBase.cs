using Moq.AutoMock;

namespace Broker.UnitTests.TestingCommon;

public abstract class TestClassBase
{
    protected AutoMocker Mocker { get; private set; }
    
    [SetUp]
    public void Setup()
    {
        Mocker = new AutoMocker();
        OnSetup();
    }

    protected virtual void OnSetup()
    {
    }
}