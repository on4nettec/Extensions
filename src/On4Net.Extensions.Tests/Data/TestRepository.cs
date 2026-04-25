using On4Net.Extensions.Data.DataManager;
using On4Net.Extensions.Data.DataManager.Infostruct;
using On4Net.Extensions.Data.Model.Request;

namespace On4Net.Extensions.Tests.Data;

/// <summary>
/// Minimal concrete repository used to exercise protected helpers on <see cref="BaseRepository"/>.
/// </summary>
internal sealed class TestRepository : BaseRepository
{
    public TestRepository(IOutboxTransactionManager outboxTransactionManager, Func<DateTime> dateTimeProvider)
        : base(outboxTransactionManager, dateTimeProvider)
    {
    }

    protected override string TableName => "test_table";

    protected override string LocalizationTableName => "test_localization";

    public string ExposeGetSortCommand(Dictionary<string, SortDirection>? orders) => GetSortCommand(orders!);

    public static Guid ExposeGenerateNewId() => GenerateNewId();

    public static int ExposeGenerateNewVersion(int? oldVersion) => GenerateNewVersion(oldVersion);
}
