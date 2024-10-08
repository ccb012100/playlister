use crate::output::search::SearchOutput;
use playlist_util::{
    data::{
        search::{LastAlbumsRequest, SearchFileType, SearchRequest, SearchResults},
        FilterField, SortField,
    },
    sqlite::AlbumSelection,
};
use playlist_util::{
    search,
    sync::{self},
};
use subcommands::{FileType, Subcommands};

use anyhow::Context;
use clap::{
    arg, builder::{styling::AnsiColor, Styles}, command, Args, Parser
};
use log::{info, LevelFilter};
use std::path::PathBuf;

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
    #[clap(flatten)]
    pub options: CliOptions,

    #[command(subcommand)]
    pub subcommand: subcommands::Subcommands,
}

#[derive(Args, Debug, Clone, Copy)]
pub struct CliOptions {
    /// Set verbosity; adding multiple times increases the verbosity level (>=4, i.e. `-vvvv`, sets maximum verbosity).
    #[arg(
        long,
        short = 'v',
        action = clap::ArgAction::Count,
    )]
    pub verbose: u8,

    /// Set logging level - if set, overrides `verbose`
    #[arg(
        long,
        visible_alias("log"),
        visible_alias("level"),
        value_name = "LEVEL",
        global = true
    )]
    pub log_level: Option<LevelFilter>,
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

                let results: SearchResults<'_> = search::search(&request)
                    .with_context(|| format!("❌ Search failed: {:#?}", request))?;

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
                        "❌ Syncing {:#?} to {:#?} failed!",
                        source_path, destination_path
                    )
                })?;

                println!("\n✔️ Sync complete! ✔️");
                Ok(())
            }
        }
    }

    pub fn initialize_logger(&self) {
        let level = match self.options.log_level {
            Some(logging_level) => logging_level,
            None => match self.options.verbose {
                0 => LevelFilter::Error,
                1 => LevelFilter::Warn,
                2 => LevelFilter::Info,
                3 => LevelFilter::Debug,
                4..=std::u8::MAX => LevelFilter::Trace,
            },
        };

        env_logger::Builder::new().filter_level(level).init();

        info!("ℹ️ logging initialized at level {}", level);
    }
}
