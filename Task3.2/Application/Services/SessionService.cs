using System.Data;
using Application.Abstractions;
using Application.DTOs;
using Domain.Models;

namespace Application.Services
{
    public sealed class SessionService(IBaseRepository<Session, int> sessionRepository, IUnitOfWork unitOfWork) : ISessionService
    {
        public async Task<SessionResponseDto?> GetSessionByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await sessionRepository.GetByIdAsync(id, cancellationToken);
            if (session == null) return null;

            return MapToResponseDto(session);
        }

        public async Task<PagedResponse<SessionResponseDto, int>> GetAllSessionsAsync(
            int? keySetId = null, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default)
        {
            var pagedEntities = await sessionRepository.GetAllAsync(cancellationToken, keySetId , page, pageSize);

            var mappedData = pagedEntities.Data.Select(MapToResponseDto).ToList();

            return new PagedResponse<SessionResponseDto, int>
            {
                Data = mappedData,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize,
                TotalItems = pagedEntities.TotalItems,
                TotalPages = pagedEntities.TotalPages,
                LastSeenId = pagedEntities.LastSeenId
            };
        }

        public async Task<SessionResponseDto> CreateSessionAsync(SessionCreateDto dto, CancellationToken cancellationToken = default)
        {
            var session = new Session
            {
                UserId = Guid.Parse(dto.UserId), 
                AccessToken = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = dto.ExpiresAt.ToUniversalTime(),
                IsActive = true
            };

            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                await sessionRepository.AddAsync(session, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return MapToResponseDto(session);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task UpdateSessionAsync(int id, SessionUpdateDto dto, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                var session = await sessionRepository.GetByIdAsync(id, cancellationToken);
                if (session == null) return;

                session.IsActive = dto.IsActive;
                session.ExpiresAt = dto.ExpiresAt.ToUniversalTime();

                sessionRepository.Update(session);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task DeleteSessionAsync(int id, CancellationToken cancellationToken = default)
        {
            await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                var session = await sessionRepository.GetByIdAsync(id, cancellationToken);
                if (session != null)
                {
                    sessionRepository.Delete(session);
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

        private static SessionResponseDto MapToResponseDto(Session session)
        {
            return new SessionResponseDto
            {
                Id = session.Id.ToString(),
                UserId = session.UserId.ToString(), 
                IsActive = session.IsActive,
                CreatedAt = session.CreatedAt,
                ExpiresAt = session.ExpiresAt
            };
        }
    }
}
