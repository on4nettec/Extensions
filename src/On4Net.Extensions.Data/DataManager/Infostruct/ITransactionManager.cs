using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Data.DataManager.Infostruct;

public interface ITransactionManager
{
    Task RunAsync(Func<IDbConnection, Task> action);

    Task RunAsync(Func<IDbConnection, Task> action, CancellationToken cancellationToken);

    Task<T> RunAsync<T>(Func<IDbConnection, Task<T>> action, CancellationToken cancellationToken);
}
