# On4Net.Extensions.Data

PostgreSQL data layer using **Dapper**, transaction helpers, optional **outbox** messaging, **DbUp** SQL migrations, and base entity / localization models. Root namespace: `On4Net.Extensions.Data`.

---

## DI registration — `Configuration.cs`

| Method | What it does |
|--------|----------------|
| `ConfigureDataServices<T>(this IServiceCollection services)` | Registers: `DataSchemaMigrator` (singleton), `SchemaMigratorService<T>` (hosted), `DbProviderFactory` → `NpgsqlFactory.Instance`, `ITransactionManager` → factory-built `TransactionManager`, `IOutboxTransactionManager` → factory-built `OutboxTransactionManager`. Connection string comes from `IOptions<DataOptions>`. |

Generic `T` is the type whose **assembly** contains embedded `.sql` scripts for migrations.

---

## Options — `DataOptions.cs`

| Property | What it does |
|----------|----------------|
| `Address`, `Port` (default `5432`), `UserName`, `Password`, `Name` | PostgreSQL server and database credentials. |
| `JournalTable` | DbUp journal table name (default `schema_version`). |
| `CommandTimeout` | Seconds (default `30`). |
| `ConnectionString` | Computed Npgsql connection string from the above. |

---

## Transactions — `DataManager/TransactionManager.cs`

`TransactionManager` implements `ITransactionManager` (`DataManager/Infostruct/ITransactionManager.cs`).

| Method | File | What it does |
|--------|------|----------------|
| `RunAsync(Func<IDbConnection, Task> action)` | `TransactionManager.cs` | Opens connection, begins transaction, runs `action`, commits; rolls back on failure. |
| `RunAsync(Func<IDbConnection, Task> action, CancellationToken)` | same | Cancellation-aware overload. |
| `RunAsync<T>(Func<IDbConnection, Task<T>> action, CancellationToken)` | same | Same pattern with a return value. |

---

## Outbox transactions — `DataManager/OutboxTransactionManager.cs`

`OutboxTransactionManager` extends `TransactionManager` and implements `IOutboxTransactionManager` (`Infostruct/IOutboxTransactionManager.cs`).

| Method | What it does |
|--------|----------------|
| `RunAsync(Func<IDbConnection, IMessageOutbox, Task> action)` | Runs work with an `IMessageOutbox` scoped to the connection; after successful commit, if `messageOutbox.HasMessage`, runs `PublishMessagesAsync` inside a **new** transactional `RunAsync`. |
| `RunAsync(..., CancellationToken)` | Same with cancellation. |
| `RunAsync<T>(Func<IDbConnection, IMessageOutbox, Task<T>> action, CancellationToken)` | Same with return value and post-commit publish. |

---

## Outbox contract — `DataManager/Infostruct/IMessageOutbox.cs`

| Member | What it does |
|--------|----------------|
| `HasMessage` | Whether queued messages need publishing. |
| `On(IDbConnection connection)` | Binds outbox implementation to the active connection. |
| `SendAsync(string queueUrl, string groupId, IEnumerable<string> messages, CancellationToken)` | Queues messages (implementation-specific). |
| `PublishMessagesAsync(IDbConnection connection, CancellationToken)` | Sends queued messages after the main transaction commits. |

---

## Query execution base — `DataManager/ExecutionMethods.cs`

Abstract base used by `BaseRepository`. Constructor sets `DefaultTypeMap.MatchNamesWithUnderscores = true`. All execution helpers below are **`internal protected`** (for subclasses in this assembly).

| Method | What it does |
|--------|----------------|
| `ExecuteAsync(IDbConnection db, string query, object model, CancellationToken)` | Dapper `ExecuteAsync` on the given connection. |
| `ExecuteAsync(string query, object model, CancellationToken)` | Runs inside `_outboxTransactionManager.RunAsync` (no explicit `IDbConnection` passed in). |
| `ExecuteAndGetFirstOrDefaultAsync<T>(...)` | Two overloads: with `IDbConnection` or via outbox manager; `QueryFirstOrDefaultAsync`. |
| `ExecuteAndGetListAsync<T>(...)` | `QueryMultipleAsync` then `ReadAsync<T>()`. |
| `ExecuteAndGetPagedResultAsync<T>(...)` where `T : class` | `QueryAsync<T>`; total count read from a property named **`TotalCountRecords`** on each row (compiled expression). |
| `ExecuteAndGetPagedResultHasMultiAsync<T>(...)` where `T : class` | `QueryMultiple`: first grid = items, second = single `int` total count. |
| `ExecuteScalarAsync<T>(IDbConnection db, ...)` | If `db` is null, runs scalar inside outbox `RunAsync`; else scalar on supplied connection. |
| `GetPropertyByName<T, Y>(string propertyName)` | Builds `Expression<Func<T,Y>>` for a property (used for paging projection). |

---

## Repository base — `DataManager/BaseRepository.cs`

Inherits `ExecutionMethods`. Inject via `IOutboxTransactionManager` + `Func<DateTime>` (passed to base).

| Member | What it does |
|--------|----------------|
| `TableName`, `LocalizationTableName` | **abstract** — concrete repository supplies PostgreSQL table names. |
| `EntityColumnMappings` | Maps logical sort keys to quoted SQL identifiers (default maps `ID` → `"ID"`). |
| `GenerateNewId()`, `GenerateNewVersion(int?)` | Static helpers for new GUID and optimistic version bump. |
| `AddLocalization(...)`, `AddLocalization<T>(...)` | Inserts a row into the localization table with standard columns; optional extra columns/parameters. |
| `DeleteLocalizationsAsync` | Soft-delete / batch update localization rows (CTE SQL in source). |
| `DeleteAsync` | Soft-delete main row by id + version + user audit fields. |
| `UpdateAsync` | Dynamic `UPDATE` with `DynamicParameters` + version check. |
| `GetLocalizationEntitiesAsync<T>` | Select from localization table by `localization_id` and optional culture(s). |
| `GetByIdAsync<T>` | Select from main table by `id`. |
| `GetSortCommand` | Builds `ORDER BY` from `Dictionary<string, SortDirection>` using `EntityColumnMappings`. |

---

## SQL fragment helpers — `DataManager/RepositoryExtensions.cs`

| Method | What it does |
|--------|----------------|
| `ToAndQuery(this List<string> parts)` | Joins non-empty parts with `AND` wrapped in parentheses each; empty/null list → `"TRUE"`. |
| `ToOrQuery(this List<string> parts)` | Joins with `OR` inside outer parentheses; empty → `"TRUE"`. |

---

## Paging DTO — `DataManager/PagedResult.cs`

| Property | What it does |
|----------|----------------|
| `Items` | `List<T>` where `T : class`. |
| `TotalCount` | Total row count for the query. |

---

## Migrations — `Migration/DataSchemaMigrator.cs`

| Method | What it does |
|--------|----------------|
| `DropDatabase()` | Drops the database named in connection string **only** if the database name starts with `test_`. |
| `ResetDataInTestDatabase(params string[] tableNames)` | Runs `DELETE FROM` for each table **only** on `test_*` databases. |
| `UpdateSchemas<T>(string journalTableName = null)` | DbUp: ensures DB exists, runs embedded `.sql` scripts from the assembly containing `T`, optional journal table override (else `DataOptions.JournalTable`). |

---

## Hosted migration — `Migration/SchemaMigratorService.cs`

| Type | What it does |
|------|----------------|
| `SchemaMigratorService<T> : BackgroundService` | On host startup, calls `DataSchemaMigrator.UpdateSchemas<T>()`. On exception, calls `IHostApplicationLifetime.StopApplication()` and rethrows. |

---

## Outbox row model — `Schema/Message.cs`

| Property | What it does |
|----------|----------------|
| `Id`, `MessageId`, `QueueUrl`, `GroupId`, `DeduplicationId`, `Payload`, `CreatedAt` | Typical fields for a persisted outbox / queue message row. |

---

## Request / entity / response models

| Type | File | What it does |
|------|------|----------------|
| `Status` enum | `Model/Entity/BaseEntity.cs` | `Deleted = 0`, `Active = 1`. |
| `BaseEntity` | same | Adds `AppId`, `UserName` on top of `BaseStatusEntity`. |
| `BaseStatusEntity` | `Model/Entity/BaseStatusEntity.cs` | `Id`, `Status`, `Version` + audit from `BaseLog`. |
| `BaseLog` | `Model/Entity/BaseLog.cs` | `CreatedAt`, `ModifiedAt`, `CreatedBy`, `ModifiedBy`. |
| `BaseLocalizationEntity` | `Model/Entity/BaseLocalizationEntity.cs` | `LocaliztionId` (spelling in code), `Culture`. |
| `BaseTitleLocalizationEntity` | `Model/Entity/BaseTitleLocalizationEntity.cs` | Adds `Title`. |
| `SortDirection`, `BaseSaerchRequest` | `Model/Request/BaseSaerchRequest.cs` | Search key, culture, paging, sort dictionary. |
| `BaseLocalization` (request) | `Model/Request/BaseLocalization.cs` | Namespace `Model.Request`. Abstract inbound row: `Id`, `Culture`, optional `Version`. |
| `BaseTitleLocalization` (request) | `Model/Request/BaseTitleLocalization.cs` | Namespace `Model.Request`. Extends request `BaseLocalization`; adds `Title`. |
| `BaseLocalizationRequest<T>` | `Model/Request/BaseLocalizationRequest.cs` | `Localization` is `IEnumerable<T>` where `T : BaseLocalization, new()`. |
| `BaseTitleLocalizationRequest` | `Model/Request/BaseTitleLocalizationRequest.cs` | Concrete `BaseLocalizationRequest<BaseTitleLocalization>` for title+culture payloads. |
| `BaseLocalization` (response) | `Model/Response/BaseLocalization.cs` | Namespace `Model.Response`. DTO: `Id`, `Culture`. |
| `BaseTitleLocalization` (response) | `Model/Response/BaseTitleLocalization.cs` | Extends response `BaseLocalization`; adds `Title`. |
| `BaseLocalizationResponse<T>` | `Model/Response/BaseLocalizationResponse.cs` | `IEnumerable<T> Localization`. |
| `BaseTitleLocalizationResponse` | `Model/Response/BaseTitleLocalizationResponse.cs` | Concrete response for title localizations. |

---

## Dependencies

Npgsql, Dapper, DbUp (PostgreSQL), Microsoft.Extensions.\*, **On4Net.Extensions.Common**.
