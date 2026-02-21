# AGENTS.md — Playlister API

## Overview

ASP.NET Core 8.0 (C# 12) web application that integrates with the Spotify API.
Uses CQRS (without MediatR), Dapper + SQLite for data access, Refit for HTTP clients,
and Razor views for the UI. Mono-repo — this is the `api/` subproject.

## Build / Run / Test Commands

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build Playlister/Playlister.csproj

# Run (or use ./run.sh)
dotnet run --project Playlister/Playlister.csproj

# Run all tests (or use ./test.sh)
dotnet test

# Run a single test by fully-qualified name
dotnet test --filter "FullyQualifiedName~Playlister.Tests.Extensions.TimeSpanExtensionsTests.ToFormattedString"

# Run a single test class
dotnet test --filter "FullyQualifiedName~Playlister.Tests.Extensions.TimeSpanExtensionsTests"

# Run only unit tests
dotnet test Playlister.Tests/Playlister.Tests.csproj

# Run only integration tests
dotnet test Playlister.Tests.Integration/Playlister.Tests.Integration.csproj

# Watch mode
dotnet watch run --project Playlister/Playlister.csproj
```

CI runs on GitHub Actions: restore → build → test on `ubuntu-latest` with .NET 8.0.x,
triggered on push/PR to `main` when `api/**` files change.

## Code Style

### Formatting (enforced by `.editorconfig`)

- **Indentation**: 4 spaces for C#, 2 spaces for XML/YAML/JSON
- **Line endings**: LF, with final newline
- **Braces**: K&R style — opening brace on same line (not Allman)
- **Spaces inside parentheses**: YES — `method( param1 , param2 )`, including
  method calls, control flow, casts, and indexers. Space before comma.
- **Namespaces**: File-scoped (`namespace Playlister;`)
- **Explicit types**: Preferred over `var` for built-in types
- **`this.` qualifier**: Discouraged
- **Braces for single-line blocks**: Always required
- **Primary constructors**: Preferred for dependency injection

### Imports

System namespaces first, then third-party, then project namespaces.
Separate each group with a blank line. Place outside the namespace.

```csharp
using System.Text.Json;

using Dapper;

using Microsoft.AspNetCore.Mvc;

using Playlister.Models;
using Playlister.Services;

namespace Playlister.Controllers;
```

Global usings in `GlobalUsings.cs`:
`System.Collections.Immutable`, `Playlister.Extensions`, `Playlister.Models`.

### Naming Conventions

| Element                  | Convention                                  | Example                    |
|--------------------------|---------------------------------------------|----------------------------|
| Interfaces               | `I` prefix + PascalCase                     | `IPlaylistService`         |
| Private/internal fields  | `_` prefix + camelCase                      | `_logger`, `_connectionString` |
| Static fields            | `s_` prefix + camelCase                     | `s_playlistCache`          |
| Constants                | PascalCase                                  | `CorsPolicyName`           |
| Options classes          | `{Name}Options` with `const string` section | `SpotifyOptions`           |
| Services                 | `I{Name}Service` / `{Name}Service`          | `IAuthService`             |
| Repositories             | `I{Name}Repository` / `{Name}Repository`    | `IPlaylistReadRepository`  |
| Handlers                 | `{Name}Handler`                             | `PlaylistSyncHandler`      |
| Commands/Queries         | `{Verb}{Noun}Command` / `{Verb}{Noun}Query` | `SyncPlaylistCommand`      |

### Types & Models

- Use `record` types for models, commands, and queries
- Use `required` keyword and `init` accessors on record properties
- Nullable reference types are enabled — respect nullability annotations
- JSON serialization uses `System.Text.Json` with `[JsonPropertyName("snake_case")]`
  for Spotify API models, `[JsonConverter(typeof(JsonStringEnumConverter))]` for enums

```csharp
public record Artist {
    public required string Id { get; init; }
    public required string Name { get; init; }
}

public record SyncPlaylistCommand( string AccessToken , string PlaylistId );
```

### Error Handling

- Global error handling via `GlobalErrorHandlerMiddleware` (catches, logs, re-throws)
- Use `ArgumentNullException.ThrowIfNull()` for null-checking
- Handle `Refit.ApiException` specifically for HTTP errors from Spotify API
- Only one custom exception: `DbRecordNotFoundException`
- Polly `WaitAndRetryAsync` handles HTTP 429 (rate limiting) via `RetryAfter` header

### Dependency Injection

- `Singleton`: `IConnectionFactory`, `IHttpContextAccessor`
- `Scoped`: Services and repositories that touch the database
- `Transient`: Handlers and middleware
- Registration lives in `StartupExtensions.cs` extension methods

## Architecture

### CQRS Pattern

Commands/Queries are simple records. Handlers contain business logic and are injected
directly (no MediatR). Handlers live in `CQRS/Handlers/`, commands in `CQRS/Commands/`,
queries in `CQRS/Queries/`.

### Data Access

- Dapper with raw SQL in `Data/SqlQueries.cs`
- `IConnectionFactory` creates a new `SqliteConnection` per call
- Snake-case SQL column mapping: `DefaultTypeMap.MatchNamesWithUnderscores = true`
- Upsert pattern: `INSERT ... ON CONFLICT ... DO UPDATE`
- Table name constants in `Data/DataTables.cs`

### Controllers

Two controller types:
- `Controllers/` — REST API endpoints (inherit `BaseApiController`)
- `Mvc/Controllers/` — MVC controllers serving Razor views

### Refit HTTP Clients

- `ISpotifyApi` — Spotify Web API endpoints
- `ISpotifyAccountsApi` — Spotify OAuth token endpoints

## Testing

### Unit Tests (`Playlister.Tests/`)

- xUnit 2.9.2 with `[Fact]` and `[Theory]`/`[InlineData]`
- Mocking: Moq 4.20.72
- Assertions: AwesomeAssertions 9.3.0 (FluentAssertions fork)
- Test data: Bogus 35.6.1
- Follow AAA pattern with `// ARRANGE`, `// ACT`, `// ASSERT` comments
- Name tests: `MethodName_ShouldDoX_WhenConditionY`

### Integration Tests (`Playlister.Tests.Integration/`)

- `Microsoft.AspNetCore.Mvc.Testing` with `CustomWebApplicationFactory<TProgram>`
- Each test class gets an isolated in-memory SQLite database (unique GUID connection string)
- Schema in `SQLite/create_db.sqlite`, seed data in `SQLite/seed_db.sql`
- Mock Spotify API via `MockSpotifyApiProvider`
- Use `IClassFixture<CustomWebApplicationFactory<Startup>>`

## Configuration

- App settings: `appsettings.json` / `appsettings.Development.json`
- Secrets (Spotify API keys, DB path): .NET User Secrets
- Options pattern: `SpotifyOptions`, `DatabaseOptions`, `DebuggingOptions`

## Copilot Instructions Reference

Full Copilot instructions are in `.github/copilot-instructions.md` (135 lines).
Key additional guidelines from that file:
- Follow existing code patterns; do not introduce new frameworks without discussion
- Keep handlers thin — delegate to services
- All Spotify API calls go through Refit interfaces
- Use `ILogger<T>` for logging throughout
- Configuration uses the Options pattern bound from `appsettings.json` / User Secrets
