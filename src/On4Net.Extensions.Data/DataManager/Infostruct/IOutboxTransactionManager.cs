using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Data.DataManager.Infostruct;

public interface IOutboxTransactionManager : ITransactionManager
{
    Task RunAsync(Func<IDbConnection, IMessageOutbox, Task> action);

    Task RunAsync(Func<IDbConnection, IMessageOutbox, Task> action, CancellationToken cancellationToken);

    Task<T> RunAsync<T>(Func<IDbConnection, IMessageOutbox, Task<T>> action, CancellationToken cancellationToken);
}