using Application.Abstractions;
using Domain.Models;

namespace Application.Services
{
   public sealed class OrganizationService(IBaseRepository<Organization> organizationRepository, IUnitOfWork unitOfWork) : IOrganizationService
   {
      public Task<Organization?> GetOrganizationByIdAsync(int id) => organizationRepository.GetByIdAsync(id);

      public Task<IEnumerable<Organization>> GetAllOrganizationsAsync() => organizationRepository.GetAllAsync();

      public async Task CreateOrganizationAsync(Organization organization)
      {
         await organizationRepository.AddAsync(organization);
         await unitOfWork.SaveChangesAsync();
      }

      public async Task UpdateOrganizationAsync(Organization organization)
      {
         organizationRepository.Update(organization);
         await unitOfWork.SaveChangesAsync();
      }

      public async Task DeleteOrganizationAsync(int id)
      {
         var organization = await organizationRepository.GetByIdAsync(id);
         if (organization != null)
         {
            organizationRepository.Delete(organization);
            await unitOfWork.SaveChangesAsync();
         }
      }
   }
}
