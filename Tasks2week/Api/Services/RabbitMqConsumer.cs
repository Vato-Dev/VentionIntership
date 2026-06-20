using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Api.Services;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private const string QueueName = "todo-events";

    public RabbitMqConsumer(IConfiguration config, ILogger<RabbitMqConsumer> logger)
    {
        _config = config;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = _config["RabbitMq:Host"] };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.QueueDeclare(QueueName, durable: false, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, e) =>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            _logger.LogInformation("Received from RabbitMQ: {Message}", message);
        };

        channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);

        stoppingToken.Register(() =>
        {
            channel.Dispose();
            connection.Dispose();
        });

        return Task.CompletedTask;
    }
}
