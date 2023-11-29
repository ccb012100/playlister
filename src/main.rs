use crate::{
    output::Output,
    search::data::{SearchQuery, SearchType},
};
use anyhow::Error;
use clap::Parser;
use cli::{get_path, Cli, Subcommands};
use log::{debug, info, LevelFilter};
use std::{path::PathBuf, process::ExitCode};

mod cli;
mod output;
mod search;

fn main() -> core::result::Result<ExitCode, Error> {
    let cli = Cli::parse();

    initialize_logger(&cli.verbose);

    debug!("parsed Cli: {:#?}", &cli);

    parse_cli_command(&cli.subcommand)
}

fn parse_cli_command(subcommand: &Subcommands) -> Result<ExitCode, Error> {
    match subcommand {
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
                sort: search::data::SortFields::from(*sort),
            };

            let results: search::data::SearchResults<'_> = search::search(&query)?;

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

    Ok(ExitCode::SUCCESS)
}

fn initialize_logger(verbosity: &u8) {
    let log_level = match &verbosity {
        0 => LevelFilter::Error,
        1 => LevelFilter::Warn,
        2 => LevelFilter::Info,
        3 => LevelFilter::Debug,
        4..=std::u8::MAX => LevelFilter::Trace,
    };

    env_logger::Builder::new().filter_level(log_level).init();

    info!("logging initialized at level {}", log_level);
}
