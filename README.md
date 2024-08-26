# playlister

Tool to download your Spotify Playlists into a SQLite database.

## notes

- runs on <http://localhost:5000> and <https://localhost:5001>

## String Enums

`ReleaseDatePrecision` on `Album`s can't be set as an enum right now due to a limitation in the System.Text.Json
serializer: <https://github.com/dotnet/runtime/issues/31081>

## Database

Uses a single **Sqlite** database.

### Schema

![Playlister.db sqlite database schema](/images/db_schema.png)

## ClientApp

A `Vue.js` SPA contained in a single file, `index.html`. It's just a simple hacked-together app for assisting
development. It gets the job done but needs to be updated.

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

- [x] FIXME: set secrets in app
- [ ] FIXME: Open web browser when running from Docker container

### UI

- [ ] Rewrite the UI (in Blazor, Svelte, Solid.js, or just vanilla JS/HTML (or possibly a CLI))

### .NET

- Tests
  - [ ] Integration
    - [ ] Use [mountebank](https://www.mbtest.org/)
  - [ ] Unit
    - [ ] Property-based
- [ ] Add `Polly` Policy to attempt re-auths for 401s
- [ ] Upgrade to .NET 8
  - [ ] Swap out `Polly` for `Microsoft.Extensions.Http.Resilience`
- Spotify Access Token
  - [ ] Validate that the `state` value matches the original value sent to user
  - [ ] Generate a client token to return so that the Spotify Access Token is never exposed outside the API
- [ ] `SpotifyAuthUrlHandler.cs` => cache `state` so that it can be validated on the access token command
- [ ] [Get User's saved tracks](https://developer.spotify.com/documentation/web-api/reference/get-users-saved-tracks)
- [ ] `PlaylistService.cs` => Fix performance issue with updating large playlists
  - There doesn't seem to be a way to fix this; Spotify's API doesn't allow sorting/filtering when calling the Spotify
      [Get Playlist Items](https://developer.spotify.com/documentation/web-api/reference/get-playlists-tracks) endpoint.
- [ ] Add `.devcontainer`
- [ ] Documentation/visualisations
- [ ] (?) Use [Humanizer](https://github.com/Humanizr/Humanizer)
- SQLite
  - [x] Add a stored proc to cull the SQL tables
    - artists/albums/songs that have been removed from playlists and have no references
    - need to figure out which table(s) to cull
  - [ ] Schema visualisation
  - [ ] (maybe) Simplify Playlist tables (add all relevant data in table? - need to look into : this)
- CI/CD
  - [ ] dotnet format
  - [ ] Test coverage
  - [ ] Add badges to README
  - [ ] Codecov
  - [x] Add CI/CD pipeline with GitHub Actions
  - [x] Build/format/test
  - [x] Test
- [x] Remove `Database::ConnectionString` from `appsettings.json` and use Dotnet Secret Manager to set it
- [x] put in Docker container
- [x] Automatically open URL, not Swagger Page
- [x] `UrlUtility`: add case for WSL
