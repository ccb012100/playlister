use crate::{
    output::search::SearchOutput,
    search::data::{SearchQuery, SearchType},
};
use crate::{
    search,
    sync::{self},
};

use anyhow::Context;
use clap::arg;
use clap::Parser;
use log::{info, LevelFilter};
use std::path::PathBuf;

use subcommands::{FileType, Subcommands};

mod subcommands;

#[derive(Parser, Debug)]
#[command(about, version, arg_required_else_help = true)]
pub struct Cli {
    /// Set verbosity
    #[arg(
        long,
        short = 'v',
        action = clap::ArgAction::Count,
        global = true
    )]
    pub verbose: u8,

    #[command(subcommand)]
    pub subcommand: subcommands::Subcommands,
}

impl Cli {
    pub fn run_subcommand(&self) -> Result<(), anyhow::Error> {
        match &self.subcommand {
            Subcommands::Search {
                include_header,
                include_playlist_name,
                file_name,
                file_type,
                no_format,
                sort,
                filter,
                term,
            } => {
                info!("ℹ️ Searching... 🔎");

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
                    sort: search::data::SortField::from(*sort),
                    filters: filter
                        .iter()
                        .map(|f| search::data::FilterField::from(*f))
                        .collect(),
                };

                let results: search::data::SearchResults<'_> = search::search(&query)
                    .with_context(|| format!("❌ Search failed: {:#?} ❌", query))?;

                match no_format {
                    true => SearchOutput::search_results(&results),
                    false => SearchOutput::search_results_table(&results),
                }

                Ok(())
            }
            Subcommands::Sync {
                source,
                destination,
            } => {
                info!("ℹ️ Syncing...");

                let source_path: PathBuf = FileType::Sqlite.get_path(source)?;
                let destination_path: PathBuf = FileType::Tsv.get_path(destination)?;

                sync::sync(&source_path, &destination_path).with_context(|| {
                    format!(
                        "❌ Syncing {:#?} to {:#?} failed! ❌",
                        source_path, destination_path
                    )
                })?;

                println!("\n✔️ Sync complete! ✔️");
                Ok(())
            }
        }
    }

    pub fn initialize_logger(&self) {
        let log_level = match self.verbose {
            0 => LevelFilter::Error,
            1 => LevelFilter::Warn,
            2 => LevelFilter::Info,
            3 => LevelFilter::Debug,
            4..=std::u8::MAX => LevelFilter::Trace,
        };

        env_logger::Builder::new().filter_level(log_level).init();

        info!("ℹ️ logging initialized at level {}", log_level);
    }
}
