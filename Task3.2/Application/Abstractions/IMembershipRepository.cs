using Application.DTOs;
using Domain.Models;

namespace Application.Abstractions
{
    public interface IMembershipRepository : IBaseRepository<Membership, Guid>
    {
        public Task<MembershipBatchResultDto> ProcessBatchAsync(MembershipBatchOperationDto operation, CancellationToken cancellationToken);
        Task<bool> IsMemberAsync(Guid userId, Guid orgId, CancellationToken cancellationToken);
    }
}
