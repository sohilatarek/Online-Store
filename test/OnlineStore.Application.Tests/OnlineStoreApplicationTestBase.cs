using Volo.Abp.Modularity;

namespace OnlineStore;

public abstract class OnlineStoreApplicationTestBase<TStartupModule> : OnlineStoreTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
