using Microsoft.AspNetCore.Mvc;

namespace Broker.Face.Http.Controllers;

[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly IBrokerGroup _brokerGroup;

    public EventController(IBrokerGroup brokerGroup)
    {
        _brokerGroup = brokerGroup;
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromQuery(Name = "broker")] string brokerId,
        CancellationToken token)
    {
        if (!_brokerGroup.HasBroker(brokerId))
            return NotFound();

        await _brokerGroup.HandleAsync(brokerId, Request.Body, token);
        return Ok();
    }
}