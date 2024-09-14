# playlister

[![.NET](https://github.com/ccb012100/playlister/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ccb012100/playlister/actions/workflows/dotnet.yml)

Tool to download your Spotify Playlists into a SQLite database.

## Database

Uses a single **Sqlite** database.

## UI

Uses [ASP.NET Core MVC](https://dotnet.microsoft.com/en-us/apps/aspnet/mvc) without any style sheets or JavaScript libraries.

## "Hands-free" mode

When the configuration value `"HandsFree": true` is present, after login/authorization the app will automatically Sync All Playlists and then stop the application.

## CQRS

The app generally follows the Command Query Responsibility Segregation (CQRS) pattern (though Commands sometimes return values), but it does _not_
use [MediatR](https://github.com/jbogard/MediatR), which adds too much abstraction/indirection for my tastes.

## JSON serialization

The .NET `System.Text.Json` serializer/deserializer

## External API access

APIs are accessed through a mixture of `Refit` and `HttpClient`.

## Redirect URIs

Configured in the Spotify Developer Portal.

- Points to `https://localhost:5001/app/home/login`

## Set up client secrets

- Open the root folder in a CLI
- Get **Client ID** and **ClientSecret** from [Spotify Developer Dashboard](https://developer.spotify.com/dashboard)
- Create `secrets.json` file with the config values:

```bash
cat <<EOF > secrets.json
{
  "Spotify": {
    "ClientId": "<ID>",
    "ClientSecret": "<SECRET>",
    "Database": {
      "ConnectionString": "<PATH_TO_SQLITE_DATABASE>"
    }
  }
}
EOF
```

- Run `cat secrets.json | dotnet user-secrets set`

## TODO

- [ ] Sync "liked" albums/songs

### Docker

- [ ] FIXME: Open web browser when running from Docker container

### .NET

- Tests
  - [ ] Integration
  - [ ] Unit
    - [ ] Property-based
- [ ] Add `Polly` Policy to attempt re-auths for 401s
- [ ] Upgrade to .NET 8
  - [ ] Swap out `Polly` for `Microsoft.Extensions.Http.Resilience`
- Spotify Access Token
  - [ ] Validate that the `state` value matches the original value sent to user
  - [x] Generate a client token to return so that the Spotify Access Token is never exposed outside the API
- [ ] `SpotifyAuthUrlHandler.cs` => cache `state` so that it can be validated on the access token command
- [ ] [Get User's saved tracks](https://developer.spotify.com/documentation/web-api/reference/get-users-saved-tracks)
- [ ] `PlaylistService.cs` => Fix performance issue with updating large playlists
  - There doesn't seem to be a way to fix this; Spotify's API does not allow sorting/filtering when calling the Spotify
      [Get Playlist Items](https://developer.spotify.com/documentation/web-api/reference/get-playlists-tracks) endpoint.
- [ ] Add `.devcontainer`
- [ ] Documentation/visualisations
- SQLite
  - [x] Add a stored proc to cull the SQL tables
    - artists/albums/songs that have been removed from playlists and have no references
    - need to figure out which table(s) to cull
  - [x] Schema visualisation
  - [ ] (maybe) Simplify Playlist tables (add all relevant data in table? - need to look into this)
- CI/CD
  - [ ] dotnet format
  - [ ] Test coverage
  - [x] Add badges to README
  - [ ] Codecov
  - [x] Add CI/CD pipeline with GitHub Actions
  - [x] Build/format/test
  - [x] Test
- [x] Remove `Database::ConnectionString` from `appsettings.json` and use Dotnet Secret Manager to set it
- [x] put in Docker container
- [x] Automatically open URL, not Swagger Page
- [x] `UrlUtility`: add case for WSL
