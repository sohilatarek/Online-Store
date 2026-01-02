using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace OnlineStore.Categories
{
    public class CategoriesAppService : CrudAppService<Category, CategoryDto, int, PagedAndSortedResultRequestDto, CreateUpdateCategoryDto>, ICategoriesAppService
    {
        public CategoriesAppService(IRepository<Category, int> repository) : base(repository)
        {
        }
    }
}
