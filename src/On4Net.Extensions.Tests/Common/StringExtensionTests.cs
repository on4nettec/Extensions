using On4Net.Extensions.Common;
using Xunit;

namespace On4Net.Extensions.Tests.Common;

/// <summary>
/// Tests for <see cref="StringExtension"/> parsing and culture helpers.
/// </summary>
public sealed class StringExtensionTests
{
    [Theory]
    [InlineData(null, true, true)]
    [InlineData("", true, true)]
    [InlineData("  ", true, true)]
    [InlineData("  ", false, false)]
    [InlineData("x", true, false)]
    public void IsNullOrEmpty_matches_expected(string? value, bool whitespaceAsEmpty, bool expected)
    {
        Assert.Equal(expected, value.IsNullOrEmpty(whitespaceAsEmpty));
    }

    [Theory]
    [InlineData("a", true, true)]
    [InlineData("", true, false)]
    public void IsNotNullOrEmpty_is_negation(string value, bool whitespaceMode, bool expected)
    {
        Assert.Equal(expected, value.IsNotNullOrEmpty(whitespaceMode));
    }

    [Fact]
    public void Add_Zero_Before_left_pads_to_length()
    {
        Assert.Equal("0012", "12".Add_Zero_Before(4));
    }

    [Fact]
    public void Add_Zero_After_right_pads_to_length()
    {
        Assert.Equal("1200", "12".Add_Zero_After(4));
    }

    [Fact]
    public void Add_SpaceAfter_right_pads_with_spaces()
    {
        Assert.Equal("ab  ", "ab".Add_SpaceAfter(4));
    }

    [Fact]
    public void ToDecimal_and_nullable_parse_numbers()
    {
        Assert.Equal(1.5m, "1.5".ToDecimal());
        Assert.Equal(1.5m, "1.5".ToDecimalNull());
        Assert.Null("bad".ToDecimalNull());
    }

    [Fact]
    public void ToInt32_and_nullable()
    {
        Assert.Equal(7, "7".ToInt32());
        Assert.Null("x".ToInt32Null());
    }

    [Fact]
    public void ToInt16_and_Byte_variants()
    {
        Assert.Equal((short)9, "9".ToInt16());
        Assert.Null("x".ToInt16Null());
        Assert.Equal((byte)2, "2".ToByte());
        Assert.Null("999".ToByteNull());
    }

    [Fact]
    public void ToInt64_and_Double_Float()
    {
        Assert.Equal(100L, "100".ToInt64());
        Assert.Null("z".ToInt64Null());
        Assert.Equal(2.5, "2.5".ToDouble());
        Assert.Null("z".ToDoubleNull());
        Assert.Equal(1.25f, "1.25".ToFloat());
        Assert.Null("z".ToFloatNull());
    }

    [Fact]
    public void ToGuid_parses_or_empty()
    {
        var g = Guid.NewGuid();
        Assert.Equal(g, g.ToString().ToGuid());
        Assert.Equal(Guid.Empty, "not-a-guid".ToGuid());
    }

    [Fact]
    public void IsRtl_fa_IR_is_rtl()
    {
        Assert.True("fa-IR".IsRtl());
    }

    [Fact]
    public void GetDirection_returns_rtl_or_ltr()
    {
        Assert.Equal("rtl", "fa-IR".GetDirection());
        Assert.Equal("ltr", "en-US".GetDirection());
    }

    [Fact]
    public void GetLanguageCulture_returns_first_segment()
    {
        Assert.Equal("en", "en-US".GetLanguageCulture());
    }

    [Fact]
    public void GetCountryCulture_returns_second_segment()
    {
        Assert.Equal("US", "en-US".GetCountryCulture());
    }

    [Fact]
    public void Culture_helpers_throw_on_null()
    {
        string? n = null;
        Assert.Throws<ArgumentNullException>(() => n!.IsRtl());
        Assert.Throws<ArgumentNullException>(() => n!.GetDirection());
        Assert.Throws<ArgumentNullException>(() => n!.GetLanguageCulture());
        Assert.Throws<ArgumentNullException>(() => n!.GetCountryCulture());
    }
}
