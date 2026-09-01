using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions
{
    public interface IFileUploadService
    {
        Task<(int StatusCode, FileResponseDto? File, string? Error)> HandleUploadAsync(
            HttpRequest request, Guid ownerId, Guid organisationId, CancellationToken ct = default);

        Task<PagedResponse<FileResponseDto, Guid>> GetFilesByOrganizationAsync(
            Guid organisationId, int page, int pageSize, CancellationToken ct);

        Task<(int StatusCode, string? Error)> DeleteFileAsync(
            Guid id, Guid organisationId, CancellationToken ct);

        Task<(int StatusCode, string? Message, string? Error)> ReprocessFileAsync(
            Guid id, Guid organisationId, CancellationToken ct);
    }
}
