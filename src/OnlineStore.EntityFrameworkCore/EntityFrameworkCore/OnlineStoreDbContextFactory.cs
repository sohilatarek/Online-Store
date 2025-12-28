using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OnlineStore.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class OnlineStoreDbContextFactory : IDesignTimeDbContextFactory<OnlineStoreDbContext>
{
    public OnlineStoreDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        OnlineStoreEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<OnlineStoreDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new OnlineStoreDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../OnlineStore.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
