using Volo.Abp.Modularity;

namespace OnlineStore;

[DependsOn(
    typeof(OnlineStoreDomainModule),
    typeof(OnlineStoreTestBaseModule)
)]
public class OnlineStoreDomainTestModule : AbpModule
{

}
