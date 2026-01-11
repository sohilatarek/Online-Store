using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace OnlineStore.Categories
{
   
    public interface ICategoryRepository : IRepository<Category, int>
    {
       
        Task<List<Category>> GetActiveListAsync(CancellationToken cancellationToken = default);

        Task<bool> IsNameUniqueAsync(
            string name,
            int? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<Category> FindByNameAsync(
            string name,
            CancellationToken cancellationToken = default);
       
        Task<Dictionary<int, int>> GetProductCountsAsync(CancellationToken cancellationToken = default);

        Task<int> GetProductCountAsync(int categoryId, CancellationToken cancellationToken = default);

        Task<bool> HasProductsAsync(int categoryId, CancellationToken cancellationToken = default);

        Task<List<Category>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            bool? isActive = null,
            string searchTerm = null,
            CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(
            bool? isActive = null,
            string searchTerm = null,
            CancellationToken cancellationToken = default);

        Task<int> GetNextIdAsync(CancellationToken cancellationToken = default);
    }
}