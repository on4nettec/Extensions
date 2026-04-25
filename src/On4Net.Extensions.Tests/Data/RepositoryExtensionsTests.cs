using On4Net.Extensions.Data.DataManager;
using Xunit;

namespace On4Net.Extensions.Tests.Data;

/// <summary>
/// Tests for SQL fragment helpers on <see cref="RepositoryExtensions"/>.
/// </summary>
public sealed class RepositoryExtensionsTests
{
    [Fact]
    public void ToAndQuery_null_or_empty_returns_TRUE()
    {
        Assert.Equal("TRUE", ((List<string>?)null).ToAndQuery());
        Assert.Equal("TRUE", new List<string>().ToAndQuery());
    }

    [Fact]
    public void ToAndQuery_joins_parenthesized_parts_with_AND()
    {
        var parts = new List<string> { "a = 1", "b = 2" };
        var q = parts.ToAndQuery();
        Assert.Equal("(a = 1)AND(b = 2)", q);
    }

    [Fact]
    public void ToOrQuery_null_or_empty_wraps_TRUE()
    {
        Assert.Equal("(TRUE)", ((List<string>?)null).ToOrQuery());
        Assert.Equal("(TRUE)", new List<string>().ToOrQuery());
    }

    [Fact]
    public void ToOrQuery_joins_with_OR_inside_outer_parentheses()
    {
        var parts = new List<string> { "x", "y" };
        var q = parts.ToOrQuery();
        Assert.Equal("( x OR y )", q);
    }
}
