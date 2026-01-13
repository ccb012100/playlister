# playlister

[![.NET](https://github.com/ccb012100/playlister/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ccb012100/playlister/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/ccb012100/playlister/actions/workflows/codeql.yml/badge.svg)](https://github.com/ccb012100/playlister/actions/workflows/codeql.yml)

Tool to download your **Spotify** Playlists into a **SQLite** database and search them.

The [api](/api/) folder contains the **.NET** API/UI for syncing your **Spotify** data to **SQLite**.

The [shell](/shell/) folder contains shell scripts, mainly for working with the `playlister.sqlite3` **SQLite** database.

The [playlist-util](/playlist-util/) folder contains a **Rust** utility for searching `playlister.sqlite3` and syncing it to a corresponding `TSV` file.

## Database schema

Schema for the **SQLite** database containing synced **Spotify** data.

![SQLite database schema](/images/db_schema.png)

## TODO

- [ ] Why does Spotify see these 2 albums as the _same_ album?
  - possibly some tracks are from 1 and some from the other?
  - Martha Reeves & The Vandellas - **Dance Party**
  - Various Artists - **Motown: The Complete No. 1's**
