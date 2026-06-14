using LibraryManager.Application.Interfaces;
using LibraryManager.Domain.Models;


namespace LibraryManager.Application.Services
{
    public sealed class CatalogueService(
        IBaseRepository<Catalogue> catalogueRepository,
        IUnitOfWork unitOfWork) : ICatalogueService
    {

        public async Task<int> CreateCatalogue(string name, int? parentId, CancellationToken ct)
        {

            if (parentId.HasValue)
            {
                var parent = await catalogueRepository.GetByIdAsync(parentId.Value, ct);
                if (parent == null) throw new Exception("Parent catalogue not found");
            }

            var catalogue = new Catalogue
            {
                Name = name,
                ParentId = parentId
            };

            catalogueRepository.Add(catalogue);
            await unitOfWork.SaveChangesAsync(ct);

            return catalogue.Id;
        }

        public async Task<Catalogue> GetCatalogue(int id, CancellationToken ct)
        {
            var catalogue = await catalogueRepository.GetByIdAsync(id, ct);
            return catalogue ?? throw new Exception("Catalogue not found");
        }

        public async Task UpdateCatalogue(int id, string newName, int? newParentId, CancellationToken ct)
        {
            var catalogue = await catalogueRepository.GetByIdAsync(id, ct);
            if (catalogue == null) throw new Exception("Catalogue not found");

            if (newParentId == id)
                throw new Exception("Catalogue cannot be its own parent");

            catalogue.Name = newName;
            catalogue.ParentId = newParentId;

            catalogueRepository.Update(catalogue);
            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task DeleteCatalogue(int id, CancellationToken ct)
        {
            var catalogue = await catalogueRepository.GetByIdAsync(id, ct);
            if (catalogue == null) throw new Exception("Catalogue not found");
            
            if (catalogue.TotalQuantity > 0)
                throw new Exception("Cannot delete catalogue with books. Move or delete books first.");

            catalogueRepository.Delete(catalogue);
            await unitOfWork.SaveChangesAsync(ct);
        }
    }

}
