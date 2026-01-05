using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace OnlineStore.Categories
{
    /// <summary>
    /// Domain service for Category business logic
    /// Enforces business rules and validations
    /// </summary>
    public class CategoryManager : DomainService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryManager(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Creates a new category with business rule validation
        /// </summary>
        public async Task<Category> CreateAsync(
            int id,
            string nameAr,
            string nameEn,
            string descriptionAr,
            string descriptionEn,
            bool isActive = true,
            int displayOrder = 0)
        {
            // Business Rule: Category name must be unique per tenant
            await CheckNameUniquenessAsync(nameEn, null);

            // Business Rule: Display order must be positive
            ValidateDisplayOrder(displayOrder);

            // Create the category
            var category = new Category(
                id,
                nameAr,
                nameEn,
                descriptionAr,
                descriptionEn,
                isActive,
                displayOrder
            );

            return category;
        }

        /// <summary>
        /// Updates an existing category with validation
        /// </summary>
        public async Task UpdateAsync(
            Category category,
            string nameAr,
            string nameEn,
            string descriptionAr,
            string descriptionEn,
            int displayOrder)
        {
            // Business Rule: New name must be unique (excluding current category)
            if (category.NameEn != nameEn)
            {
                await CheckNameUniquenessAsync(nameEn, category.Id);
            }

            // Business Rule: Display order must be positive
            ValidateDisplayOrder(displayOrder);

            // Update the category
            category.Update(nameAr, nameEn, descriptionAr, descriptionEn, displayOrder);
        }

        /// <summary>
        /// Deletes a category (business rules apply)
        /// </summary>
        public async Task DeleteAsync(Category category)
        {
            // Business Rule: Cannot delete category if it has active products
            var hasProducts = await _categoryRepository.HasProductsAsync(category.Id);

            if (hasProducts)
            {
                throw new BusinessException("OnlineStore:002")
                    .WithData("CategoryId", category.Id)
                    .WithData("CategoryName", category.NameEn)
                    .WithData("Message", "Cannot delete category that has products. Please reassign or delete products first.");
            }

            // Soft delete is handled by ABP framework automatically
        }

        /// <summary>
        /// Checks if a category can be deleted
        /// </summary>
        public async Task<bool> CanDeleteAsync(int categoryId)
        {
            return !await _categoryRepository.HasProductsAsync(categoryId);
        }

        /// <summary>
        /// Changes the display order of a category
        /// </summary>
        public void ChangeDisplayOrder(Category category, int newOrder)
        {
            ValidateDisplayOrder(newOrder);
            category.SetDisplayOrder(newOrder);
        }

        /// <summary>
        /// Activates a category
        /// </summary>
        public void Activate(Category category)
        {
            category.Activate();
        }

        /// <summary>
        /// Deactivates a category
        /// </summary>
        public void Deactivate(Category category)
        {
            category.Deactivate();
        }

        // ==========================================
        // PRIVATE HELPER METHODS
        // ==========================================

        /// <summary>
        /// Ensures category name is unique within tenant
        /// </summary>
        private async Task CheckNameUniquenessAsync(string name, int? excludeId = null)
        {
            var exists = await _categoryRepository.IsNameUniqueAsync(name, excludeId);

            if (!exists)
            {
                throw new BusinessException("OnlineStore:001")
                    .WithData("Name", name)
                    .WithData("Message", $"Category with name '{name}' already exists.");
            }
        }

        /// <summary>
        /// Validates display order is positive
        /// </summary>
        private void ValidateDisplayOrder(int displayOrder)
        {
            if (displayOrder < 0)
            {
                throw new BusinessException("OnlineStore:007")
                    .WithData("DisplayOrder", displayOrder)
                    .WithData("Message", "Display order must be a positive number.");
            }
        }
    }
}