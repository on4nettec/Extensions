using On4Net.Extensions.Common;
using Xunit;

namespace On4Net.Extensions.Tests.Common;

/// <summary>
/// Tests for the <see cref="Culture"/> value type presets and derived culture strings.
/// </summary>
public sealed class CultureTests
{
    [Fact]
    public void English_preset_has_en_US_for_current_and_default()
    {
        var c = Culture.English;
        Assert.Equal("en-US", c.CurrentCulture);
        Assert.Equal("en-US", c.DefaultCulture);
    }

    [Fact]
    public void CurrentLanguage_and_CurrentCountry_use_string_extensions()
    {
        var c = new Culture { CurrentCulture = "en-US" };
        Assert.Equal("en", c.CurrentLanguage);
        Assert.Equal("US", c.CurrentCountry);
    }
}
