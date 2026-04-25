using Xunit;

namespace On4Net.Extensions.Tests.Firebase;

/// <summary>
/// Tests for <see cref="FirebaseOptions"/> (global namespace in the product assembly).
/// </summary>
public sealed class FirebaseOptionsTests
{
    [Fact]
    public void Url_points_at_identity_toolkit_v1()
    {
        var o = new FirebaseOptions { ApiKey = "k", Audience = "my-project" };
        Assert.Equal("https://identitytoolkit.googleapis.com/v1/", o.Url);
    }

    [Fact]
    public void Validator_uses_securetoken_google_audience()
    {
        var o = new FirebaseOptions { ApiKey = "k", Audience = "proj" };
        Assert.Equal("https://securetoken.google.com/proj", o.Validator);
    }
}
