use crate::{cli::subcommands::FileType, search};
use crate::{
    output::Output,
    search::data::{SearchQuery, SearchType},
};
use clap::arg;
use clap::Parser;
use log::{info, LevelFilter};
use std::path::PathBuf;

use subcommands::Subcommands;

mod subcommands;

#[derive(Parser, Debug)]
#[command(about, version, arg_required_else_help = true)]
pub(crate) struct Cli {
    /// Set verbosity
    #[arg(
        long,
        short = 'v',
        action = clap::ArgAction::Count,
        global = true
    )]
    pub(crate) verbose: u8,

    #[command(subcommand)]
    pub(crate) subcommand: subcommands::Subcommands,
}

impl Cli {
    pub(crate) fn run_subcommand(&self) -> Result<(), anyhow::Error> {
        match &self.subcommand {
            Subcommands::Search {
                include_header,
                include_playlist_name,
                file_name,
                file_type,
                no_format,
                sort,
                term,
            } => {
                info!("Searching...");

                let path: PathBuf = file_type.get_path(file_name)?;

                let query: SearchQuery = SearchQuery {
                    search_term: &term.join(" "),
                    search_type: match &file_type {
                        FileType::Sqlite => SearchType::Sqlite,
                        FileType::Tsv => SearchType::Tsv,
                    },
                    file: &path,
                    include_header: *include_header,
                    include_playlist_name: *include_playlist_name,
                    sort: search::data::SortFields::from(*sort),
                };

                let results: search::data::SearchResults<'_> = search::search(&query)?;

                match no_format {
                    true => Output::search_results(&results),
                    false => Output::search_results_table(&results),
                }

                Ok(())
            }
            Subcommands::Sync {
                file_name,
                file_type,
            } => {
                info!("Syncing...");

                let _path: PathBuf = file_type.get_path(file_name)?;
                todo!()
            }
        }
    }

    pub(crate) fn initialize_logger(&self) {
        let log_level = match self.verbose {
            0 => LevelFilter::Error,
            1 => LevelFilter::Warn,
            2 => LevelFilter::Info,
            3 => LevelFilter::Debug,
            4..=std::u8::MAX => LevelFilter::Trace,
        };

        env_logger::Builder::new().filter_level(log_level).init();

        info!("logging initialized at level {}", log_level);
    }
}
