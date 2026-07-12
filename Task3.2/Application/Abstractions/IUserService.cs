using Application.DTOs;
using Domain.Models;

namespace Application.Abstractions
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResponse<User>> GetAllUsersAsync(int? keySetId = null, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default);
        Task CreateUserAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
        Task DeleteUserAsync(int id, CancellationToken cancellationToken = default);
    }

    public interface IOrganizationService
    {
        Task<Organization?> GetOrganizationByIdAsync(CancellationToken cancellationToken,int id);
        Task<PagedResponse<Organization>> GetAllOrganizationsAsync(CancellationToken cancellationToken,int? keySetId = null, int? page = 1, int? pageSize = 10);
        Task CreateOrganizationAsync(Organization organization,CancellationToken cancellationToken);
        Task UpdateOrganizationAsync(Organization organization, CancellationToken cancellationToken);
        Task DeleteOrganizationAsync(int id, CancellationToken cancellationToken);
    }
}
