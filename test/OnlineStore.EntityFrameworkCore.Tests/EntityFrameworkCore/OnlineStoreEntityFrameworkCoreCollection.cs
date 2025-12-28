using Xunit;

namespace OnlineStore.EntityFrameworkCore;

[CollectionDefinition(OnlineStoreTestConsts.CollectionDefinitionName)]
public class OnlineStoreEntityFrameworkCoreCollection : ICollectionFixture<OnlineStoreEntityFrameworkCoreFixture>
{

}
