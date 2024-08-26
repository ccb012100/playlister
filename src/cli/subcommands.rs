use crate::search;
use anyhow::{anyhow, Result};
use clap::{arg, Subcommand, ValueEnum};
use log::debug;
use regex::Regex;
use std::path::PathBuf;

#[derive(Subcommand, Debug, Clone)]
pub enum Subcommands {
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
        #[arg(short, long, value_name = "FIELD", value_enum)]
        #[arg(default_value_t = SortField::Artists)]
        sort: SortField,

        /// Field(s) to filter the search on
        #[arg(short, long, value_name = "FILTER", value_delimiter = ',', value_enum)]
        #[arg(default_values_t = [FilterField::Artists, FilterField::Album])]
        filter: Vec<FilterField>,

        /// Include Playlist names in search results
        #[arg(long, default_value_t = false)]
        include_playlist_name: bool,

        /// Don't format output
        #[arg(long, default_value_t = false)]
        no_format: bool,

        /// Include header row in output
        #[arg(long, default_value_t = false)]
        include_header: bool,
    },
    /// Sync playlists
    Sync {
        /// The full path of the sqlite file to sync from
        #[arg(short, long)]
        source: String,

        /// The full path of the tsv file to sync to
        #[arg(short, long)]
        destination: String,
    },
}
#[derive(ValueEnum, Debug, Copy, Clone, PartialEq, Eq, PartialOrd, Ord)]
pub enum FileType {
    /// SQLite database file (file name = `[*.sql|*.sqlite|*.sqlite3|*.db]`).
    Sqlite,
    /// TSV file (file name = `*.tsv`)
    Tsv,
}

#[derive(ValueEnum, Debug, Copy, Clone, PartialEq, Eq, PartialOrd, Ord)]
pub enum SortField {
    Artists,
    Album,
    Year,
    Added,
    Playlist,
}

#[derive(clap::ValueEnum, Debug, Copy, Clone, PartialEq, Eq, PartialOrd, Ord)]
pub(crate) enum FilterField {
    Artists,
    Album,
    Playlist,
}

impl From<SortField> for search::data::SortField {
    fn from(value: SortField) -> Self {
        match value {
            SortField::Added => search::data::SortField::Added,
            SortField::Album => search::data::SortField::Album,
            SortField::Artists => search::data::SortField::Artists,
            SortField::Playlist => search::data::SortField::Playlist,
            SortField::Year => search::data::SortField::Year,
        }
    }
}

impl From<FilterField> for search::data::FilterField {
    fn from(value: FilterField) -> Self {
        match value {
            FilterField::Artists => search::data::FilterField::Artists,
            FilterField::Album => search::data::FilterField::Album,
            FilterField::Playlist => search::data::FilterField::Playlist,
        }
    }
}

impl FileType {
    /// Parse `file_name` and return it as `PathBuf`
    pub fn get_path(&self, file_name: &str) -> Result<PathBuf> {
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
