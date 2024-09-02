# playlister

[![.NET](https://github.com/ccb012100/playlister/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ccb012100/playlister/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/ccb012100/playlister/actions/workflows/codeql.yml/badge.svg)](https://github.com/ccb012100/playlister/actions/workflows/codeql.yml)

Tool to download your **Spotify** Playlists into a **SQLite** database and search them.

The [api](/api/) folder contains the **.NET** API/UI for syncing your **Spotify** data to **SQLite**.

The [shell](/shell/) folder contains shell scripts, mainly for working with the `playlister.db` **SQLite** database.

## Database schema

Schema for the **SQLite** database containing synced **Spotify** data.

![SQLite database schema](/images/db_schema.png)
