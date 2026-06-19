using LibraryManager.Domain.Models;

namespace LibraryManager.Application.Interfaces
{
    public interface ICatalogueService
    {
        Task<int> CreateCatalogue(string name, int? parentId, CancellationToken ct);
        Task<Catalogue> GetCatalogue(int id, CancellationToken ct);
        Task UpdateCatalogue(int id, string newName, int? newParentId, CancellationToken ct);
        Task DeleteCatalogue(int id, CancellationToken ct);
    }
}
