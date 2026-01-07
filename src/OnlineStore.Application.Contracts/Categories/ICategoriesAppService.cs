using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineStore.Categories
{
    public interface ICategoriesAppService
        : ICrudAppService<CategoryDto, int, PagedAndSortedResultRequestDto, CreateUpdateCategoryDto>
    {
        Task<PagedResultDto<CategoryDto>> GetFilteredListAsync(GetCategoriesInput input);
        Task<List<CategoryDto>> GetActiveListAsync();
        Task<bool> CanDeleteAsync(int id);
        Task<CategoryDto> ChangeDisplayOrderAsync(int id, int newOrder);
        Task<CategoryDto> ActivateAsync(int id);
        Task<CategoryDto> DeactivateAsync(int id);
    }
}
