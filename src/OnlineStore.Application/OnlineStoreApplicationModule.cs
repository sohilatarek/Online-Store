using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineStore.Categories;
using OnlineStore.Products;
using System.Collections.Generic;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FluentValidation;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace OnlineStore;

[DependsOn(
    typeof(OnlineStoreDomainModule),
    typeof(OnlineStoreApplicationContractsModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpFluentValidationModule)
)]
public class OnlineStoreApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;

        // Configure AutoMapper
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<OnlineStoreApplicationModule>();
        });

        // ============================================
        // SERVICE REGISTRATION WITH DECORATOR PATTERN
        // ============================================

        // Register the REAL (inner) application services as concrete types
        services.AddTransient<CategoriesAppService>();
        services.AddTransient<ProductsAppService>();

        // Register the INTERFACE with CACHED decorator using factory
        services.AddTransient<ICategoriesAppService>(provider =>
        {
            // Resolve inner service directly (concrete type)
            var innerService = provider.GetRequiredService<CategoriesAppService>();

            // Resolve dependencies for cached wrapper
            var listCache = provider.GetRequiredService<IDistributedCache<List<CategoryDto>>>();
            var singleCache = provider.GetRequiredService<IDistributedCache<CategoryDto>>();
            var currentTenant = provider.GetRequiredService<ICurrentTenant>();
            var logger = provider.GetRequiredService<ILogger<CachedCategoryAppService>>();

            // Return cached wrapper that wraps the real service
            return new CachedCategoryAppService(innerService, listCache, singleCache, currentTenant, logger);
        });

        services.AddTransient<IProductsAppService>(provider =>
        {
            // Resolve inner service directly (concrete type)
            var innerService = provider.GetRequiredService<ProductsAppService>();

            // Resolve dependencies for cached wrapper
            var listCache = provider.GetRequiredService<IDistributedCache<List<ProductDto>>>();
            var singleCache = provider.GetRequiredService<IDistributedCache<ProductDto>>();
            var currentTenant = provider.GetRequiredService<ICurrentTenant>();

            // Return cached wrapper that wraps the real service
            return new CachedProductAppService(innerService, listCache, singleCache, currentTenant);
        });

    }
}