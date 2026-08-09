using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

const string MainExchange = "main-exchange";
const string MainQueue = "my-main-queue";
const string RoutingKey = "orders.new";

const string DlxExchange = "my-dlx";
const string DlqQueue = "my-main-queue.dlq";
const string DlxRoutingKey = "orders.failed";

var factory = new ConnectionFactory { HostName = "localhost" };
factory.UserName = "guest"; //defalt but i had error , so i'm hardcoding it
factory.Password = "guest";
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(DlxExchange, ExchangeType.Direct);
await channel.QueueDeclareAsync(DlqQueue, durable: true, exclusive: false, autoDelete: false);
await channel.QueueBindAsync(DlqQueue, DlxExchange, DlxRoutingKey);

await channel.ExchangeDeclareAsync(MainExchange, ExchangeType.Topic);

var queueArguments = new Dictionary<string, object?>
{
    { "x-dead-letter-exchange", DlxExchange },
    { "x-dead-letter-routing-key", DlxRoutingKey }
};

await channel.QueueDeclareAsync(
    queue: MainQueue,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: queueArguments
);
await channel.QueueBindAsync(MainQueue, MainExchange, RoutingKey);

Console.WriteLine("RabbitMQ infrastructure configured.");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"message: '{message}'");

    Console.WriteLine("Dead Letter Queue...");
    
    await channel.BasicRejectAsync(deliveryTag: ea.DeliveryTag, requeue: false);//to reject it instead of pushin into main
};

await channel.BasicConsumeAsync(queue: MainQueue, autoAck: false, consumer: consumer);

string messageBody = "Test Order #1042";
var body = Encoding.UTF8.GetBytes(messageBody);

Console.WriteLine($"[Publisher] Sending message: '{messageBody}'");
await channel.BasicPublishAsync(
    exchange: MainExchange,
    routingKey: RoutingKey,
    body: body
);

Console.WriteLine("Press [enter] to exit the application.");
Console.ReadLine();
