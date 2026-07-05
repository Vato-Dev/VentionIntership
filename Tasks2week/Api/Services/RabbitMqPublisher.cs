

using System.Text;
using RabbitMQ.Client;

namespace Api.Services
{
    public class RabbitMqPublisher : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string QueueName = "todo-events";

        public RabbitMqPublisher(IConfiguration config)
        {
            var factory = new ConnectionFactory { HostName = config["RabbitMq:Host"] };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(QueueName, durable: false, exclusive: false, autoDelete: false);
        }

        public void PublishTodoCreated(int id, string title)
        {
            var message = $"Todo created: [{id}] {title}";
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: QueueName, body: body);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
