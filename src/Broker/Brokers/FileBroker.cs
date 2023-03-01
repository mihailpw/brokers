namespace Broker.Brokers;

public class FileBroker : BrokerBase
{
    public class Options
    {
        public string FilePath { get; init; } = null!;
    }

    private readonly Options _options;

    public FileBroker(string id, Options options)
        : base(id)
    {
        _options = options;
    }

    public override async Task HandleAsync(Stream payloadStream, CancellationToken token)
    {
        await using var fileStream = File.OpenWrite(_options.FilePath);
        fileStream.Seek(0, SeekOrigin.End);
        await payloadStream.CopyToAsync(fileStream, token);
    }
}