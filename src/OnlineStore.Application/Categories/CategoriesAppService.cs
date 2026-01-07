using Microsoft.AspNetCore.Authorization;
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

        public CategoriesAppService(
            ICategoryRepository repository,
            CategoryManager categoryManager) : base(repository)
        {
            _categoryRepository = repository;
            _categoryManager = categoryManager;

            // Set default permissions
            GetPolicyName = OnlineStorePermissions.Categories.Default;
            GetListPolicyName = OnlineStorePermissions.Categories.Default;
            CreatePolicyName = OnlineStorePermissions.Categories.Create;
            UpdatePolicyName = OnlineStorePermissions.Categories.Edit;
            DeletePolicyName = OnlineStorePermissions.Categories.Delete;
        }

     
        [Authorize(OnlineStorePermissions.Categories.Default)]
        public async Task<PagedResultDto<CategoryDto>> GetFilteredListAsync(GetCategoriesInput input)
        {
            // Get total count with filters
            var totalCount = await _categoryRepository.GetCountAsync(
                input.IsActive,
                input.SearchTerm
            );

            // Get categories with pagination and filters
            var categories = await _categoryRepository.GetListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting ?? OnlineStore.OnlineStoreConsts.DefaultCategorySorting,
                input.IsActive,
                input.SearchTerm
            );

            // Get product counts for each category
            var productCounts = await _categoryRepository.GetProductCountsAsync();

            // Map to DTOs and populate product counts
            var categoryDtos = ObjectMapper.Map<List<Category>, List<CategoryDto>>(categories);
            foreach (var dto in categoryDtos)
            {
                dto.ProductCount = productCounts.ContainsKey(dto.Id) ? productCounts[dto.Id] : 0;
            }

            return new PagedResultDto<CategoryDto>(totalCount, categoryDtos);
        }


    
        /// <summary>
        /// Gets a single category by ID
        /// </summary>
        [Authorize(OnlineStorePermissions.Categories.Default)]
        public override async Task<CategoryDto> GetAsync(int id)
        {
            var category = await _categoryRepository.GetAsync(id);
            var categoryDto = ObjectMapper.Map<Category, CategoryDto>(category);

            // Get product count for this specific category (optimized)
            categoryDto.ProductCount = await _categoryRepository.GetProductCountAsync(id);

            return categoryDto;
        }

      
        [AllowAnonymous]
        public async Task<List<CategoryDto>> GetActiveListAsync()
        {
            var categories = await _categoryRepository.GetActiveListAsync();
            return ObjectMapper.Map<List<Category>, List<CategoryDto>>(categories);
        }

      
        [Authorize(OnlineStorePermissions.Categories.Create)]
        public override async Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

           
            var category = await _categoryManager.CreateAsync(
                input.Id,
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