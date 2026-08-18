using Application.DTOs;

namespace Application.Abstractions
{
    public interface IOrganizationService
    {
        Task<OrganizationResponseDto?> GetOrganizationByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResponse<OrganizationResponseDto, Guid>> GetAllOrganizationsAsync(Guid? keySetId = null, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default);
        Task<OrganizationResponseDto> CreateOrganizationAsync(OrganizationCreateDto dto, CancellationToken cancellationToken = default);
        Task UpdateOrganizationAsync(Guid id, OrganizationUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteOrganizationAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
