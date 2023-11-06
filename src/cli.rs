use crate::search;
use anyhow::{anyhow, Result};
use clap::{arg, Parser, Subcommand, ValueEnum};
use regex::Regex;
use std::path::PathBuf;

#[derive(Parser)]
#[command(about, version, arg_required_else_help = true)]
pub(crate) struct Cli {
    /// Increase message output; useful for debugging
    #[arg(long)]
    #[arg(default_value_t = false)]
    pub(crate) verbose: bool,

    /// File type to perform action against
    #[clap(value_enum)]
    pub(crate) file_type: FileType,

    /// File to use
    pub(crate) file_name: String,

    #[command(subcommand)]
    pub(crate) command: Commands,
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
        #[arg(long)]
        #[arg(default_value_t = false)]
        include_playlist_name: bool,

        /// Don't format output
        #[arg(long)]
        #[arg(default_value_t = false)]
        no_format: bool,

        /// Include header row in output
        #[arg(long)]
        #[arg(default_value_t = false)]
        include_header: bool,

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
    Year,
    Added,
}

impl From<SortFields> for search::SortFields {
    fn from(value: SortFields) -> Self {
        match value {
            SortFields::Artists => search::SortFields::Artists,
            SortFields::Album => search::SortFields::Album,
            SortFields::Year => search::SortFields::Year,
            SortFields::Added => search::SortFields::Added,
        }
    }
}

pub(crate) fn get_path(file_name: &str, file_type: FileType) -> Result<PathBuf> {
    let pattern = match file_type {
        FileType::Db => r"(?im).+\.(?:sql|sqlite|sqlite3|db)$",
        FileType::Tsv => r"(?im).+\.(?:tsv)$",
    };

    // check file name validity
    match !Regex::new(pattern)?.is_match(file_name) {
        true => Err(anyhow!(format!("File name format \"{}\" is invalid.", {
            file_name
        }))),
        false => {
            // check if file is exists
            let path = PathBuf::from(file_name);

            match path.try_exists()? {
                true => Ok(path),
                false => Err(anyhow!(format!("File {} does not exist", file_name))),
            }
        }
    }
}
