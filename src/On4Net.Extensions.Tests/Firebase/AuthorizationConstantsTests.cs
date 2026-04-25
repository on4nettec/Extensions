using On4Net.Extensions.Identity.Firebase.Auth;
using Xunit;

namespace On4Net.Extensions.Tests.Firebase;

/// <summary>
/// Smoke tests for <see cref="AuthorizationConstants"/> role name strings.
/// </summary>
public sealed class AuthorizationConstantsTests
{
    [Fact]
    public void Role_constants_are_non_empty()
    {
        Assert.False(string.IsNullOrWhiteSpace(AuthorizationConstants.User));
        Assert.False(string.IsNullOrWhiteSpace(AuthorizationConstants.Admin));
        Assert.False(string.IsNullOrWhiteSpace(AuthorizationConstants.SupperAdmin));
        Assert.False(string.IsNullOrWhiteSpace(AuthorizationConstants.Support));
        Assert.False(string.IsNullOrWhiteSpace(AuthorizationConstants.SupperSupport));
    }
}
