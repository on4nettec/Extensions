using On4Net.Extensions.Data;
using Xunit;

namespace On4Net.Extensions.Tests.Data;

/// <summary>
/// Tests for <see cref="DataOptions"/> connection string composition.
/// </summary>
public sealed class DataOptionsTests
{
    [Fact]
    public void ConnectionString_includes_server_port_database_user_timeout()
    {
        var o = new DataOptions
        {
            Address = "localhost",
            Port = "5433",
            Name = "appdb",
            UserName = "u",
            Password = "p",
            CommandTimeout = 15
        };
        var cs = o.ConnectionString;
        Assert.Contains("Server=localhost", cs);
        Assert.Contains("Port=5433", cs);
        Assert.Contains("Database=appdb", cs);
        Assert.Contains("User Id=u", cs);
        Assert.Contains("Password=p", cs);
        Assert.Contains("Command Timeout=15", cs);
    }

    [Fact]
    public void JournalTable_defaults_to_schema_version()
    {
        var o = new DataOptions();
        Assert.Equal("schema_version", o.JournalTable);
    }

    [Fact]
    public void Port_defaults_to_5432()
    {
        var o = new DataOptions();
        Assert.Equal("5432", o.Port);
    }
}
