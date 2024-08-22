use crate::search;
use anyhow::{anyhow, Result};
use clap::{arg, Subcommand, ValueEnum};
use log::debug;
use regex::Regex;
use std::path::PathBuf;

#[derive(Subcommand, Debug, Clone)]
pub(crate) enum Subcommands {
    /// Search playlists
    Search {
        /// File type to perform action against
        #[clap(value_enum)]
        file_type: FileType,

        /// File to use
        file_name: String,

        /// Search term
        term: Vec<String>,

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
    },
    /// Sync playlists
    Sync {
        /// File type to perform action against
        #[clap(value_enum)]
        file_type: FileType,

        /// File to use
        file_name: String,
    },
}
#[derive(ValueEnum, Debug, Copy, Clone, PartialEq, Eq, PartialOrd, Ord)]
pub(crate) enum FileType {
    /// SQLite database file (file name = `[*.sql|*.sqlite|*.sqlite3|*.db]`).
    Sqlite,
    /// TSV file (file name = `*.tsv`)
    Tsv,
}

#[derive(ValueEnum, Debug, Copy, Clone, PartialEq, Eq, PartialOrd, Ord)]
pub(crate) enum SortFields {
    Artists,
    Album,
    Year,
    Added,
    Playlist,
}

impl From<SortFields> for search::data::SortFields {
    fn from(value: SortFields) -> Self {
        match value {
            SortFields::Added => search::data::SortFields::Added,
            SortFields::Album => search::data::SortFields::Album,
            SortFields::Artists => search::data::SortFields::Artists,
            SortFields::Playlist => search::data::SortFields::Playlist,
            SortFields::Year => search::data::SortFields::Year,
        }
    }
}

impl FileType {
    /// Parse `file_name` and return it as `PathBuf`
    pub(crate) fn get_path(&self, file_name: &str) -> Result<PathBuf> {
        debug!(
            "get_path called with: {} for FileType {:#?}",
            file_name, self
        );

        let pattern = match self {
            FileType::Sqlite => r"(?im).+\.(?:sql|sqlite|sqlite3|db)$",
            FileType::Tsv => r"(?im).+\.(?:tsv)$",
        };

        // check file name validity
        match Regex::new(pattern)?.is_match(file_name) {
            true => {
                let path = PathBuf::from(file_name);

                match path.try_exists()? {
                    true => Ok(path),
                    false => {
                        let err_msg = format!("❌ File \"{}\" does not exist ❌", file_name);
                        debug!("🪵 {}", err_msg);
                        Err(anyhow!(err_msg))
                    }
                }
            }
            false => {
                let err_msg = format!("❌ File name format \"{}\" is invalid ❌", { file_name });
                debug!("🪵 {}", err_msg);
                Err(anyhow!(err_msg))
            }
        }
    }
}
