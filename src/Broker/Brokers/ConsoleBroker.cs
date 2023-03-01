namespace Broker.Brokers;

public class ConsoleBroker : BrokerBase
{
    public ConsoleBroker(string id)
        : base(id)
    {
    }

    public override async Task HandleAsync(Stream payloadStream, CancellationToken token)
    {
        var consoleOutputTextWriter = Console.Out;
        var streamReader = new StreamReader(payloadStream);
        var buffer = new char[1024];
        while (true)
        {
            var readBytes = await streamReader.ReadAsync(buffer, token);
            if (readBytes == 0)
            {
                break;
            }

            await consoleOutputTextWriter.WriteAsync(buffer, 0, readBytes);
        }
    }
}