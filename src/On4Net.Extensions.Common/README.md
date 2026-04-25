# On4Net.Extensions.Common

Shared helpers: extension methods on `string`, `byte[]`, and other types; regex validation constants; and a `Culture` struct. Namespace: `On4Net.Extensions.Common`.

**Dependencies:** Newtonsoft.Json; `StringExtension.cs` includes a static `using` for `System.Formats.Asn1` (can be removed if unused).

---

## `ObjectExtension` — `ObjectExtension.cs`

| Method | What it does |
|--------|----------------|
| `Compress(this byte[] data)` | Compresses a byte array using **GZipStream**; returns compressed bytes. |
| `Decompress(this byte[] data)` | Decompresses GZip-compressed bytes; returns raw bytes. |
| `DecompressMemoryStream(this byte[] data)` | Same as decompress but returns a **MemoryStream** (e.g. for HTTP responses). |
| `GetNumberByGuid(this Guid guid)` | XOR-combines GUID bytes into a numeric string (short/display code). |
| `GetNumberByGuid(this string arg)` | Parses string as GUID (via `ToGuid`) then returns `GetNumberByGuid(Guid)`. |
| `ConvertToDatatable<T>(this IList<T> data, string tableName)` | Builds a **DataTable** from public properties of `T` and fills rows from the list. |
| `BindDataClass<T>(this T contractData, T enjectData, bool isCheckValue = false)` | Copies same-named property values from `enjectData` to `contractData` via reflection; optional skip of default/null values. |
| `CreateInstance(this string ClassName)` | `Type.GetType` + `Activator.CreateInstance` for a type name string. |
| `CreateInstance<T>(this T ClassName) where T : class` | Creates a new instance of `T`; the receiver is only used as extension target. |
| `ToDateTime(this long unixTime)` | Converts Unix epoch **seconds** to local **DateTime**. |
| `GetTime()` | Returns current UTC time as Unix seconds (not an extension). |
| `ToEnum<T>(this string enumString) where T : Enum` | Parses string to enum; throws `ArgumentException` if not defined. |
| `ToJsonString<T>(this T @object, JsonSerializerSettings settings = null, Formatting formatting = None)` | Newtonsoft serialize; default ignores nulls. |
| `FromJsonString<T>(this string json)` | Newtonsoft deserialize to `T`; throws if null/whitespace. |

`DefaultValue(Type)` in the same file is **private** (used by `BindDataClass`).

---

## `StringExtension` — `StringExtension.cs`

| Method | What it does |
|--------|----------------|
| `IsNullOrEmpty(this string obj, bool isNullOrWhiteSpace = true)` | `IsNullOrWhiteSpace` or `IsNullOrEmpty` based on flag. |
| `IsNotNullOrEmpty(this string obj, bool isNullOrWhiteSpace = true)` | Negation of the above. |
| `Add_Zero_Before(this string InputStr, int OutLen)` | Left-pads with zeros to length `OutLen` (accounting-style). |
| `Add_Zero_After(this string InputStr, int OutLen)` | Right-pads with zeros to length `OutLen`. |
| `Add_SpaceAfter(this string InStr, int OutLen)` | Right-pads with spaces to length `OutLen`. |
| `ToDecimalNull` / `ToDecimal` | Safe parse to `decimal?` / `decimal` (default if fail). |
| `ToInt16Null` / `ToInt16` | Safe parse to `short?` / `short`. |
| `ToByteNull` / `ToByte` | Safe parse to `byte?` / `byte`. |
| `ToInt32Null` / `ToInt32` | Safe parse to `int?` / `int`. |
| `ToInt64Null` / `ToInt64` | Safe parse to `long?` / `long`. |
| `ToDoubleNull` / `ToDouble` | Safe parse to `double?` / `double`. |
| `ToFloatNull` / `ToFloat` | Safe parse to `float?` / `float`. |
| `ToGuidNull` / `ToGuid` | Parse GUID; `ToGuid` returns `Guid.Empty` on failure. |
| `IsRtl(this string culture)` | Uses `CultureInfo.GetCultureInfo`; returns `TextInfo.IsRightToLeft`. |
| `GetDirection(this string culture)` | Returns `"rtl"` or `"ltr"`. |
| `GetLanguageCulture(this string culture)` | First segment of `xx-YY` (e.g. `fa` from `fa-IR`). |
| `GetCountryCulture(this string culture)` | Second segment; throws if culture format is invalid. |

---

## `RegExExtesion` — `RegExExtesion.cs` (typo in type name)

**Public regex pattern constants:** `RegEx_Digit`, `RegEx_Decimal`, `RegEx_Email`, `RegEx_WebAddress`, `RegEx_Time12`, `RegEx_Time24`, `RegEx_TimeDuration`, `RegEx_IPAddress`, `RegEx_Password`.

| Method | What it does |
|--------|----------------|
| `IsMatch_Digit` … `IsMatch_Password` (`this string InputString`) | Each wraps the matching `RegEx_*` constant with `Regex.IsMatch`. |
| `IsExpressionValid(this string stringForValidate, string pattern)` | Validates with a custom pattern; returns `false` if input is null or empty. |

`IsMatch` (two strings) is **private** in this class.

---

## `Culture` — `Culture.cs`

| Member | What it does |
|--------|----------------|
| `Culture.English` | Static preset: `CurrentCulture` and `DefaultCulture` = `en-US`. |
| `CurrentCulture`, `DefaultCulture`, `OtherCultures` | Fields on the struct for app culture settings. |
| `CurrentLanguage` | Getter: `CurrentCulture.GetLanguageCulture()` (uses `StringExtension`). |
| `CurrentCountry` | Getter: `CurrentCulture.GetCountryCulture()`. |

---

## Project layout

```
On4Net.Extensions.Common/
├── Culture.cs
├── ObjectExtension.cs
├── RegExExtesion.cs
└── StringExtension.cs
```
