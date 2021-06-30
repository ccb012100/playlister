# playlister

View and search your Spotify playlists

## notes

- runs on <https://localhost:5001>

## String Enums

`ReleaseDatePrecision` on `Album`s can't be set as an enum right now due to a limitation in the System.Text.Json
serializer: https://github.com/dotnet/runtime/issues/31081

## Database

Sqlite

## ClientApp

A `Vue.js` SPA contained in a single file, `index.html`. It's just a simple hacked-together app for assisting
development. A more feature-filled Console app will be created in a separate project.

## JSON serialization

The .NET `System.Text.Json` serializer/deserializer

## External API access

A mixture of `Refit` and `HttpClient`
