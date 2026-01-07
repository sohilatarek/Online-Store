using Volo.Abp.Identity;

namespace OnlineStore
{
    public static class OnlineStoreConsts
    {
        public const int GeneralTextMaxLength = 500; 
        public const int DescriptionTextMaxLength = 2000; 
        public const string AdminEmailDefaultValue = IdentityDataSeedContributor.AdminEmailDefaultValue;
        public const string AdminPasswordDefaultValue = IdentityDataSeedContributor.AdminPasswordDefaultValue;
        
        // Business Constants
        public const int DefaultLowStockThreshold = 10;
        public const decimal MaxPrice = 999999.99m;
        public const int MaxStockQuantity = 1000000;

        // Default Sorting
        public const string DefaultProductSorting = "CreationTime desc";
        public const string DefaultCategorySorting = "DisplayOrder asc";
        
        // Cache Keys (for reference)
        public const string ProductCacheKeyPrefix = "Products:";
        public const string CategoryCacheKeyPrefix = "Categories:";
    }
}