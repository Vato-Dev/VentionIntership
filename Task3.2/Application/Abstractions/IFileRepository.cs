using Application.DTOs;
using Domain.Models;

namespace Application.Abstractions
{
    public interface IFileRepository
    {
        Task AddAsync(FileModel file, CancellationToken ct = default);
        Task<FileModel?> GetByHashAsync(string checksum, Guid organisationId, CancellationToken ct = default);
        Task<PagedResponse<FileModel, Guid>> GetByOrganizationIdAsync(Guid organisationId, int page, int pageSize, CancellationToken ct = default);
        Task<FileModel?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task UpdateAsync(FileModel file, CancellationToken ct = default);
        Task DeleteAsync(FileModel file, CancellationToken ct = default);
    }
}
