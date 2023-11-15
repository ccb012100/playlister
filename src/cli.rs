use crate::search;
use anyhow::{anyhow, Result};
use clap::{arg, Parser, Subcommand, ValueEnum};
use log::debug;
use regex::Regex;
use std::path::PathBuf;

#[derive(Parser, Debug)]
#[command(about, version, arg_required_else_help = true)]
pub(crate) struct Cli {
    /// Set logging level; if only the flag is supplied, it will set LogLevel::Debug
    #[arg(long, global = true, value_name = "LogLevel")]
    #[arg(default_value_t = LogLevel::Warn)]
    #[arg(default_missing_value = "debug", num_args = 0..=1, require_equals = true)]
    #[clap(value_enum)]
    pub(crate) verbose: LogLevel,

    #[command(subcommand)]
    pub(crate) command: Subcommands,
}

#[derive(ValueEnum, Debug, Copy, Clone, PartialEq, Eq, PartialOrd, Ord)]
pub(crate) enum FileType {
    /// SQLite database file (file name = [*.sql|*.sqlite|*.sqlite3|*.db]).
    Db,
    /// TSV file (file name = *.tsv)
    Tsv,
}

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

/// Parse `file_name` and return it as PathBuf
pub(crate) fn get_path(file_name: &str, file_type: &FileType) -> Result<PathBuf> {
    debug!("get_path called with: {}, {:#?}", file_name, file_type);

    let pattern = match file_type {
        FileType::Db => r"(?im).+\.(?:sql|sqlite|sqlite3|db)$",
        FileType::Tsv => r"(?im).+\.(?:tsv)$",
    };

    // check file name validity
    match Regex::new(pattern)?.is_match(file_name) {
        true => {
            let path = PathBuf::from(file_name);

            match path.try_exists()? {
                true => Ok(path),
                false => {
                    let err_msg = format!("File \"{}\" does not exist", file_name);
                    debug!("{}", err_msg);
                    Err(anyhow!(err_msg))
                }
            }
        }
        false => {
            let err_msg = format!("File name format \"{}\" is invalid.", { file_name });
            debug!("{}", err_msg);
            Err(anyhow!(err_msg))
        }
    }
}

#[derive(ValueEnum, Debug, Copy, Clone, PartialEq, Eq, PartialOrd, Ord)]
pub(crate) enum LogLevel {
    /// Info
    Info,
    /// Debug
    Debug,
    /// Warn
    Warn,
    /// Error
    Error,
    /// Trace
    Trace,
    /// Off
    Off,
}
