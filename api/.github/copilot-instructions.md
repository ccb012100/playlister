# Playlister - AI Coding Agent Instructions

## Project Overview
ASP.NET Core 8.0 web application that syncs Spotify playlists into a local SQLite database. Uses ASP.NET Core MVC for UI with minimal client-side dependencies.

## Architecture

### CQRS Pattern (Without MediatR)
- **Commands**: Write operations in `CQRS/Commands/` (e.g., `SyncPlaylistCommand`)
- **Queries**: Read operations in `CQRS/Queries/` (e.g., `GetCurrentUserPlaylistsQuery`)
- **Handlers**: Business logic in `CQRS/Handlers/` (e.g., `PlaylistSyncHandler`)
- Commands/Queries are simple records; Handlers contain the logic
- Handlers are registered as services in `StartupExtensions.AddHandlers()`
- **Key difference**: Commands may return values (not pure CQRS)

### Service Organization
- **Controllers**: API endpoints in `Controllers/` and MVC endpoints in `Mvc/Controllers/`
- **Services**: Business logic interfaces/implementations follow `I{Name}Service` pattern
- **Repositories**: Data access with read/write separation (`IPlaylistReadRepository`, `IPlaylistWriteRepository`)
- **Utilities**: Helper classes in `Utilities/` (e.g., `PollyUtility` for retry policies)

### Dependency Injection
Services registered in `Extensions/StartupExtensions.cs`:
- `AddServices()` - Application services and handlers
- `AddRefitClients()` - Spotify API clients with Polly retry policies
- `AddAndValidateConfiguration()` - Options pattern with validation
- Service lifetimes: Use `Scoped` for DB-touching services, `Transient` for handlers

## External API Integration

### Spotify API Access
- Uses **Refit** for declarative HTTP clients (`ISpotifyApi`, `ISpotifyAccountsApi` in `RefitClients/`)
- Clients configured with snake_case JSON serialization (`JsonUtility.SnakeCaseRefitSettings`)
- **Polly** retry policy handles 429 (rate limit) responses via `RetryAfter` header
- Models in `Models/SpotifyApi/` and `Models/SpotifyAccounts/`

### Authentication Flow
- OAuth 2.0 flow through `SpotifyAuthorizationController`
- Access tokens stored in cookies, validated via `[ValidateTokenCookie]` attribute
- `TokenValidationMiddleware` handles token validation
- Redirect URI: `https://127.0.0.1:5001/login` (configured in Spotify Developer Portal)

## Configuration

### User Secrets (Required)
Store sensitive configuration using .NET User Secrets:
```bash
cat <<EOF > secrets.json
{
  "Spotify": {
    "ClientId": "<ID>",
    "ClientSecret": "<SECRET>",
    "Database": {
      "ConnectionString": "DataSource=/path/to/playlister.sqlite3;"
    }
  }
}
EOF
cat secrets.json | dotnet user-secrets set
```

### Configuration Classes
- `SpotifyOptions` - Spotify API credentials and endpoints
- `DatabaseOptions` - SQLite connection string
- `DebuggingOptions` - Development settings (HTTP logging, config printing)
- Validated on startup using `Options` pattern with data annotations

### "Hands-Free" Mode
Set `"HandsFree": true` in config to automatically sync all playlists after login and exit.

## Database

### SQLite Schema
- Tables defined in `Data/DataTables.cs`: `Playlist`, `Track`, `Album`, `Artist`, `PlaylistTrack`, etc.
- Dapper for data access with snake_case compatibility (`DefaultTypeMap.MatchNamesWithUnderscores = true`)
- SQL queries in `Data/SqlQueries.cs`
- Repositories in `Repositories/Implementations/`

### Connection Factory
`IConnectionFactory` provides SQLite connections; used by all repositories.

## Development Workflows

### Build & Run
- **Build**: `dotnet build Playlister/Playlister.csproj` or VS Code task
- **Run**: `dotnet watch run` (with hot reload) or VS Code task
- **Tests**: `./test.sh` runs all test projects
- **Docker**: `docker-compose up` (see `docker-compose.yml` for volume mounts)

### Testing
- Unit tests in `Playlister.Tests/`
- Integration tests in `Playlister.Tests.Integration/` use in-memory SQLite
- Custom `WebApplicationFactory` in `CustomWebApplicationFactory.cs` for integration testing setup
- **Database seeding**: Tests auto-initialize with schema + seed data via `TestDatabaseHelper`
  - Schema: `SQLite/create_db.sqlite`
  - Seed data: `SQLite/seed_db.sql` (8 artists, 10 albums, 27 tracks, 5 playlists)
  - Set `factory.SeedDatabase = false` to skip seeding for specific tests

### Logging
- Serilog file logging configured in `appsettings.json`
- HTTP request/response logging via `HttpLoggingMiddleware` (dev only)
- Logs written to `Playlister/Logs/`

## Key Conventions

### Code Style
- Primary constructors preferred for DI (see handlers and controllers)
- Explicit null handling with nullable reference types enabled
- Global usings in `GlobalUsings.cs` (e.g., `System.Collections.Immutable`, `Playlister.Extensions`)

### JSON Serialization
- Uses `System.Text.Json` (not Newtonsoft.Json)
- Snake_case for Spotify API communication
- CamelCase for application JSON responses
- Enums serialized as strings with `[JsonConverter(typeof(JsonStringEnumConverter))]`

### Middleware
- Custom middleware in `Middleware/`: error handling, token validation, HTTP logging
- Registered in `Startup.ConfigureWebApplication()`

### Attributes
- `[ValidateTokenCookie]` - Ensures access token cookie exists on controller/action

## Important Files
- [Startup.cs](../Playlister/Startup.cs) - Service registration and middleware pipeline
- [StartupExtensions.cs](../Playlister/Extensions/StartupExtensions.cs) - DI configuration
- [Program.cs](../Playlister/Program.cs) - Application entry point
- [appsettings.json](../Playlister/appsettings.json) - Non-sensitive configuration
- [docker-compose.yml](../docker-compose.yml) - Container configuration with secret/volume mounts
