using Microsoft.AspNetCore.Mvc;
using Entity;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitmqQueueProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QueueController : Controller
    {
        private ConnectionFactory _factory;
        private IConnection _connection;
        public QueueController()
        {
            _factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = _factory.CreateConnection();
        }
        [HttpPost]
        public IActionResult AddtoQueue(EmailModel model)
        {
            try
            {
                using (var channel = this._connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "EmailQueue",
                        durable: false,
                        exclusive: false,
                        autoDelete: true,
                        arguments: null);

                    var message = JsonSerializer.Serialize(model);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "EmailQueue",
                        basicProperties: null,
                        body: body
                        );
                }

                //return true;
            }
            catch (System.Exception ex)
            {
                //return false;
            }
            return Ok();

        }
    }
}
