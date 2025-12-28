using OnlineStore.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace OnlineStore.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(OnlineStoreEntityFrameworkCoreModule),
    typeof(OnlineStoreApplicationContractsModule)
)]
public class OnlineStoreDbMigratorModule : AbpModule
{
}
