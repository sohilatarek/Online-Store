using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using OnlineStore.Categories;
using OnlineStore.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace OnlineStore.Categories
{

    [RemoteService(false)]
   
    [Authorize(OnlineStorePermissions.Categories.Default)]
    public class CategoriesAppService : CrudAppService<Category, CategoryDto, int, PagedAndSortedResultRequestDto, CreateUpdateCategoryDto>, ICategoriesAppService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly CategoryManager _categoryManager;
        private readonly ILogger<CategoriesAppService> _logger;

        public CategoriesAppService(
            ICategoryRepository repository,
            CategoryManager categoryManager,
            ILogger<CategoriesAppService> logger) : base(repository)
        {
            _categoryRepository = repository;
            _categoryManager = categoryManager;
            _logger = logger;

            // Set default permissions
            GetPolicyName = OnlineStorePermissions.Categories.Default;
            GetListPolicyName = OnlineStorePermissions.Categories.Default;
            CreatePolicyName = OnlineStorePermissions.Categories.Create;
            UpdatePolicyName = OnlineStorePermissions.Categories.Edit;
            DeletePolicyName = OnlineStorePermissions.Categories.Delete;
        }
     
        [Authorize(OnlineStorePermissions.Categories.Default)]
        public override async Task<PagedResultDto<CategoryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
          
            var totalCount = await _categoryRepository.GetCountAsync();

      
            var categories = await _categoryRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting ?? OnlineStore.OnlineStoreConsts.DefaultCategorySorting
            );

           
            var categoryDtos = ObjectMapper.Map<List<Category>, List<CategoryDto>>(categories);

         
            try
            {
                var productCounts = await _categoryRepository.GetProductCountsAsync();
                foreach (var dto in categoryDtos)
                {
                    dto.ProductCount = productCounts.ContainsKey(dto.Id) ? productCounts[dto.Id] : 0;
                }
            }
            catch (Exception)
            {
                              foreach (var dto in categoryDtos)
                {
                    dto.ProductCount = 0;
                }
            }

            return new PagedResultDto<CategoryDto>(totalCount, categoryDtos);
        }

     

        [Authorize(OnlineStorePermissions.Categories.Default)]
        public async Task<PagedResultDto<CategoryDto>> GetFilteredListAsync(GetCategoriesInput input)
        {
            _logger.LogInformation("CategoriesAppService.GetFilteredListAsync called. Input: IsActive={IsActive}, SearchTerm={SearchTerm}, SkipCount={SkipCount}, MaxResultCount={MaxResultCount}, Sorting={Sorting}",
                input.IsActive, input.SearchTerm, input.SkipCount, input.MaxResultCount, input.Sorting);
            
            try
            {
                    var totalCount = await _categoryRepository.GetCountAsync(
                    input.IsActive,
                    input.SearchTerm
                );
                             var categories = await _categoryRepository.GetListAsync(
                    input.SkipCount,
                    input.MaxResultCount,
                    input.Sorting ?? OnlineStore.OnlineStoreConsts.DefaultCategorySorting,
                    input.IsActive,
                    input.SearchTerm
                );
             
                var categoryDtos = ObjectMapper.Map<List<Category>, List<CategoryDto>>(categories);
                    try
                {
                    var productCounts = await _categoryRepository.GetProductCountsAsync();
                    _logger.LogInformation("Step 4 completed: Retrieved product counts for {Count} categories", productCounts?.Count ?? 0);
                    
                    foreach (var dto in categoryDtos)
                    {
                        dto.ProductCount = productCounts.ContainsKey(dto.Id) ? productCounts[dto.Id] : 0;
                    }
                    }
                catch (Exception ex)
                {      foreach (var dto in categoryDtos)
                    {
                        dto.ProductCount = 0;
                    }
                }

                _logger.LogInformation("GetFilteredListAsync completed successfully. Returning {TotalCount} total, {ItemsCount} items", 
                    totalCount, categoryDtos?.Count ?? 0);
                return new PagedResultDto<CategoryDto>(totalCount, categoryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFilteredListAsync FAILED. Exception Type: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}, InnerException: {InnerException}",
                    ex.GetType().Name, ex.Message, ex.StackTrace, ex.InnerException?.Message);
                throw;
            }
        }


    
        /// <summary>
        /// Gets a single category by ID
        /// </summary>
        [Authorize(OnlineStorePermissions.Categories.Default)]
        public override async Task<CategoryDto> GetAsync(int id)
        {
            var category = await _categoryRepository.GetAsync(id);
            var categoryDto = ObjectMapper.Map<Category, CategoryDto>(category);

            // Get product count for this specific category (optimized, with error handling)
            try
            {
                categoryDto.ProductCount = await _categoryRepository.GetProductCountAsync(id);
            }
            catch (Exception)
            {
                // If product count fails, set to 0 (don't fail the entire request)
                categoryDto.ProductCount = 0;
            }

            return categoryDto;
        }

      
        [AllowAnonymous]
        public async Task<List<CategoryDto>> GetActiveListAsync()
        {
            var categories = await _categoryRepository.GetActiveListAsync();
            var categoryDtos = ObjectMapper.Map<List<Category>, List<CategoryDto>>(categories);
            
            // Get product counts for each category (with error handling)
            try
            {
                var productCounts = await _categoryRepository.GetProductCountsAsync();
                foreach (var dto in categoryDtos)
                {
                    dto.ProductCount = productCounts.ContainsKey(dto.Id) ? productCounts[dto.Id] : 0;
                }
            }
            catch (Exception)
            {
                // If product counts fail, set all to 0 (don't fail the entire request)
                foreach (var dto in categoryDtos)
                {
                    dto.ProductCount = 0;
                }
            }
            
            return categoryDtos;
        }

      
        [Authorize(OnlineStorePermissions.Categories.Create)]
        public override async Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            // Generate next ID if input.Id is 0 (new category)
            int categoryId = input.Id;
            if (categoryId == 0)
            {
                // Get the next available ID from the repository
                categoryId = await _categoryRepository.GetNextIdAsync();
            }

            var category = await _categoryManager.CreateAsync(
                categoryId,
                input.NameAr,
                input.NameEn,
                input.DescriptionAr,
                input.DescriptionEn,
                input.IsActive,
                input.DisplayOrder
            );

            await _categoryRepository.InsertAsync(category);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Category, CategoryDto>(category);
        }

        
        [Authorize(OnlineStorePermissions.Categories.Edit)]
        public override async Task<CategoryDto> UpdateAsync(int id, CreateUpdateCategoryDto input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var category = await _categoryRepository.GetAsync(id);

         
            await _categoryManager.UpdateAsync(
                category,
                input.NameAr,
                input.NameEn,
                input.DescriptionAr,
                input.DescriptionEn,
                input.DisplayOrder
            );

            // Update IsActive separately (not in manager)
            if (input.IsActive)
                category.Activate();
            else
                category.Deactivate();

            await _categoryRepository.UpdateAsync(category);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Category, CategoryDto>(category);
        }

      
        [Authorize(OnlineStorePermissions.Categories.Delete)]
        public override async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetAsync(id);

          
            await _categoryManager.DeleteAsync(category);

            await _categoryRepository.DeleteAsync(category);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

    
        [Authorize(OnlineStorePermissions.Categories.Delete)]
        public async Task<bool> CanDeleteAsync(int id)
        {
            return await _categoryManager.CanDeleteAsync(id);
        }

      
        [Authorize(OnlineStorePermissions.Categories.Edit)]
        public async Task<CategoryDto> ChangeDisplayOrderAsync(int id, int newOrder)
        {
            var category = await _categoryRepository.GetAsync(id);
            _categoryManager.ChangeDisplayOrder(category, newOrder);
            await _categoryRepository.UpdateAsync(category);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Category, CategoryDto>(category);
        }

        [Authorize(OnlineStorePermissions.Categories.Edit)]
        public async Task<CategoryDto> ActivateAsync(int id)
        {
            var category = await _categoryRepository.GetAsync(id);
            _categoryManager.Activate(category);
            await _categoryRepository.UpdateAsync(category);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Category, CategoryDto>(category);
        }

        [Authorize(OnlineStorePermissions.Categories.Edit)]
        public async Task<CategoryDto> DeactivateAsync(int id)
        {
            var category = await _categoryRepository.GetAsync(id);
            _categoryManager.Deactivate(category);
            await _categoryRepository.UpdateAsync(category);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Category, CategoryDto>(category);
        }
    }
}