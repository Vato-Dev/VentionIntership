using Grpc.Net.Client;
using SharedContracts;

using var channel = GrpcChannel.ForAddress("https://localhost:7108");
var client1 = new Greeter.GreeterClient(channel);

try
{
    Console.WriteLine("Sending requests...");
    var response = await client1.SayHelloAsync(new HelloRequest { Name = "Luffy" });
    
    Console.WriteLine($"response :: {response.Message}");
    
}
catch (Exception ex)
{
    Console.WriteLine($"ERRRRRRRRRROR: {ex.Message}");
}

var client2 = new Calculator.CalculatorClient(channel);

try
{
    int num1 = 15;
    int num2 = 27;

    Console.WriteLine($"sending request: {num1} + {num2}...");
    
    var response = await client2.AddAsync(new AddRequest 
    { 
        NumberA = num1, 
        NumberB = num2 
    });
    
    Console.WriteLine($"result: {response.Result}");
}
catch (Exception ex)
{
    Console.WriteLine($"error: {ex.Message}");
}
Console.ReadKey();
