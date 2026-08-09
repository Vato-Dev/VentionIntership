using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

if (args.Length < 1)
{
    Console.WriteLine("Usage: {0} <routingKey>", Environment.GetCommandLineArgs()[0]);
    Console.ReadKey();
    Environment.ExitCode = 1;
}

var factory = new ConnectionFactory { HostName =  "localhost" };
factory.UserName = "guest";
factory.Password = "guest";
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync("exchange", ExchangeType.Topic);

var queueDeclareResult = await channel.QueueDeclareAsync();
var queueName = queueDeclareResult.QueueName;

foreach (string? bindingKey in args)
{
    await channel.QueueBindAsync(queue: queueName, exchange: "exchange", routingKey: bindingKey);

}

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();