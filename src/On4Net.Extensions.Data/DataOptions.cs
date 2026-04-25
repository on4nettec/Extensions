using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Data;

public class DataOptions
{
    public string Address { get; set; }

    public string Port { get; set; } = "5432";


    public string UserName { get; set; }

    public string Password { get; set; }

    public string Name { get; set; }

    public string JournalTable { get; set; } = "schema_version";


    public int CommandTimeout { get; set; } = 30;


    public string ConnectionString => $"Server={Address};Port={Port};Database={Name};User Id={UserName};Password={Password};Command Timeout={CommandTimeout};";
}