# playlist-util

[![Rust](https://github.com/ccb012100/playlist-util/actions/workflows/rust.yml/badge.svg)](https://github.com/ccb012100/playlist-util/actions/workflows/rust.yml)

CLI utility for Syncing and Searching my Spotify album playlists created from [Playlister](https://github.com/ccb012100/playlister) and [playlist-search](https://github.com/ccb012100/playlist-search/)

## Usage

```console
Usage: playlist-util [OPTIONS] <COMMAND>

Commands:
  search  Search playlists
  sync    Sync playlists
  help    Print this message or the help of the given subcommand(s)

Options:
  -v, --verbose...  Set verbosity
  -h, --help        Print help
  -V, --version     Print version
```

### `search`

```console
Search playlists

Usage: playlist-util search [OPTIONS] <FILE_TYPE> <FILE_NAME> [TERM]...

Arguments:
  <FILE_TYPE>  File type to perform action against [possible values: sqlite, tsv]
  <FILE_NAME>  File to use
  [TERM]...    Search term

Options:
  -s, --sort <FIELD>           Field to sort on [default: artists] [possible values: artists, album, year, added, playlist]
  -v, --verbose...             Set verbosity
  -f, --filter <FILTER>        Field(s) to filter the search on [default: artists album] [possible values: artists, album, playlist]
      --include-playlist-name  Include Playlist names in search results
      --no-format              Don't format output
      --include-header         Include header row in output
  -h, --help                   Print help (see more with '--help')
```

### `sync`

```console
Sync playlists

Usage: playlist-util sync [OPTIONS] --source <SOURCE> --destination <DESTINATION>

Options:
  -s, --source <SOURCE>            The full path of the sqlite file to sync from
  -v, --verbose...                 Set verbosity
  -d, --destination <DESTINATION>  The full path of the tsv file to sync to
  -h, --help                       Print help
```

## TODO

- [ ] Return an `Error` if the search term is empty
- [ ] Use [unicase](https://github.com/seanmonstar/unicase/pull/52) once [this issue](https://github.com/seanmonstar/unicase/pull/52) to add `UniCase::contains()` is completed.
