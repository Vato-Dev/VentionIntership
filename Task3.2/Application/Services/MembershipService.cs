using Application.Abstractions;
using Application.DTOs;

namespace Application.Services
{
    public sealed class MembershipService(IMembershipRepository repository) : IMembershipService
    {
        public async Task<MembershipBatchResultDto> ExecuteAsync(MembershipBatchOperationDto operationDto, CancellationToken cancellationToken = default) =>
           await  repository.ProcessBatchAsync(operationDto, cancellationToken);

    }
}
