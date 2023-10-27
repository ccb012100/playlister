use clap::{arg, Parser, Subcommand};
use regex::Regex;
#[derive(Parser)]
#[command(about, version, arg_required_else_help = true)]
pub(crate) struct Cli {
    /// Perform action against a SQLite database instead of a TSV.
    #[arg(long)]
    #[arg(default_value_t = false)]
    pub db: bool,

    /// File to use
    pub file: String,

    #[command(subcommand)]
    pub command: Commands,
}

#[derive(Subcommand)]
pub(crate) enum Commands {
    /// Search playlists
    Search {
        /// Field to sort on; default is "Artist"
        #[arg(short, long, value_name = "FIELD")]
        sort: Option<String>,

        /// Include Playlist names in search results
        #[arg(short, long)]
        #[arg(default_value_t = false)]
        include_playlist_name: bool,
    },
    /// Sync playlists
    Sync {},
}

// TODO: how to set this as the allowed values for "sort"?
// #[derive(Clone,Copy)]
// enum SortFields {
//     Artists,
//     Album,
//     Released,
//     Added,
// }

pub(crate) fn db_is_valid(filename: &str) -> bool {
    Regex::new(r"(?im).+\.(?:sql|sqlite|sqlite3|db)$")
        .unwrap()
        .is_match(filename)

    // TODO: Verify file exists
}

pub(crate) fn tsv_is_valid(filename: &str) -> bool {
    Regex::new(r"(?im).+\.(?:tsv)$").unwrap().is_match(filename)

    // TODO: Verify file exists
}
