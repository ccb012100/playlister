use crate::{
    data::{FilterField, SortField},
    output::search::SearchOutput,
    search::data::{LastAlbumsRequest, SearchFileType, SearchRequest},
    sqlite::AlbumSelection,
};
use crate::{
    search,
    sync::{self},
};

use anyhow::Context;
use clap::{
    arg,
    builder::{styling::AnsiColor, Styles},
};
use clap::{command, Parser};
use log::{info, LevelFilter};
use std::path::PathBuf;

use subcommands::{FileType, Subcommands};

mod subcommands;

const STYLES: Styles = Styles::styled()
    .header(AnsiColor::Yellow.on_default())
    .usage(AnsiColor::Green.on_default())
    .literal(AnsiColor::Green.on_default())
    .placeholder(AnsiColor::Green.on_default());

#[derive(Parser, Debug)]
#[command(styles=STYLES)]
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
            Subcommands::Last {
                n,
                all,
                source: file,
                no_format,
            } => {
                info!("ℹ️ Last...");

                let file_type = SearchFileType::Sqlite;
                let file_path: PathBuf = FileType::Sqlite.get_path(file)?;

                let request = LastAlbumsRequest {
                    source: &file_path,
                    source_file_type: file_type,
                    num: *n as usize,
                    selection: match all {
                        true => AlbumSelection::All,
                        false => AlbumSelection::Starred,
                    },
                };

                let results = search::last(&request)?;

                if *no_format {
                    SearchOutput::print_last_n_albums(&results, request.num)
                } else {
                    SearchOutput::print_last_n_albums_table(&results, request.num)
                }

                Ok(())
            }
            Subcommands::Search {
                include_header,
                include_playlist_name,
                file_name,
                file_type,
                no_format,
                sort,
                filter,
                term,
                all: all_albums,
            } => {
                info!("ℹ️ Searching... 🔎");

                let path: PathBuf = file_type.get_path(file_name)?;

                let request: SearchRequest = SearchRequest {
                    selection: match all_albums {
                        true => AlbumSelection::All,
                        false => AlbumSelection::Starred,
                    },
                    source: &path,
                    filters: filter
                        .iter()
                        .map(|f| FilterField::from(*f))
                        .collect::<std::collections::HashSet<_>>() // collect into HashSet to dedupe the values
                        .into_iter()
                        .collect(),
                    include_header: *include_header,
                    include_playlist_name: *include_playlist_name,
                    search_term: &term.join(" "),
                    search_type: match &file_type {
                        FileType::Sqlite => SearchFileType::Sqlite,
                        FileType::Tsv => SearchFileType::Tsv,
                    },
                    sort: SortField::from(*sort),
                };

                let results: search::data::SearchResults<'_> = search::search(&request)
                    .with_context(|| format!("❌ Search failed: {:#?} ❌", request))?;

                if *no_format {
                    SearchOutput::print_search_results(&results)
                } else {
                    SearchOutput::print_search_results_table(&results)
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
