using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesOrder.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyController : ControllerBase
    {
        //private readonly IPublishEndpoint _publishEndpoint;
        //public MyController(IPublishEndpoint publishEndpoint)
        //{
        //    _publishEndpoint = publishEndpoint;
        //}

        //[HttpPost]
        //public async Task<IActionResult> SendMessage(string text)
        //{
        //    var message = new MyMessage { Text = text };
        //    await _publishEndpoint.Publish(message);
        //    return Ok();
        //}

        private readonly string _connectionString;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public MyController(IConfiguration configuration, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _connectionString = $"{configuration["RabbitMqSettings:Uri"]}{configuration["RabbitMqSettings:Queue"]}";
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;

        }


        [HttpPost]
        //public async Task<IActionResult> SendOrder(ISendEndpointProvider sendEndpointProvider)
        //{
        //    var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:input-queue"));
        //    await endpoint.Send(new MyMessage { Text = "5555"});
        //    return Ok(endpoint);
        //}
        public async Task<IActionResult> SendMessage(MyMessage payload, CancellationToken cancellationToken)
        {
            await _publishEndpoint.Publish(payload, cancellationToken);
            return Ok();
        }
    }
}
