using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using On4Net.Extensions.Data;
using On4Net.Extensions.Data.DataManager.Infostruct;
using On4Net.Extensions.Data.Migration;
using Xunit;

namespace On4Net.Extensions.Tests.Data;

/// <summary>
/// Verifies that <see cref="Configuration.ConfigureDataServices{T}"/> registers expected service types.
/// </summary>
public sealed class ConfigurationExtensionsTests
{
    [Fact]
    public void ConfigureDataServices_registers_core_data_services()
    {
        var services = new ServiceCollection();
        services.AddLogging(b => b.AddDebug());
        services.AddOptions<DataOptions>().Configure(o =>
        {
            o.Address = "127.0.0.1";
            o.Name = "test_db";
            o.UserName = "u";
            o.Password = "p";
        });

        services.ConfigureDataServices<ConfigurationExtensionsTests>();

        var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetService<DataSchemaMigrator>());
        Assert.NotNull(provider.GetService<ITransactionManager>());
        Assert.NotNull(provider.GetService<IOutboxTransactionManager>());
    }
}
