using System.Threading.Tasks;

namespace OnlineStore.Data;

public interface IOnlineStoreDbSchemaMigrator
{
    Task MigrateAsync();
}
