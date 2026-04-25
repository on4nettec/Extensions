using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using On4Net.Extensions.Data.DataManager;
using On4Net.Extensions.Data.DataManager.Infostruct;
using On4Net.Extensions.Data.Migration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Npgsql;

namespace On4Net.Extensions.Data;

public static class Configuration
{
    public static IServiceCollection ConfigureDataServices<T>(this IServiceCollection services )
    { 
        return services.AddSingleton<DataSchemaMigrator, DataSchemaMigrator>().AddHostedService < SchemaMigratorService <T>>()
            .AddSingleton<DbProviderFactory, NpgsqlFactory>((IServiceProvider _) => NpgsqlFactory.Instance)
              .AddScoped<ITransactionManager, TransactionManager>(TransactionManagerFactory)
              .AddTransient<IOutboxTransactionManager, OutboxTransactionManager>(TransactionOutboxManagerFactory); 

    } 
    private static TransactionManager TransactionManagerFactory(IServiceProvider services)
    {
        return new TransactionManager(services.GetService<DbProviderFactory>(), services.GetService<IOptions<DataOptions>>().Value.ConnectionString);
    }

    private static OutboxTransactionManager TransactionOutboxManagerFactory(IServiceProvider services)
    {
        return new OutboxTransactionManager(services.GetService<DbProviderFactory>(), services.GetService<IOptions<DataOptions>>().Value.ConnectionString, services.GetService<IServiceScopeFactory>());
    }

}
