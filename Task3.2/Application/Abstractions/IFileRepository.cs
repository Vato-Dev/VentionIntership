using Domain.Models;

namespace Application.Abstractions
{
    public interface IFileRepository
    {
        Task AddAsync(FileModel file, CancellationToken ct = default);
        Task<FileModel?> GetByHashAsync(string checksum, Guid organisationId, CancellationToken ct = default);
    }
}
