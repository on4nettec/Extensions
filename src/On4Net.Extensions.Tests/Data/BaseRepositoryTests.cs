using Moq;
using On4Net.Extensions.Data.DataManager.Infostruct;
using On4Net.Extensions.Data.Model.Request;
using Xunit;

namespace On4Net.Extensions.Tests.Data;

/// <summary>
/// Tests for static and protected helpers exposed via <see cref="TestRepository"/>.
/// </summary>
public sealed class BaseRepositoryTests
{
    [Fact]
    public void GenerateNewId_returns_non_empty_guid()
    {
        var id = TestRepository.ExposeGenerateNewId();
        Assert.NotEqual(Guid.Empty, id);
    }

    [Theory]
    [InlineData(null, 1)]
    [InlineData(0, 1)]
    [InlineData(5, 6)]
    public void GenerateNewVersion_increments_or_starts_at_one(int? old, int expected)
    {
        Assert.Equal(expected, TestRepository.ExposeGenerateNewVersion(old));
    }

    [Fact]
    public void GetSortCommand_empty_or_null_returns_empty_string()
    {
        var mock = new Mock<IOutboxTransactionManager>();
        var repo = new TestRepository(mock.Object, () => DateTime.UtcNow);
        Assert.Equal(string.Empty, repo.ExposeGetSortCommand(null));
        Assert.Equal(string.Empty, repo.ExposeGetSortCommand(new Dictionary<string, SortDirection>()));
    }

    [Fact]
    public void GetSortCommand_maps_ID_to_quoted_column_and_direction()
    {
        var mock = new Mock<IOutboxTransactionManager>();
        var repo = new TestRepository(mock.Object, () => DateTime.UtcNow);
        var sql = repo.ExposeGetSortCommand(new Dictionary<string, SortDirection>(StringComparer.OrdinalIgnoreCase)
        {
            ["ID"] = SortDirection.DESC
        });
        Assert.Equal("ORDER BY \"ID\" DESC", sql);
    }
}
