using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

using On4Net.Extensions.Common;
using On4Net.Extensions.Data.DataManager.Infostruct;

using On4Net.Extensions.Data.Model.Entity;
using On4Net.Extensions.Data.Model.Request;

using Dapper;

namespace On4Net.Extensions.Data.DataManager;
public abstract class BaseRepository : ExecutionMethods
{

    protected virtual Dictionary<string, string> EntityColumnMappings { get; set; } = new(StringComparer.OrdinalIgnoreCase)
        {
            {"ID", "\"ID\""}
        };


    public BaseRepository(IOutboxTransactionManager outboxTransactionManager,
        Func<DateTime> dateTimeProvider) : base(outboxTransactionManager, dateTimeProvider)
    {

    }

    protected abstract string TableName { get; }
    protected abstract string LocalizationTableName { get; }


    protected static Guid GenerateNewId() => Guid.NewGuid();

    protected static int GenerateNewVersion(int? oldVersion = null)
    {
        return oldVersion is null
            ? 1
            : oldVersion.Value + 1;
    }

    protected virtual async Task<BaseTitleLocalizationEntity> AddLocalization(BaseTitleLocalizationEntity entity, string user, CancellationToken cancellationToken)
    {
        var query = $@"INSERT INTO {LocalizationTableName}
        (
           id,
           localization_id,
           cultur,
           title,   
           status_code,
           created_at, 
           created_by, 
           version
        )
        VALUES
        (
            @Id,
            @LocaliztionId,
            @Culture,  
            @Title,
            @statusCode,
            @createdAt,
            @createdBy,
            @version
        ) RETURNING *; ";

        entity.Id = GenerateNewId();
        entity.Version = GenerateNewVersion();
        entity.CreatedBy = user;
        entity.CreatedAt = _dateTimeProvider();
        await ExecuteAsync(query, entity, cancellationToken);
        return entity;
    }

    protected virtual async Task<T> AddLocalization<T>(T entity, string columns, string parameters, string user, CancellationToken cancellationToken)
        where T : BaseTitleLocalizationEntity
    {
        if (columns.IsNotNullOrEmpty() && parameters.IsNullOrEmpty())
            throw new ArgumentException("The Columns is not null but the Parameters is null");
        if (parameters.IsNotNullOrEmpty() && columns.IsNullOrEmpty())
            throw new ArgumentException("The Parameters is not null but the Columns is null");

        if (columns.IsNotNullOrEmpty())
            columns = columns + ",";
        if (parameters.IsNotNullOrEmpty())
            parameters = parameters + ",";

        var query = $@"INSERT INTO {LocalizationTableName}
        (
           id,
           localization_id,
           cultur,
           {columns}
           title,   
           status_code,
           created_at, 
           created_by, 
           version
        )
        VALUES
        (
            @Id,
            @LocaliztionId,
            @Culture,
            {parameters}
            @Title,
            @statusCode,
            @createdAt,
            @createdBy,
            @version
        ) RETURNING *; ";

        entity.Id = GenerateNewId();
        entity.Version = GenerateNewVersion();
        entity.CreatedBy = user;
        entity.CreatedAt = _dateTimeProvider();
        await ExecuteAsync(query, entity, cancellationToken);
        return (T)entity;
    }

    protected virtual async Task<int> DeleteLocalizationsAsync(IDbConnection db, IEnumerable<Guid> ids, string user, CancellationToken cancellationToken)
    {
        var idList = ids
        .Distinct()
        .ToArray();
        var parameter = new
        {
            Ids = idList,
            Status = Status.Active,
            NewStatusCode = Status.Deleted,
            ModifiedAt = _dateTimeProvider(),
            ModifiedBy = user
        };
        var query = $@"
             WITH deleteDependItems as (
                 SELECT  lm.""id""  from  {TableName} as m
                    inner join {LocalizationTableName} as lm on lm.""localization_id"" = m.""id""
                    where  id = ANY (@Ids)
             ),
              deleteItems AS (
                UPDATE {LocalizationTableName}
                SET  
                    ""version"" = ""version"" +1, 
                    ""status"" = @NewStatusCode, 
                    ""modified_at"" = @ModifiedAt,
                    ""modified_by"" = @ModifiedBy
                from deleteDependItems 
                WHERE ""scope_id"" = @ScopeId
                      AND ""status"" = @StatusCode
                      AND ""id"" = deleteDependItems.rowId
				RETURNING *
            )
            SELECT COUNT(*) FROM deleteItems;
        ";

        return await ExecuteAsync(db,
         query,
         parameter,
         cancellationToken: cancellationToken);
    }
    protected virtual async Task<int> DeleteAsync(IDbConnection db, Guid id, int version, string user, CancellationToken cancellationToken)
    {
        var parameter = new
        {
            Id = id,
            Status = Status.Active,
            NewStatusCode = Status.Deleted,
            ModifiedAt = _dateTimeProvider(),
            ModifiedBy = user,
            version
        };
        var query = $@"
             WITH updater AS (
                UPDATE {TableName}
                SET  ""version"" = ""version"" +1,  
                    ""status"" = @NewStatusCode, 
                    ""modified_at"" = @ModifiedAt,
                    ""modified_by"" = @ModifiedBy
                WHERE  ""version""= @version
                  AND ""status"" = @StatusCode
                  AND ""id"" = @Id
                RETURNING *
            )
            SELECT COUNT(*) FROM updater;
        ";
        if (db == null)
        {
            return await ExecuteAsync(
          query,
          parameter,
          cancellationToken: cancellationToken);
        }
        return await ExecuteAsync(db,
          query,
          parameter,
          cancellationToken: cancellationToken);
    }

    protected virtual async Task<int> UpdateAsync(IDbConnection db, Guid id, int version,
        string fieldSet, Dictionary<string, object> parameters,
        string user, CancellationToken cancellationToken)
    {
        if (fieldSet.IsNotNullOrEmpty() && !parameters.Any())
            throw new ArgumentException("The Fieldset is not null but the Parameters is null");
        if (fieldSet.IsNullOrEmpty() && parameters.Any())
            throw new ArgumentException("The Parameters is not null but the Fieldset is null");
        DynamicParameters dynamicParameters = new DynamicParameters();
        foreach (var item in parameters)
        {
            dynamicParameters.Add(item.Key, item.Value);
        }
        dynamicParameters.Add("Id", id);
        dynamicParameters.Add("Version", version);
        dynamicParameters.Add("StatusCode", Status.Active);
        dynamicParameters.Add("NewStatusCode", Status.Deleted);
        dynamicParameters.Add("ModifiedAt", _dateTimeProvider());
        dynamicParameters.Add("ModifiedBy", user);
        if (fieldSet.IsNotNullOrEmpty())
            fieldSet = fieldSet + ",";
        var query = $@"
             WITH updater AS (
                UPDATE {TableName}
                SET 
                    {fieldSet}
                    ""version"" = ""version"" +1,  
                    ""status"" = @NewStatusCode, 
                    ""modified_at"" = @ModifiedAt,
                    ""modified_by"" = @ModifiedBy
                WHERE  ""version""= @Version
                  AND ""status"" = @StatusCode
                  AND ""id"" = @Id
                RETURNING *
            )
            SELECT COUNT(*) FROM updater;
        ";

        return await ExecuteScalarAsync<int>(db,query,
          dynamicParameters,
          cancellationToken: cancellationToken);
        //if (db == null)
        //{
        //    return await ExecuteAsync(
        //  query,
        //  dynamicParameters,
        //  cancellationToken: cancellationToken);
        //}
        //return await ExecuteAsync(db,
        // query,
        // dynamicParameters,
        // cancellationToken: cancellationToken);
    }
    protected virtual async Task<IEnumerable<T>> GetLocalizationEntitiesAsync<T>(Guid id, string culture, CancellationToken cancellationToken) where T : class
    {
        var query = @$"
             Select * from {LocalizationTableName}
                 where ""localization_id"" =@Id
         ";
        if (!string.IsNullOrEmpty(culture))
            query += @" And ""culture"" = @Culture";
        return await ExecuteAndGetListAsync<T>(query, new
        {
            Id = id,
            Culture = culture
        }, cancellationToken);
    }
    protected virtual async Task<IEnumerable<T>> GetLocalizationEntitiesAsync<T>(Guid id, string[] cultures, CancellationToken cancellationToken) where T : class
    {
        
        var query = @$"
             Select * from {LocalizationTableName}
                 where ""localiztion_id"" =@Id
         ";
        if (cultures.Any())
            query += @" And ""culture"" = Any(@Cultures)";
        return await ExecuteAndGetListAsync<T>(query, new
        {
            Id = id,
            Cultures = cultures
        }, cancellationToken);
       
    }

    protected virtual async Task<T> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken) where T : class
    {
        var query = @$"
             Select * from {TableName}
                 where ""id"" =@Id
         ";
        return await ExecuteAndGetFirstOrDefaultAsync<T>(query, new { Id = id }, cancellationToken);
    }

    protected virtual string GetSortCommand(Dictionary<string, SortDirection> orders)
    {
        if (orders is null || orders.Count == 0)
            return string.Empty;

        var columns = new List<string>();
        foreach (var order in orders)
        {
            EntityColumnMappings.TryGetValue(order.Key, out var fieldName);
            if (!string.IsNullOrEmpty(fieldName))
                columns.Add($"{fieldName} {(order.Value == SortDirection.DESC ? "DESC" : "ASC")}");
        }

        if (columns.Count == 0)
            return string.Empty;

        return "ORDER BY " + string.Join(",", columns);
    }
}
