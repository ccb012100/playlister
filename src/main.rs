use std::process::exit;

use clap::Parser;
use cli::{file_name_is_valid, Cli, Commands};
use output::error_output;
use search::SearchQuery;

use crate::{
    output::info_output,
    search::SearchType,
};

mod cli;
mod output;
mod search;

fn main() {
    let cli = Cli::parse();

    if !file_name_is_valid(&cli.file_name, cli.file_type) {
        error_output(&format!("Filename {} is not a valid file.", cli.file_name));
        exit(0)
    }

    match &cli.command {
        Commands::Search {
            sort,
            include_playlist_name,
            term,
        } => {
            info_output("Searching...");

            let query: SearchQuery = SearchQuery {
                term: term.join(" "),
                search_type: match &cli.file_type {
                    cli::FileType::Db => SearchType::Db,
                    cli::FileType::Tsv => SearchType::Tsv,
                },
                file_name: cli.file_name.to_string(),
                include_playlist_name: *include_playlist_name,
                sort: search::SortFields::from(*sort),
            };

            info_output(&format!("Search query: {:#?}", query));

            match search::search(&query) {
                Ok(_results) => todo!(),
                Err(_err) => {
                    todo!()
                }
            }
        }
        Commands::Sync {} => {
            info_output("Syncing...");
            todo!()
        }
    }
}
