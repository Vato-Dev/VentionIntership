using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManager.Application.Interfaces
{
    public interface IBaseRepository<TModel> where TModel : class
    {
        Task<TModel?> GetByIdAsync(int Id, CancellationToken cancellationToken);
        void Add(TModel model);
        void Delete(TModel Id);
        void Update(TModel model);
    }
}
