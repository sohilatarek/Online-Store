using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.AutoMapper;  // ✅ Add this
using Volo.Abp.Mapperly;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.TenantManagement;
using Volo.Abp.FluentValidation;

namespace OnlineStore;

[DependsOn(
    typeof(OnlineStoreDomainModule),
    typeof(OnlineStoreApplicationContractsModule),
    typeof(AbpAutoMapperModule),  // ✅ Add this dependency
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
        // Configure AutoMapper
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<OnlineStoreApplicationModule>();
        });
    }
}