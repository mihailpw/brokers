namespace Broker.Brokers;

public class HttpBroker : BrokerBase
{
    public class Options
    {
        public string RequestUri { get; init; } = null!;
        public string HttpMethod { get; init; } = null!;
        public Dictionary<string, string>? Headers { get; init; }
    }

    private readonly Options _options;
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpBroker(string id, Options options, IHttpClientFactory httpClientFactory)
        : base(id)
    {
        _options = options;
        _httpClientFactory = httpClientFactory;
    }

    public override async Task HandleAsync(Stream payloadStream, CancellationToken token)
    {
        using var client = _httpClientFactory.CreateClient();
        
        var message = new HttpRequestMessage(new HttpMethod(_options.HttpMethod), _options.RequestUri)
        {
            Content = new StreamContent(payloadStream),
        };
        if (_options.Headers != null)
        {
            foreach (var header in _options.Headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }
        }
        
        await client.SendAsync(message, token);
    }
}