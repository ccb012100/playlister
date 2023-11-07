use crate::{output::Output, search::SearchType};
use anyhow::Error;
use clap::Parser;
use cli::{get_path, Cli, Commands};
use search::SearchQuery;
use std::{path::PathBuf, process::ExitCode};

mod cli;
mod output;
mod search;

fn main() -> core::result::Result<ExitCode, Error> {
    #[cfg(windows)]
    ansi_term::enable_ansi_support();

    let cli = Cli::parse();

    let path: PathBuf = get_path(&cli.file_name, cli.file_type)?;

    match &cli.command {
        Commands::Search {
            include_header,
            include_playlist_name,
            no_format,
            sort,
            term,
        } => {
            let search_term = term.join(" ");

            if cli.verbose {
                Output::info(&format!("Searching for '{}'...", search_term));
            }

            let query: SearchQuery = SearchQuery {
                search_term: &term.join(" "),
                search_type: match &cli.file_type {
                    cli::FileType::Db => SearchType::Db,
                    cli::FileType::Tsv => SearchType::Tsv,
                },
                file: &path,
                include_header: *include_header,
                include_playlist_name: *include_playlist_name,
                sort: search::SortFields::from(*sort),
                verbose: cli.verbose,
            };

            if cli.verbose {
                Output::info(&format!("Search query: {:#?}", query));
            }

            match search::search(&query) {
                Ok(results) => match no_format {
                    true => Output::search_results(&results),
                    false => Output::search_results_table(&results),
                },
                Err(err) => {
                    return Err(err);
                }
            }
        }
        Commands::Sync {} => {
            if cli.verbose {
                Output::info("Syncing...");
            }
            todo!()
        }
    }

    if cli.verbose {
        Output::success("Done!");
    }
    Ok(ExitCode::SUCCESS)
}
