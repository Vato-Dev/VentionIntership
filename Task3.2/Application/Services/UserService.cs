using System.Data;
using Application.Abstractions;
using Application.DTOs;
using Domain.Models;

namespace Application.Services
{
    public sealed class UserService(
        IBaseRepository<User, Guid> userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher) : IUserService
    {
        public async Task<UserResponseDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null) return null;

            return MapToResponseDto(user);
        }

        public async Task<PagedResponse<UserResponseDto, Guid>> GetAllUsersAsync(
            Guid? keySetId = null, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default)
        {
            var pagedEntities = await userRepository.GetAllAsync(cancellationToken, keySetId ?? Guid.Empty, page, pageSize);

            var mappedData = pagedEntities.Data.Select(MapToResponseDto).ToList();

            return new PagedResponse<UserResponseDto, Guid>
            {
                Data = mappedData,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize,
                TotalItems = pagedEntities.TotalItems,
                TotalPages = pagedEntities.TotalPages,
                LastSeenId = pagedEntities.LastSeenId
            };
        }

        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto dto, CancellationToken cancellationToken = default)
        {
            var user = new User
            {
                Email = dto.Email.ToLower(), // that's ugly workaround i should create field for normalized email
                Name = dto.Name,
                Role = "User",
      
                PasswordHash = passwordHasher.Hash(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                await userRepository.AddAsync(user, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return MapToResponseDto(user);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task UpdateUserAsync(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                var user = await userRepository.GetByIdAsync(id, cancellationToken);
                if (user == null) return;

                user.Email = dto.Email;
                user.Name = dto.Name;

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    user.PasswordHash = passwordHasher.Hash(dto.Password);
                }

                userRepository.Update(user);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                var user = await userRepository.GetByIdAsync(id, cancellationToken);
                if (user != null)
                {
                    userRepository.Delete(user);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        private static UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                Name = user.Name,
                Organisations = user.Memberships?.Select(m => new UserOrganizationMembershipDto
                {
                    Id = m.OrganizationId.ToString(),
                    Name = m.Organization?.Name ?? string.Empty,
                    Role = m.Role
                }).ToList() ?? []
            };
        }
    }
}