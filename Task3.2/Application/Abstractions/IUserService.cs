using Application.DTOs;
using Domain.Models;

namespace Application.Abstractions
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<PagedResponse<User>> GetAllUsersAsync(int? keySetId = null, int? page = 1, int? pageSize = 10);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }

    public interface IOrganizationService
    {
        Task<Organization?> GetOrganizationByIdAsync(int id);
        Task<PagedResponse<Organization>> GetAllOrganizationsAsync(int? keySetId = null, int? page = 1, int? pageSize = 10);
        Task CreateOrganizationAsync(Organization organization);
        Task UpdateOrganizationAsync(Organization organization);
        Task DeleteOrganizationAsync(int id);
    }
}
