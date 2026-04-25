using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using On4Net.Extensions.Data.DataManager.Infostruct;

namespace On4Net.Extensions.Data.DataManager;
public class TransactionManager : ITransactionManager
{
    private readonly string _connectionString;

    private readonly DbProviderFactory _providerFactory;

    public TransactionManager(DbProviderFactory providerFactory, string connectionString)
    {
        _providerFactory = providerFactory;
        _connectionString = connectionString;
    }

    public Task RunAsync(Func<IDbConnection, Task> action)
    {
        return RunAsync(action, CancellationToken.None);
    }

    public async Task RunAsync(Func<IDbConnection, Task> action, CancellationToken cancellationToken)
    {
        await using DbConnection connection = _providerFactory.CreateConnection();
        if (connection == null)
        {
            throw new InvalidOperationException("Unable to create database connection.");
        }

        DbTransaction transaction = null;
        try
        {
            connection.ConnectionString = _connectionString;
            await connection.OpenAsync(cancellationToken);
            transaction = await connection.BeginTransactionAsync(cancellationToken);
            await action(connection);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }

    public async Task<T> RunAsync<T>(Func<IDbConnection, Task<T>> action, CancellationToken cancellationToken)
    {
        T result2;
        await using (DbConnection connection = _providerFactory.CreateConnection())
        {
            if (connection == null)
            {
                throw new InvalidOperationException("Unable to create database connection.");
            }

            DbTransaction transaction = null;
            T result;
            try
            {
                connection.ConnectionString = _connectionString;
                await connection.OpenAsync(cancellationToken);
                transaction = await connection.BeginTransactionAsync(cancellationToken);
                result = await action(connection);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }

                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }

            result2 = result;
        }

        return result2;
    }
}
