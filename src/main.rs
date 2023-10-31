use std::process::exit;

use clap::Parser;
use cli::{get_path, Cli, Commands};
use search::SearchQuery;

use crate::{output::Output, search::SearchType};

mod cli;
mod output;
mod search;

fn main() {
    let cli = Cli::parse();

    let path = match get_path(&cli.file_name, cli.file_type) {
        Ok(p) => p,
        Err(err) => {
            Output::error(&format!(
                "Unable to construct a Path from \"{}\":\n- {}",
                cli.file_name, err
            ));
            exit(0)
        }
    };

    match &cli.command {
        Commands::Search {
            sort,
            include_playlist_name,
            term,
            no_format,
        } => {
            let search_term = term.join(" ");

            Output::info(&format!("Searching for '{}'...", search_term));

            if !matches!(sort, cli::SortFields::Artists) {
                todo!()
            }

            let query: SearchQuery = SearchQuery {
                search_term: term.join(" "),
                search_type: match &cli.file_type {
                    cli::FileType::Db => SearchType::Db,
                    cli::FileType::Tsv => SearchType::Tsv,
                },
                file: path,
                include_playlist_name: *include_playlist_name,
                sort: search::SortFields::from(*sort),
            };

            Output::info(&format!("Search query: {:#?}", query));

            match search::search(&query) {
                Ok(results) => match no_format {
                    true => Output::search_results(results),
                    false => Output::search_results_table(results),
                },
                Err(_err) => {
                    todo!()
                }
            }
        }
        Commands::Sync {} => {
            Output::info("Syncing...");
            todo!()
        }
    }

    Output::success("Done!")
}
