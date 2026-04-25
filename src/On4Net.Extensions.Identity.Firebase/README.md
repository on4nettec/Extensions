# On4Net.Extensions.Identity.Firebase

ASP.NET Core integration with **Firebase**: **JWT Bearer** validation for Firebase ID tokens, HTTP calls to **Google Identity Toolkit** (sign-in, sign-up, verify email, custom token exchange), and a **role claim** authorization filter.

---

## Configuration file — project root

Place **`firebase-config.json`** in the host application root when you call `RegisterFirebaseAuth(..., isCreateFirebaseApp: true)`. Used by `FirebaseApp.Create` with `GoogleCredential.FromFile("./firebase-config.json")` in `ServiceConfiguration.cs`.

---

## Options — `Services/FirebaseOptions.cs`

**Note:** `FirebaseOptions` is declared in the **global namespace** (no `namespace` block in the file).

| Property | What it does |
|----------|----------------|
| `Url` | Getter returns `https://identitytoolkit.googleapis.com/v1/` (REST base for toolkit). |
| `ApiKey` | Firebase Web API key for REST calls. |
| `Audience` | Typically Firebase **project id**; used as JWT audience. |
| `Validator` | Computed issuer-style URL: `https://securetoken.google.com/{Audience}`. |

Environment / appsettings binding example:

```json
"FirebaseOptions": {
  "ApiKey": "[ApiKey]",
  "Audience": "[ProjectId]"
}
```

Equivalent env vars: `FirebaseOptions__ApiKey`, `FirebaseOptions__Audience`.

---

## DI registration — `Services/ServiceConfiguration.cs`

| Method | What it does |
|--------|----------------|
| `RegisterFirebaseAuth(this IServiceCollection services, IConfiguration Configuration, bool isCreateFirebaseApp = false)` | Reads section `FirebaseOptions`. If `isCreateFirebaseApp`, creates `FirebaseApp` from `firebase-config.json`. Registers `IFirebaseServices` → `FirebaseServices` with `HttpClient` base address from options `Url` and ctor `apiKey`. Adds **JwtBearer** authentication: `Authority` / `ValidIssuer` = `Validator`, `Audience` = options audience, `OnAuthenticationFailed` hook (currently no-op). Calls `AddAuthorization()`. |

---

## Firebase REST client — `Services/FirebaseServices.cs` / `Services/Infostruct/IFirebaseServices.cs`

`FirebaseServices` implements `IFirebaseServices`. Each method sets `HttpClient.BaseAddress` by appending the relative URI to the existing base (see implementation when using multiple calls).

| Method | File | What it does |
|--------|------|----------------|
| `Login(string username, string password, CancellationToken cancellationToken)` | `accounts:signInWithPassword`; POST email/password; returns `LoginModel` or **null** if HTTP failure. |
| `SignupNewUser(string email, string password, bool returnSecureToken, CancellationToken cancellationToken)` | `accounts:signUp`; returns `LoginModel` or **null**. |
| `SendVerifyEmail(string idToken, CancellationToken cancellationToken)` | `accounts:sendOobCode` with `requestType: VERIFY_EMAIL`; returns raw response string or **null**. |
| `VerifyCustomToken(string token, bool returnSecureToken, CancellationToken cancellationToken)` | `accounts:signInWithCustomToken`; returns `VerifyCustomTokenModel` or **null**. |

---

## DTOs — `Models/`

| Type | File | Properties / role |
|------|------|---------------------|
| `LoginModel` | `Models/LoginModel.cs` | `IdToken`, `Email`, `RefreshToken`, `ExpiresIn`, `LocalId`, `Registered` (JSON property names via `JsonProperty`). |
| `VerifyCustomTokenModel` | `Models/VerifyCustomTokenModel.cs` | `IdToken`, `RefreshToken`, `ExpiresIn`. |

---

## Role-based authorization — `Auth/`

| Type | File | What it does |
|------|------|----------------|
| `AuthorizationAttribute` | `Auth/AuthorizationAttribute.cs` | `TypeFilterAttribute` for `AuthorizationFilter`; ctor takes `params string[] policies` passed as filter constructor args. |
| `AuthorizationFilter` | `Auth/AuthorizationFilter.cs` | `IAsyncAuthorizationFilter`: if no policies → forbid. Else checks each identity’s **`ClaimTypes.Role`** claims; if any claim value equals any requested policy name → allow; else **`ForbidResult`**. |
| `AuthorizationConstants` | `Auth/AuthorizationConstants.cs` | Suggested role name constants: `User`, `SupperAdmin`, `Admin`, `SupperSupport`, `Support`. |

**Usage on controllers/actions:** `[Authorization("Admin", "User")]` — users must have a matching role claim.

---

## Dependencies

FirebaseAdmin, Google.Apis.Auth, Microsoft.AspNetCore.Authentication.JwtBearer, Newtonsoft.Json.
