using Microsoft.AspNetCore.Mvc;
using Steeltoe.Messaging.RabbitMQ.Core;
using System.Text;

namespace Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageProducerController : ControllerBase
    {
        private readonly RabbitTemplate _rabbitTemplate;

        public MessageProducerController(RabbitTemplate rabbitTemplate)
        {
            _rabbitTemplate = rabbitTemplate;
        }

        // Endpoint to send messages
        [HttpPost("send")]
        public IActionResult SendMessage([FromBody] string message)
        {
            string queueName = "TestQ";
            _rabbitTemplate.ConvertAndSend(queueName, message);
            return Ok($"Message '{message}' sent to queue '{queueName}'");
        }

        // Endpoint to receive messages
        [HttpGet("receive")]
        public IActionResult ReceiveMessage()
        {
            string queueName = "TestQ";

            // Receive the message as an IMessage object
            var rawMessage = _rabbitTemplate.Receive(queueName);

            if (rawMessage == null)
            {
                return Ok("No messages in the queue");
            }

            // Extract the payload from the IMessage object
            var payload = rawMessage.Payload;

            // Convert payload to a string if it's a byte array
            string message = payload is byte[] byteArray
                ? Encoding.UTF8.GetString(byteArray)
                : payload.ToString();

            return Ok($"Received message from queue '{queueName}': {message}");
        }
    }
}
