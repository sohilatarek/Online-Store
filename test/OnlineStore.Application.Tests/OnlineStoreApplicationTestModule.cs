using Volo.Abp.Modularity;

namespace OnlineStore;

[DependsOn(
    typeof(OnlineStoreApplicationModule),
    typeof(OnlineStoreDomainTestModule)
)]
public class OnlineStoreApplicationTestModule : AbpModule
{

}
