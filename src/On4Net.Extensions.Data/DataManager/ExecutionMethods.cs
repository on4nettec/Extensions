using System.Data;
using System.Linq.Expressions;
using Dapper;

using On4Net.Extensions.Data.DataManager.Infostruct;

namespace On4Net.Extensions.Data.DataManager;


public abstract class ExecutionMethods
{
    internal protected IOutboxTransactionManager _outboxTransactionManager;
    internal protected readonly Func<DateTime> _dateTimeProvider;

    public ExecutionMethods(IOutboxTransactionManager outboxTransactionManager, Func<DateTime> dateTimeProvider)
    {
        _outboxTransactionManager = outboxTransactionManager;
        _dateTimeProvider = dateTimeProvider;
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    internal protected async Task<int> ExecuteAsync(
        IDbConnection
        db,
        string query,
        object model,
        CancellationToken cancellationToken = default) =>
          await db.ExecuteAsync(new CommandDefinition(
                query,
                model,
                cancellationToken: cancellationToken
            ));


    internal protected async Task<int> ExecuteAsync(
       string query,
       object model,
       CancellationToken cancellationToken = default) =>
        await _outboxTransactionManager.RunAsync(async db =>
        {
            return await db.ExecuteAsync(new CommandDefinition(
                query,
                model,
                cancellationToken: cancellationToken
            ));
        }, cancellationToken);


    

    internal protected async Task<T> ExecuteAndGetFirstOrDefaultAsync<T>(
        IDbConnection db,
        string query,
        object model,
        CancellationToken cancellationToken = default)
        => await db.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                query,
                model,
                cancellationToken: cancellationToken
            ));

    internal protected async Task<T> ExecuteAndGetFirstOrDefaultAsync<T>(

        string query,
        object model,
        CancellationToken cancellationToken = default
    )
    {

        return await _outboxTransactionManager.RunAsync(async db =>
        {
            return await db.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                query,
                model,
                cancellationToken: cancellationToken
            ));
        }, cancellationToken);

    }

    internal protected async Task<IEnumerable<T>> ExecuteAndGetListAsync<T>(
        IDbConnection db,
        string query,
        object model,
        CancellationToken cancellationToken = default)
    {
        var response = await db.QueryMultipleAsync(new CommandDefinition(
            query,
            model,
            cancellationToken: cancellationToken
        ));

        return await response.ReadAsync<T>();

    }
    internal protected async Task<IEnumerable<T>> ExecuteAndGetListAsync<T>(

        string query,
        object model,
        CancellationToken cancellationToken = default)
    {

        return await _outboxTransactionManager.RunAsync(async db =>
        {
            var response = await db.QueryMultipleAsync(new CommandDefinition(
                query,
                model,
                cancellationToken: cancellationToken
            ));
            return await response.ReadAsync<T>();
        }, cancellationToken);

    }


    internal protected async Task<PagedResult<T>> ExecuteAndGetPagedResultAsync<T>(
        IDbConnection db,
        string query,
        object model,
        CancellationToken cancellationToken = default
    )
    where T : class
    {

        var items = await db.QueryAsync<T>(new CommandDefinition(
            query,
            model,
            cancellationToken: cancellationToken
        ));

        items = items ?? new List<T>();
        return new PagedResult<T>
        {
            Items = items.ToList(),
            TotalCount = items == null || items.Count() == 0 ?
            0 : items
                    .AsQueryable()
                    .Select(GetPropertyByName<T, int>("TotalCountRecords").Compile())
                    .First()
        };

    }

    internal protected async Task<PagedResult<T>> ExecuteAndGetPagedResultAsync<T>(

       string query,
       object model,
       CancellationToken cancellationToken = default
   )
   where T : class
    {

        return await _outboxTransactionManager.RunAsync(async db =>
        {
            var items = await db.QueryAsync<T>(new CommandDefinition(
                query,
                model,
                cancellationToken: cancellationToken
            ));

            items = items ?? new List<T>();
            return new PagedResult<T>
            {
                Items = items.ToList(),
                TotalCount = items.Count() == 0 ?
                0 : items
                        .AsQueryable()
                        .Select(GetPropertyByName<T, int>("TotalCountRecords").Compile())
                        .First()
            };
        }, cancellationToken);

    }

    internal protected async Task<PagedResult<T>> ExecuteAndGetPagedResultHasMultiAsync<T>(
        IDbConnection db,
        string query,
        object model,
        CancellationToken cancellationToken = default
    ) where T : class
    {

        using var multi = await db.QueryMultipleAsync(
            new CommandDefinition(
                query,
                model,
                cancellationToken: cancellationToken
            )
        );
        var items = await multi.ReadAsync<T>();
        var totalCount = await multi.ReadSingleAsync<int>();
        items = items ?? new List<T>();
        return new PagedResult<T>
        {
            Items = items.ToList(),
            TotalCount = items == null || items.Count() == 0 ? 0 : totalCount
        };

    }
    internal protected async Task<PagedResult<T>> ExecuteAndGetPagedResultHasMultiAsync<T>(

        string query,
        object model,
        CancellationToken cancellationToken = default
    ) where T : class
    {

        return await _outboxTransactionManager.RunAsync(async db =>
        {
            using var multi = await db.QueryMultipleAsync(
                new CommandDefinition(
                    query,
                    model,
                    cancellationToken: cancellationToken
                )
            );
            var items = await multi.ReadAsync<T>();
            var totalCount = await multi.ReadSingleAsync<int>();
            items = items ?? new List<T>();
            return new PagedResult<T>
            {
                Items = items.ToList(),
                TotalCount = items == null || items.Count() == 0 ? 0 : totalCount
            };
        }, cancellationToken);

    }
    internal protected Expression<Func<T, Y>> GetPropertyByName<T, Y>(
        string propertyName
    )
    {
        var parameter = Expression.Parameter(typeof(T), "s");
        var body = Expression.PropertyOrField(parameter, propertyName);
        return Expression.Lambda<Func<T, Y>>(body, parameter);
    }

    internal protected async Task<T> ExecuteScalarAsync<T>(IDbConnection db,
      string query,
      object model,
      CancellationToken cancellationToken = default)
    {
        if (db == null)
        {
            return await _outboxTransactionManager.RunAsync<T>(async db =>
            {
                return await db.ExecuteScalarAsync<T>(
                    new CommandDefinition(
                        query,
                        model,
                        cancellationToken: cancellationToken
                    )
                );
            }, cancellationToken);
        }
        else
        {
            return await db.ExecuteScalarAsync<T>(
                    new CommandDefinition(
                        query,
                        model,
                        cancellationToken: cancellationToken
                    )
                );
        }
    }
}
