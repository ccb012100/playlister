use crate::{cli::LogLevel, output::Output, search::SearchType};
use anyhow::Error;
use clap::Parser;
use cli::{get_path, Cli, Subcommands};
use log::{debug, info, LevelFilter};
use search::SearchQuery;
use std::{path::PathBuf, process::ExitCode};

mod cli;
mod output;
mod search;

fn main() -> core::result::Result<ExitCode, Error> {
    let cli = Cli::parse();

    let log_level = match cli.verbose {
        LogLevel::Debug => LevelFilter::Debug,
        LogLevel::Error => LevelFilter::Error,
        LogLevel::Info => LevelFilter::Info,
        LogLevel::Warn => LevelFilter::Warn,
        LogLevel::Off => LevelFilter::Off,
        LogLevel::Trace => LevelFilter::Trace,
    };

    env_logger::Builder::new().filter_level(log_level).init();

    debug!("logging initialized");
    debug!("parsed Cli: {:#?}", &cli);

    match &cli.command {
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

            let path: PathBuf = get_path(file_name, file_type)?;

            let query: SearchQuery = SearchQuery {
                search_term: &term.join(" "),
                search_type: match &file_type {
                    cli::FileType::Db => SearchType::Db,
                    cli::FileType::Tsv => SearchType::Tsv,
                },
                file: &path,
                include_header: *include_header,
                include_playlist_name: *include_playlist_name,
                sort: search::SortFields::from(*sort),
            };

            let results: search::SearchResults<'_> = search::search(&query)?;

            match no_format {
                true => Output::search_results(&results),
                false => Output::search_results_table(&results),
            }
        }
        Subcommands::Sync {
            file_name,
            file_type,
        } => {
            info!("Syncing...");

            let _path: PathBuf = get_path(file_name, file_type)?;
            todo!()
        }
    }

    info!("Done!");
    Ok(ExitCode::SUCCESS)
}
