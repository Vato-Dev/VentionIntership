using Application.DTOs;
using Domain.Models;

namespace Application.Abstractions
{
    public interface IUserService
    {
        Task<UserResponseDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResponse<UserResponseDto, Guid>> GetAllUsersAsync(Guid? keySetId = null, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default);
        Task<UserResponseDto> CreateUserAsync(UserCreateDto dto, CancellationToken cancellationToken = default);
        Task UpdateUserAsync(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
