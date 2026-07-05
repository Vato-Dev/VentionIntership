using System.Data;
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
            await unitOfWork.BeginTransactionAsync();
            try
            {
                await organizationRepository.AddAsync(organization);
                await unitOfWork.SaveChangesAsync();
                
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateOrganizationAsync(Organization organization)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                organizationRepository.Update(organization);
                await unitOfWork.SaveChangesAsync();
                
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteOrganizationAsync(int id)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                var organization = await organizationRepository.GetByIdAsync(id);
                if (organization != null)
                {
                    organizationRepository.Delete(organization);
                    await unitOfWork.SaveChangesAsync();
                }
                
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}