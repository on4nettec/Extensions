using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using On4Net.Extensions.Identity.Firebase.Services;
using On4Net.Extensions.Identity.Firebase.Services.Infostruct;
using Xunit;

namespace On4Net.Extensions.Tests.Firebase;

/// <summary>
/// Tests for <see cref="ServiceConfiguration.RegisterFirebaseAuth"/> DI wiring (no real Firebase app).
/// </summary>
public sealed class ServiceConfigurationTests
{
    [Fact]
    public void RegisterFirebaseAuth_registers_firebase_services_and_auth()
    {
        const string json = """
            {
              "FirebaseOptions": {
                "ApiKey": "test-key",
                "Audience": "demo-audience"
              }
            }
            """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var configuration = new ConfigurationBuilder().AddJsonStream(stream).Build();

        var services = new ServiceCollection();
        services.RegisterFirebaseAuth(configuration, isCreateFirebaseApp: false);

        var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetService<IFirebaseServices>());
    }
}
