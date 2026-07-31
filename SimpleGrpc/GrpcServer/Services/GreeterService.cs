using Grpc.Core;
using SharedContracts;
namespace GrpcServer.Services;

public class GreeterService : Greeter.GreeterBase
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = $"{request.Name} gRPC works."
        });
    }
}
