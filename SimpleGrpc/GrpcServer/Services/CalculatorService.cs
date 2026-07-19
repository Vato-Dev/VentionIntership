using Grpc.Core;
using SharedContracts;

namespace GrpcServer.Services;

public class CalculatorService : Calculator.CalculatorBase
{
    public override Task<AddReply> Add(AddRequest request, ServerCallContext context)
    {
        int sum = request.NumberA + request.NumberB;

        return Task.FromResult(new AddReply
        {
            Result = sum
        });
    }
}
