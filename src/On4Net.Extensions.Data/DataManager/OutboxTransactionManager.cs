using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using On4Net.Extensions.Data.DataManager.Infostruct;

using Microsoft.Extensions.DependencyInjection;

namespace On4Net.Extensions.Data.DataManager;

public class OutboxTransactionManager : TransactionManager, IOutboxTransactionManager, ITransactionManager
{
    private readonly string _connectionString;

    private readonly DbProviderFactory _providerFactory;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OutboxTransactionManager(DbProviderFactory providerFactory, string connectionString, IServiceScopeFactory serviceScopeFactory)
        : base(providerFactory, connectionString)
    {
        _providerFactory = providerFactory;
        _connectionString = connectionString;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task RunAsync(Func<IDbConnection, IMessageOutbox, Task> action)
    {
        return RunAsync(action, CancellationToken.None);
    }

    public async Task RunAsync(Func<IDbConnection, IMessageOutbox, Task> action, CancellationToken cancellationToken)
    {
        await using DbConnection connection = _providerFactory.CreateConnection();
        if (connection == null)
        {
            throw new InvalidOperationException("Unable to create database connection.");
        }

        DbTransaction transaction = null;
        IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
        IMessageOutbox messageOutbox = serviceScope.ServiceProvider.GetRequiredService<IMessageOutbox>().On(connection);
        try
        {
            connection.ConnectionString = _connectionString;
            await connection.OpenAsync(cancellationToken);
            transaction = await connection.BeginTransactionAsync(cancellationToken);
            await action(connection, messageOutbox);
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

        if (messageOutbox.HasMessage)
        {
            await RunAsync(async delegate (IDbConnection c)
            {
                await messageOutbox.PublishMessagesAsync(c, cancellationToken);
            }, cancellationToken);
        }
    }

    public async Task<T> RunAsync<T>(Func<IDbConnection, IMessageOutbox, Task<T>> action, CancellationToken cancellationToken)
    {
        T result2;
        await using (DbConnection connection = _providerFactory.CreateConnection())
        {
            if (connection == null)
            {
                throw new InvalidOperationException("Unable to create database connection.");
            }

            DbTransaction transaction = null;
            IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
            IMessageOutbox messageOutbox = serviceScope.ServiceProvider.GetRequiredService<IMessageOutbox>().On(connection);
            T result;
            try
            {
                connection.ConnectionString = _connectionString;
                await connection.OpenAsync(cancellationToken);
                transaction = await connection.BeginTransactionAsync(cancellationToken);
                result = await action(connection, messageOutbox);
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

            if (messageOutbox.HasMessage)
            {
                await RunAsync(async delegate (IDbConnection c)
                {
                    await messageOutbox.PublishMessagesAsync(c, cancellationToken);
                }, cancellationToken);
            }

            result2 = result;
        }

        return result2;
    }
}