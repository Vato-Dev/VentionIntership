

using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName =  "localhost" };
factory.UserName = "guest";
factory.Password = "guest";
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();


await channel.ExchangeDeclareAsync("exchange", ExchangeType.Topic);

var routingKey = args.Length > 0 ? args[0] : "nothing.there";
var message = args.Length > 1 ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
var body = Encoding.UTF8.GetBytes(message);
await channel.BasicPublishAsync("exchange", routingKey, body);

Console.WriteLine(" [x] Sent message {0}", message);