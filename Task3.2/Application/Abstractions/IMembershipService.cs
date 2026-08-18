using Application.DTOs;

namespace Application.Abstractions
{
    public interface IMembershipService
    {
        public Task<MembershipBatchResultDto> ExecuteAsync(MembershipBatchOperationDto operationDto, CancellationToken cancellationToken = default);
    }
}
