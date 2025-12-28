using Volo.Abp.Modularity;

namespace OnlineStore;

/* Inherit from this class for your domain layer tests. */
public abstract class OnlineStoreDomainTestBase<TStartupModule> : OnlineStoreTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
