# On4Net.Extensions.Exception

Domain-specific exceptions and an extension to map any `System.Exception` to a consistent API payload (`ErrorResponse`). Namespace: `On4Net.Extensions.Exception`.

---

## `AppException` — `AppException.cs`

Abstract base for application errors.

| Member | What it does |
|--------|----------------|
| `MessageId` (`string`) | Stable id for clients / localization keys. |
| `StatusCode` (`int`) | Suggested HTTP status. |
| `Params` (`object[]?`) | Optional template parameters. |
| `.ctor(messageId, message, statusCode = 500, params = null)` | Protected constructor; sets base `Exception.Message`. |

---

## Derived exceptions

| Type | File | Constructors / behavior |
|------|------|---------------------------|
| `NotFoundException` | `NotFoundException.cs` | `NotFound` message id, default HTTP **404**; exposes `EntityName`; overloads with `id` or custom `message`. |
| `DuplicateKeyException` | `DuplicateKeyException.cs` | `DuplicateKey` message id, default **409**; exposes `EntityName`. |
| `ConcurrencyException` | `ConcurrencyException.cs` | `Concurrency` message id, default **409**. |
| `DataValidationException` | `DataValidationException.cs` | `Validation` message id, default **400**. |
| `ArgumentException` | `ArgumentException.cs` | **Note:** this is `On4Net.Extensions.Exception.ArgumentException`, not `System.ArgumentException`. Message id `Argument`, default **403**. |

---

## `ErrorResponse` — `ErrorResponse.cs`

DTO for API error bodies.

| Property | What it does |
|----------|----------------|
| `StatusCode` | HTTP status. |
| `MessageId` | Error identifier. |
| `Message` | Human-readable text. |
| `Params` | Extra key/value data (entity names, validation fields, etc.). |

---

## `ExceptionHandler` — `ExceptionHandler.cs`

| Method | Location | What it does |
|--------|----------|----------------|
| `GetErrorFromException(this System.Exception exception)` | `ExceptionHandler.cs` | Maps `exception` to `ErrorResponse`: **AppException** — copies `MessageId`, `Message`, `StatusCode`, wraps `Params`; **NotFoundException** — adds `EntityName`; **ValidationException** (DataAnnotations) — flattens `Data` to dictionary; **DuplicateKeyException** — `EntityName`; **On4Net.Extensions.Exception.ArgumentException** — from `Data`; **NotImplementedException** / **NotSupportedException** — `MessageId` `NotImplementedOrNotSupported`, status **500**; **ArgumentNullException** / **ArgumentOutOfRangeException** / **System.ArgumentException** — status **400**; **default** — generic `Exception`, status **500**. |

Use from global exception middleware or a central filter to return uniform JSON errors.

---

## Project layout

```
On4Net.Extensions.Exception/
├── AppException.cs
├── ArgumentException.cs
├── ConcurrencyException.cs
├── DataValidationException.cs
├── DuplicateKeyException.cs
├── ErrorResponse.cs
├── ExceptionHandler.cs
├── NotFoundException.cs
└── (namespace On4Net.Extensions.Exception)
```
