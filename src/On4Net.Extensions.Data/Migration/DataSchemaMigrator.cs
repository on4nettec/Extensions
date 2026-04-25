using DbUp.Builder;
using DbUp.Engine;
using DbUp;
using Npgsql;
using DbUp.Postgresql;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace On4Net.Extensions.Data.Migration;

public class DataSchemaMigrator
{
    private readonly DataOptions _dataOptions;

    private readonly ILogger<DataSchemaMigrator> _logger;

    public DataSchemaMigrator(ILogger<DataSchemaMigrator> logger, IOptions<DataOptions> dataOptions)
    {
        _logger = logger;
        _dataOptions = dataOptions.Value;
    }

    public void DropDatabase()
    {
        NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder(_dataOptions.ConnectionString);
        string database = npgsqlConnectionStringBuilder.Database;
        if (database != null && !database.StartsWith("test_"))
        {
            throw new InvalidOperationException("Can only drop test databases");
        }

        npgsqlConnectionStringBuilder.Database = "postgres";
        NpgsqlConnection.ClearPool(new NpgsqlConnection(_dataOptions.ConnectionString));
        using NpgsqlConnection npgsqlConnection = new NpgsqlConnection(npgsqlConnectionStringBuilder.ConnectionString);
        npgsqlConnection.Open();
        string cmdText = "drop database if exists \"" + database + "\";";
        using NpgsqlCommand npgsqlCommand = new NpgsqlCommand(cmdText, npgsqlConnection);
        npgsqlCommand.ExecuteNonQuery();
    }

    public void ResetDataInTestDatabase(params string[] tableNames)
    {
        NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder(_dataOptions.ConnectionString);
        string database = npgsqlConnectionStringBuilder.Database;
        if (database != null && !database.StartsWith("test_"))
        {
            throw new InvalidOperationException("Can only drop data in test databases");
        }

        if (tableNames == null || tableNames.Length == 0)
        {
            return;
        }

        using NpgsqlConnection npgsqlConnection = new NpgsqlConnection(_dataOptions.ConnectionString);
        npgsqlConnection.Open();
        foreach (string text in tableNames)
        {
            string cmdText = "delete from " + text + ";";
            using NpgsqlCommand npgsqlCommand = new NpgsqlCommand(cmdText, npgsqlConnection);
            npgsqlCommand.ExecuteNonQuery();
        }
    }

    public void UpdateSchemas<T>(string journalTableName = null)
    {
        EnsureDatabase.For.PostgresqlDatabase(_dataOptions.ConnectionString);
        UpgradeEngineBuilder upgradeEngineBuilder = DeployChanges.To.PostgresqlDatabase(_dataOptions.ConnectionString).LogScriptOutput().LogToConsole()
            .WithScriptsEmbeddedInAssembly(typeof(T).Assembly, (string x) => x.EndsWith(".sql"))
            .WithTransactionPerScript();
        upgradeEngineBuilder.Configure(delegate (UpgradeConfiguration c)
        {
            c.Journal = new PostgresqlTableJournal(() => c.ConnectionManager, () => c.Log, null, journalTableName ?? _dataOptions.JournalTable);
        });
        DatabaseUpgradeResult databaseUpgradeResult = upgradeEngineBuilder.Build().PerformUpgrade();
        if (!databaseUpgradeResult.Successful)
        {
            _logger.LogError(databaseUpgradeResult.Error,null);
            return;
        }

        foreach (SqlScript script in databaseUpgradeResult.Scripts)
        {
            _logger.LogInformation("{ScriptName}: Done", script.Name);
        }
    }
}