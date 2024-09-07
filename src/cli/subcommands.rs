use anyhow::{anyhow, Result};
use clap::{arg, Subcommand, ValueEnum};
use log::trace;
use regex::Regex;
use std::path::PathBuf;

#[derive(Subcommand, Debug, Clone)]
pub enum Subcommands {
    /// Get n most recent Albums added to the specified `sqlite` database
    Last {
        /// The number of Albums to return
        #[arg(default_value_t = 10)]
        n: u8,

        /// If `false`, limit the results to Starred Albums
        #[arg(short, long, default_value_t = false)]
        all: bool,

        /// The full path of the `sqlite` file to pull from
        #[arg(short, long)]
        source: String,

        /// Don't format output
        #[arg(long, default_value_t = false)]
        no_format: bool,
    },
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

impl From<SortField> for crate::data::SortField {
    fn from(value: SortField) -> Self {
        match value {
            SortField::Added => crate::data::SortField::Added,
            SortField::Album => crate::data::SortField::Album,
            SortField::Artists => crate::data::SortField::Artists,
            SortField::Playlist => crate::data::SortField::Playlist,
            SortField::Year => crate::data::SortField::Year,
        }
    }
}

impl From<FilterField> for crate::data::FilterField {
    fn from(value: FilterField) -> Self {
        match value {
            FilterField::Artists => crate::data::FilterField::Artists,
            FilterField::Album => crate::data::FilterField::Album,
            FilterField::Playlist => crate::data::FilterField::Playlist,
        }
    }
}

impl FileType {
    /// Parse `file_name` and return it as `PathBuf`
    pub fn get_path(&self, file_name: &str) -> Result<PathBuf> {
        trace!(
            "get_path called with: {} for FileType {:#?}",
            file_name,
            self
        );

        let pattern = match self {
            FileType::Sqlite => r"(?im).+\.(?:sql|sqlite|sqlite3|db)$",
            FileType::Tsv => r"(?im).+\.(?:tsv)$",
        };

        // check file name validity
        if Regex::new(pattern)?.is_match(file_name) {
            let path = PathBuf::from(file_name);

            if path.try_exists()? {
                Ok(path)
            } else {
                let err_msg = format!("❌ File \"{}\" does not exist ❌", file_name);
                Err(anyhow!(err_msg))
            }
        } else {
            let err_msg = format!("❌ File name format \"{}\" is invalid ❌", { file_name });
            Err(anyhow!(err_msg))
        }
    }
}
