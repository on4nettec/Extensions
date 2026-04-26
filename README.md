# On4Net Extensions

A set of **.NET** libraries for On4Net products: a PostgreSQL + Dapper data layer, shared utilities, consistent API error handling, and Firebase authentication for ASP.NET Core. Each package can be published to NuGet independently and has its own English `README` with API-level detail.

---

## Prerequisites

- **SDK:** .NET 10 (`TargetFramework`: `net10.0` on all projects)
- For **Data:** PostgreSQL; if you use the outbox pattern, configure your queue transport (e.g. **AWSSDK.SQS** is referenced by the package)

---

## Repository layout

| Path | Description |
|------|-------------|
| `Extensions/src/Extensions.Data.sln` | Visual Studio solution containing all four projects |
| `Extensions/src/On4Net.Extensions.Common/` | Extension methods, JSON, culture, regex helpers |
| `Extensions/src/On4Net.Extensions.Exception/` | Domain exceptions and mapping to `ErrorResponse` |
| `Extensions/src/On4Net.Extensions.Data/` | Dapper, transactions, outbox, DbUp, base repository |
| `Extensions/src/On4Net.Extensions.Identity.Firebase/` | Firebase JWT, Identity Toolkit, role filter |
| `Extensions/src/On4Net.Extensions.Tests/` | xUnit tests (English docs under `README.md` in that folder) |

High-level dependency graph:

```text
On4Net.Extensions.Common          (base; no dependency on other packages in this repo)
        ↑
On4Net.Extensions.Data            (project reference to Common)

On4Net.Extensions.Exception       (standalone)

On4Net.Extensions.Identity.Firebase  (standalone from the three above; Firebase / ASP.NET Core)
```

---

## Packages at a glance

### On4Net.Extensions.Common

Namespace: `On4Net.Extensions.Common`. GZip compression, Newtonsoft serialization, safe parsing from strings to numeric types and GUIDs, `DataTable` from lists, regex constants and validation, and a `Culture` struct for app culture settings.

**Detailed docs:** [`src/On4Net.Extensions.Common/README.md`](src/On4Net.Extensions.Common/README.md)

---

### On4Net.Extensions.Exception

Namespace: `On4Net.Extensions.Exception`. Base type `AppException`, types such as `NotFoundException` and `DataValidationException`, concrete `ApplicationException` for **unforeseen** application errors (same constructor contract as `AppException`), the `ErrorResponse` DTO, and the `GetErrorFromException` extension for uniform JSON errors from middleware or a global filter.

**Detailed docs:** [`src/On4Net.Extensions.Exception/README.md`](src/On4Net.Extensions.Exception/README.md)

---

### On4Net.Extensions.Data

Namespace: `On4Net.Extensions.Data`. Service registration via `ConfigureDataServices<T>`, PostgreSQL connection options (`DataOptions`), `ITransactionManager` / `IOutboxTransactionManager`, `BaseRepository` with entity and localization models, embedded SQL migrations with DbUp, and the hosted `SchemaMigratorService<T>`.

Dependencies: **Common** plus Npgsql, Dapper, DbUp for PostgreSQL, `Microsoft.Extensions.*` packages, and **AWSSDK.SQS** for AWS outbox scenarios.

**Detailed docs:** [`src/On4Net.Extensions.Data/README.md`](src/On4Net.Extensions.Data/README.md)

---

### On4Net.Extensions.Identity.Firebase

**Firebase** integration for ASP.NET Core: JWT validation for Firebase ID tokens, an HTTP client for **Google Identity Toolkit** (sign-in, sign-up, verify email, custom token), and `[Authorization(...)]` based on role claims (`ClaimTypes.Role`). When `isCreateFirebaseApp: true`, place **`firebase-config.json`** at the host application root. Bind `FirebaseOptions` from `appsettings` or environment variables (e.g. `FirebaseOptions__ApiKey`).

**Detailed docs:** [`src/On4Net.Extensions.Identity.Firebase/README.md`](src/On4Net.Extensions.Identity.Firebase/README.md)

---

## Command-line build

From `Extensions/src`:

```bash
dotnet build Extensions.Data.sln -c Release
```

To pack a single project (Data example):

```bash
dotnet pack On4Net.Extensions.Data/On4Net.Extensions.Data.csproj -c Release
```

---

## Testing

From `Extensions/src`:

```bash
dotnet test On4Net.Extensions.Tests/On4Net.Extensions.Tests.csproj -c Release
```

The test project targets **.NET 10**, references **xUnit** and **Moq**, and uses **`Microsoft.AspNetCore.App`** for ASP.NET filter tests. See `Extensions/src/On4Net.Extensions.Tests/README.md` for layout and scope (unit tests only; no PostgreSQL required for the default run).

---

## Versioning and publishing

Each package version is defined in its `.csproj` (`Version` / `PackageVersion`). NuGet metadata often points at the GitHub Extensions.Data repository; see each project’s `LICENSE.txt` and `icon.png` for license and package icon details.

---

## Summary

| Package | Primary use |
|---------|-------------|
| **Common** | General-purpose helpers without database or web coupling |
| **Exception** | Error contract for APIs |
| **Data** | PostgreSQL access + transactions + migrations + repository pattern |
| **Identity.Firebase** | Authentication and roles with Firebase on ASP.NET Core |

For per-module APIs and tables, use the `README.md` inside each project folder.
