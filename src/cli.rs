use crate::search;
use clap::{arg, Parser, Subcommand, ValueEnum};
use regex::Regex;

#[derive(Parser)]
#[command(about, version, arg_required_else_help = true)]
pub(crate) struct Cli {
    #[clap(value_enum)]
    pub file_type: FileType,

    /// File to use
    pub file_name: String,

    #[command(subcommand)]
    pub command: Commands,
}

#[derive(ValueEnum, Clone, Copy, Debug)]
pub(crate) enum FileType {
    /// SQLite database file (file name = [*.sql|*.sqlite|*.sqlite3|*.db]).
    Db,
    Tsv,
}

#[derive(Subcommand)]
pub(crate) enum Commands {
    /// Search playlists
    Search {
        /// Field to sort on
        #[arg(short, long, value_name = "FIELD")]
        #[arg(default_value_t = SortFields::Artists)]
        #[clap(value_enum)]
        sort: SortFields,

        /// Include Playlist names in search results
        #[arg(short, long)]
        #[arg(default_value_t = false)]
        include_playlist_name: bool,

        /// Search term
        term: Vec<String>,
    },
    /// Sync playlists
    Sync {},
}

#[derive(ValueEnum, Clone, Copy, Debug)]
pub(crate) enum SortFields {
    Artists,
    Album,
    Released,
    Added,
}

impl From<SortFields> for search::SortFields {
    fn from(value: SortFields) -> Self {
        match value {
            SortFields::Artists => search::SortFields::Artists,
            SortFields::Album => search::SortFields::Album,
            SortFields::Released => search::SortFields::Released,
            SortFields::Added => search::SortFields::Added,
        }
    }
}

pub(crate) fn file_name_is_valid(file_name: &str, file_type: FileType) -> bool {
    let pattern = match file_type {
        FileType::Db => r"(?im).+\.(?:sql|sqlite|sqlite3|db)$",
        FileType::Tsv => r"(?im).+\.(?:tsv)$",
    };

    Regex::new(pattern).unwrap().is_match(file_name)

    // TODO: Verify file exists
}
