using System.Text;
using Broker.Face.Consumers.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Broker.Face.Consumers.Controllers;

/// <summary>
/// This is test controller for HttpBroker
/// </summary>
[ApiController]
[Route("http-consumer")]
public class HttpConsumerController : ControllerBase
{
    private readonly ILogger<HttpConsumerController> _logger;

    public HttpConsumerController(ILogger<HttpConsumerController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        var body = Encoding.UTF8.GetString(await Request.Body.ToByteArrayAsync());
        _logger.LogInformation($"HttpConsumer:POST - Data received:{Environment.NewLine}{body}");
        return Ok();
    }

    [HttpPatch]
    public async Task<IActionResult> Patch()
    {
        var body = Encoding.UTF8.GetString(await Request.Body.ToByteArrayAsync());
        _logger.LogInformation($"HttpConsumer:PATCH - Data received:{Environment.NewLine}{body}");
        return Ok();
    }
}