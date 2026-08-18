using Application.Abstractions;
using Application.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class MembershipRepository(AppDbContext context) : BaseRepository<Membership, Guid>(context)  , IMembershipRepository
    {
        private readonly AppDbContext _context = context;
        public async Task<MembershipBatchResultDto> ProcessBatchAsync(MembershipBatchOperationDto operation, CancellationToken cancellationToken)
        {
            var result = new MembershipBatchResultDto();

            foreach (var item in operation.ToDelete)
            {
                var userId = Guid.Parse(item.UserId);
                var orgId = Guid.Parse(item.OrganisationId);

                var affectedRows = await _context.Memberships
                    .Where(m => m.UserId == userId && m.OrganizationId == orgId)
                    .ExecuteDeleteAsync(cancellationToken);

                if (affectedRows > 0)
                {
                    result.Successes++;
                }
                else
                {
                    result.Failures.Add(new BatchFailureInfo
                    {
                        Operation = "Delete",
                        OrganizationId = item.OrganisationId,
                        Error = "Membership record not found for deletion."
                    });
                }
            }

            foreach (var createMemb in operation.ToCreate)
            {
                var userId = Guid.Parse(createMemb.UserId);
                var orgId = Guid.Parse(createMemb.OrganisationId);
                var role = createMemb.Role;

                if (await CheckIfExistsAsync(userId, orgId, role, cancellationToken))
                {
                    result.Failures.Add(new BatchFailureInfo
                    {
                        Operation = "Create",
                        OrganizationId = createMemb.OrganisationId,
                        Error = "This user already has this exact role in the organization."
                    });
                    continue;
                }

                int updatedRows = await _context.Memberships
                    .Where(m => m.UserId == userId && m.OrganizationId == orgId)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(m => m.Role, role), cancellationToken);

                if (updatedRows > 0)
                {
                    result.Successes++;
                }
                else
                {
                    var newMembership = new Membership
                    {
                        UserId = userId,
                        OrganizationId = orgId,
                        Role = role,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.Memberships.AddAsync(newMembership, cancellationToken);
                    result.Successes++;
                }
            }

            foreach (var item in operation.ToUpdate)
            {
                var userId = Guid.Parse(item.UserId);
                var orgId = Guid.Parse(item.OrganisationId);

                int updatedRows = await _context.Memberships
                    .Where(m => m.UserId == userId && m.OrganizationId == orgId)
                    .ExecuteUpdateAsync(setters => 
                        setters.SetProperty(m => m.Role, item.Role), cancellationToken);

                if (updatedRows > 0)
                {
                    result.Successes++;
                }
                else
                {
                    result.Failures.Add(new BatchFailureInfo
                    {
                        Operation = "Update",
                        OrganizationId = item.OrganisationId,
                        Error = "Membership record not found."
                    });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return result;
        }

        private Task<bool> CheckIfExistsAsync(Guid userId, Guid orgId, string? role, CancellationToken cancellationToken)
        {
            return role is not null 
                ? _context.Memberships.AnyAsync(m => m.UserId == userId && m.OrganizationId == orgId && EF.Functions.ILike(m.Role, role), cancellationToken) 
                : _context.Memberships.AnyAsync(m => m.UserId == userId && m.OrganizationId == orgId, cancellationToken);
        }
    }
}
