using On4Net.Extensions.Identity.Firebase.Auth;
using Xunit;

namespace On4Net.Extensions.Tests.Firebase;

/// <summary>
/// Tests for <see cref="AuthorizationAttribute"/> filter wiring.
/// </summary>
public sealed class AuthorizationAttributeTests
{
    [Fact]
    public void Constructor_passes_policies_as_single_filter_argument()
    {
        var attr = new AuthorizationAttribute("Admin", "Support");
        Assert.Equal(typeof(AuthorizationFilter), attr.ImplementationType);
        var args = attr.Arguments ?? Array.Empty<object?>();
        Assert.Single(args);
        var policies = Assert.IsType<string[]>(args[0]!);
        Assert.Equal(new[] { "Admin", "Support" }, policies);
    }
}
