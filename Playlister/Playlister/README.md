# playlister

Tool to download your Spotify Playlists into a SQLite database.

## notes

- runs on <http://localhost:5000> and <https://localhost:5001>

## String Enums

`ReleaseDatePrecision` on `Album`s can't be set as an enum right now due to a limitation in the System.Text.Json
serializer: <https://github.com/dotnet/runtime/issues/31081>

## Database

Sqlite

## ClientApp

A `Vue.js` SPA contained in a single file, `index.html`. It's just a simple hacked-together app for assisting
development. A more feature-filled Console app will be created in a separate project.

## JSON serialization

The .NET `System.Text.Json` serializer/deserializer

## External API access

A mixture of `Refit` and `HttpClient`

## Redirect URIs

Configured in the Spotify Developer Portal.

- <https://localhost:5001/app/home/login>

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
  }
}
EOF
```

- Run `cat secrets.json | dotnet user-secrets set`

## TODO

- UI
    - [ ] Rewrite the UI (in Blazor, Svelte, Solid.js, or just vanilla JS/HTML)
- .NET
    - Tests
        - [ ] Integration
        - [ ] Unit
        - [ ] Property-based
    - Add `.devcontainer
    - (maybe) put in Docker container
- SQLite
    - [ ] Add a stored proc to cull the SQL tables
        - artists/albums/songs that have been removed from playlists and have no references
        - need to figure out which table(s) to cull
    - (maybe) Simplify Playlist tables (add all relevent data in table? - need to look into this)