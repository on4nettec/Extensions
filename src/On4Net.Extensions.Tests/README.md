# On4Net.Extensions.Tests

Unit tests for the **On4Net.Extensions** packages (`Common`, `Data`, `Exception`, `Identity.Firebase`).

## Requirements

- .NET 10 SDK
- No external services are required for the default test run (Firebase REST calls use an in-memory `HttpMessageHandler`; PostgreSQL is not started by these tests).

## Run

From `Extensions/src`:

```bash
dotnet test On4Net.Extensions.Tests/On4Net.Extensions.Tests.csproj
```

With coverage (if a collector is configured in your environment):

```bash
dotnet test On4Net.Extensions.Tests/On4Net.Extensions.Tests.csproj --collect:"XPlat Code Coverage"
```

## Layout

| Folder | Scope |
|--------|--------|
| `Common/` | Extension helpers, `Culture`, regex helpers |
| `Data/` | `DataOptions`, `RepositoryExtensions`, `BaseRepository` sort helpers, DTOs, `Model.Request` localization types (`RequestLocalizationModelTests`) |
| `Exception/` | `ExceptionHandler`, exception constructors |
| `Firebase/` | `FirebaseOptions`, `FirebaseServices` HTTP behaviour, `AuthorizationFilter`, models |

## Notes

- **DataSchemaMigrator** / hosted migration and live Dapper queries are not covered here; add integration tests against PostgreSQL if you need end-to-end migration and repository SQL verification.
- **ExceptionHandler** tests assert the **current** mapping behaviour (including interaction between the `AppException` branch and the `switch` on concrete types).
- **ObjectExtension.DecompressMemoryStream** is covered with an assertion that the returned stream is disposed when the method returns (documents current disposal behaviour).
- Internal helpers (for example `ExecutionMethods` query helpers) are exercised indirectly through subclasses where possible; prefer integration tests for full Dapper SQL paths.
