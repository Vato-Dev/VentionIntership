using Application.Abstractions;
using Domain.Models;

namespace Application.Services
{
    public sealed class SessionService : ISessionService
    {
        private readonly IBaseRepository<Session> _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SessionService(IBaseRepository<Session> sessionRepository, IUnitOfWork unitOfWork)
        {
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
        }

        public Task<Session?> GetSessionByIdAsync(int id) => _sessionRepository.GetByIdAsync(id);

        public Task<IEnumerable<Session>> GetAllSessionsAsync() => _sessionRepository.GetAllAsync();

        public async Task CreateSessionAsync(Session session)
        {
            await _sessionRepository.AddAsync(session);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateSessionAsync(Session session)
        {
            _sessionRepository.Update(session);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteSessionAsync(int id)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session != null)
            {
                _sessionRepository.Delete(session);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
