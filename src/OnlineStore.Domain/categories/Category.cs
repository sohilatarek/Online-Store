using System;
using OnlineStore;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace OnlineStore.Categories
{
    /// <summary>
    /// Category entity - represents product categories in the online store
    /// Inherits full audit properties + multi-tenancy support
    /// </summary>
    public class Category : FullAuditedAggregateRoot<int>, IMultiTenant
    {
      
        public Guid? TenantId { get; set; }

        public string NameAr { get; set; }

        public string NameEn { get; set; }

        public string DescriptionAr { get; set; }

        public string DescriptionEn { get; set; }

        /// <summary>
        /// Indicates if the category is active and can be used
        /// Inactive categories are hidden but not deleted
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Display order for sorting categories
        /// Lower numbers appear first (e.g., 1, 2, 3...)
        /// </summary>
        public int DisplayOrder { get; set; }

        protected Category()
        {
          
        }

        public Category(
            int id,
            string nameAr,
            string nameEn,
            string descriptionAr,
            string descriptionEn,
            bool isActive = true,
            int displayOrder = 0) : base(id)
        {
            NameAr = nameAr;
            NameEn = nameEn;
            DescriptionAr = descriptionAr;
            DescriptionEn = descriptionEn;
            IsActive = isActive;
            DisplayOrder = displayOrder;
        }

        // ==========================================
        // BUSINESS METHODS
        // ==========================================

        /// <summary>
        /// Activates the category
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }

        /// <summary>
        /// Deactivates the category 
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Updates the display order
        /// </summary>
        /// <param name="newOrder">New display order (must be positive)</param>
        public void SetDisplayOrder(int newOrder)
        {
            if (newOrder < 0)
            {
                throw new BusinessException(OnlineStoreErrorCodes.InvalidDisplayOrder)
                    .WithData("DisplayOrder", newOrder)
                    .WithData("Message", "Display order must be a positive number.");
            }

            DisplayOrder = newOrder;
        }

        /// <summary>
        /// Updates category information
        /// </summary>
        public void Update(
            string nameAr,
            string nameEn,
            string descriptionAr,
            string descriptionEn,
            int displayOrder)
        {
            NameAr = Check.NotNullOrWhiteSpace(nameAr, nameof(nameAr));
            NameEn = Check.NotNullOrWhiteSpace(nameEn, nameof(nameEn));
            DescriptionAr = Check.NotNullOrWhiteSpace(descriptionAr, nameof(descriptionAr));
            DescriptionEn = Check.NotNullOrWhiteSpace(descriptionEn, nameof(descriptionEn));
            SetDisplayOrder(displayOrder);
        }
    }
}