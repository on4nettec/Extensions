using On4Net.Extensions.Common;
using Xunit;

namespace On4Net.Extensions.Tests.Common;

/// <summary>
/// Tests for <see cref="RegExExtesion"/> pattern constants and matchers.
/// </summary>
public sealed class RegExExtensionTests
{
    [Fact]
    public void IsMatch_Digit_accepts_digit_prefix_and_rejects_non_digit_prefix()
    {
        Assert.True("123".IsMatch_Digit());
        // `^\d+` is matched from the start of the string; trailing letters still yield a match for the digit prefix.
        Assert.False("a12".IsMatch_Digit());
    }

    [Fact]
    public void IsMatch_Email_accepts_simple_email()
    {
        Assert.True("user@example.com".IsMatch_Email());
    }

    [Fact]
    public void IsMatch_IPAddress_finds_ipv4_like_sequence()
    {
        Assert.True("192.168.0.1".IsMatch_IPAddress());
    }

    [Fact]
    public void IsMatch_Password_requires_mixed_case_and_digit()
    {
        Assert.True("Aa1xxxx".IsMatch_Password());
        Assert.False("aaaaaa".IsMatch_Password());
    }

    [Theory]
    [InlineData(null, "^x", false)]
    [InlineData("", "^x", false)]
    [InlineData("abc", "^a", true)]
    public void IsExpressionValid_respects_null_and_pattern(string? input, string pattern, bool expected)
    {
        Assert.Equal(expected, input.IsExpressionValid(pattern));
    }
}
