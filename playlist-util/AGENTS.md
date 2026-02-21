# AGENTS.md - playlist-util

## Project Overview

Rust CLI tool (`playlist-util`) for searching and syncing Spotify album playlists.
Dual-crate structure: binary (`playlist-util` in `src/main.rs`) and library (`playlist_util` in `src/lib.rs`).
Part of a monorepo at the `playlist-util/` subdirectory.

## Build Commands

```bash
# Debug build
cargo build

# Release build
cargo build --release

# Build and install to ~/bin/
./update_usr_bin.sh

# Check without producing binaries (faster)
cargo check
```

## Test Commands

```bash
# Run all tests
cargo test

# Run a single test by name (substring match)
cargo test test_name
# Example: cargo test album_new

# Run tests in a specific module
cargo test data::tests

# Run tests with output shown
cargo test -- --nocapture

# Run tests verbosely (as CI does)
cargo test --verbose
```

All unit tests are inline in `src/data.rs` inside a `#[cfg(test)] mod tests` block.
There are no integration tests in a separate `tests/` directory.

## Lint / Format

No custom configuration files exist. Use default toolchain settings:

```bash
# Format code (default rustfmt settings)
cargo fmt

# Check formatting without modifying
cargo fmt -- --check

# Run linter (default clippy settings)
cargo clippy

# Clippy with warnings as errors
cargo clippy -- -D warnings
```

## CI

GitHub Actions (`.github/workflows/rust.yml` in monorepo root) runs on pushes/PRs to `main`
touching `playlist-util/**`. Runs `cargo build --verbose` and `cargo test --verbose` on both
`ubuntu-latest` and `windows-latest`.

## Project Structure

```
src/
  main.rs              # Binary entry: CLI parsing + dispatch
  lib.rs               # Library crate root (re-exports public API)
  cli.rs               # CLI definition (clap derive Parser)
  cli/subcommands.rs   # Subcommand enums + FileType validation
  data.rs              # Core domain types (Album, etc.) + unit tests
  data/search.rs       # Search request/result types
  search.rs            # Search dispatch (sqlite vs tsv)
  search/sqlite.rs     # SQLite search implementation
  search/tsv.rs        # TSV file search implementation
  sqlite.rs            # SQLite database access layer
  sync.rs              # Sync logic (sqlite -> tsv)
  sync/tsv.rs          # TSV read/write for sync
  output.rs            # Output base (stderr printing)
  output/search.rs     # Search result formatting (table + plain)
```

## Code Style

### Formatting

Default `rustfmt` rules. No `rustfmt.toml` exists.

### Imports

Group and order imports as:
1. Standard library (using nested paths)
2. External crates
3. Internal crate (`crate::`) imports

```rust
use std::{
    fmt::{self},
    io::{Error, ErrorKind},
    str::FromStr,
};

use anyhow::{Context, Result};
use log::{debug, trace};

use crate::data::{Album, AlbumTsv};
```

### Naming Conventions

| Kind        | Convention           | Example                          |
|-------------|----------------------|----------------------------------|
| Types       | PascalCase           | `Album`, `SearchRequest`         |
| Functions   | snake_case           | `run_subcommand`, `sort_by_field`|
| Variables   | snake_case           | `search_query_upper`             |
| Constants   | SCREAMING_SNAKE_CASE | `STYLES`, `LIMIT`                |
| Modules     | snake_case           | `subcommands`, `search`          |

### Newtype Pattern

Domain primitives use the newtype pattern extensively:

```rust
pub struct AlbumName(pub String);
pub struct AlbumArtists(pub String);
pub struct DateAdded(pub String);
pub struct ReleaseYear(pub i32);
pub struct TrackCount(pub u16);
```

Implement `Display` and `FromStr` traits for these types.

### Error Handling

- Use `anyhow::Result` and `anyhow::Context` for error propagation throughout the library.
- Add context to errors with `.with_context(|| format!("..."))`.
- Use `anyhow!()` macro for creating ad-hoc errors.
- Use the `?` operator for propagation.
- Use emoji prefixes in user-facing error/status messages: `"..."`, `"..."`, `"..."`, `"..."`.
- `std::io::Error` is used in `FromStr` implementations (not `anyhow`).

```rust
fn example() -> Result<(), anyhow::Error> {
    let file = File::open(&path)
        .with_context(|| format!("Failed to open file: {:#?}", &path))?;
    Ok(())
}
```

### Type Patterns

- Derive `Clone, Debug, PartialEq, Eq, PartialOrd, Ord` on domain types.
- Use lifetime annotations on request/result types that borrow data:
  ```rust
  pub struct SearchRequest<'a> {
      pub search_term: &'a str,
      pub source: &'a std::path::PathBuf,
  }
  ```
- Implement `From` trait for enum conversions (e.g., CLI enums to domain enums).
- Use `#[derive(ValueEnum)]` from clap for CLI enum integration.

### Architecture

- **`main.rs` is thin**: parse CLI args, init logging, dispatch to library code.
- **Module-per-feature** with sub-modules (e.g., `search.rs` + `search/sqlite.rs`).
- **Strategy pattern** for file type dispatch via `match`:
  ```rust
  match request.search_type {
      SearchFileType::Sqlite => sqlite::search(request),
      SearchFileType::Tsv => tsv::search(request),
  }
  ```

### Output

- Table output uses `comfy-table`; plain output is TSV-formatted.
- Falls back to plain output when stdout is not a terminal.
- Summary/status messages go to **stderr** (not stdout) to avoid interfering with piping.

### Logging

Verbosity controlled by `-v` flag count:
- 0: Error, 1: Warn, 2: Info, 3: Debug, 4+: Trace

Use `log` macros (`debug!`, `trace!`, `info!`, etc.) for diagnostic output.

## Dependencies

| Crate        | Purpose                              |
|--------------|--------------------------------------|
| `anyhow`     | Error handling                       |
| `chrono`     | Date/time                            |
| `clap`       | CLI parsing (derive API)             |
| `comfy-table`| Terminal table formatting            |
| `env_logger` | Logging initialization               |
| `log`        | Logging facade                       |
| `nu-ansi-term`| ANSI terminal colors/styling        |
| `regex`      | Regular expressions                  |
| `rusqlite`   | SQLite access (bundled on Windows)   |

## Platform Notes

- `rusqlite` uses the `bundled` feature on Windows only; non-Windows expects system SQLite.
- CI tests on both Ubuntu and Windows.
