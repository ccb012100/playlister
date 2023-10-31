use std::process::exit;

use clap::Parser;
use cli::{get_path, Cli, Commands};
use output::error_output;
use search::SearchQuery;

use crate::{output::info_output, search::SearchType};

mod cli;
mod output;
mod search;

fn main() {
    let cli = Cli::parse();

    let path = match get_path(&cli.file_name, cli.file_type) {
        Ok(p) => p,
        Err(err) => {
            error_output(&format!(
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
        } => {
            info_output("Searching...");

            let query: SearchQuery = SearchQuery {
                term: term.join(" "),
                search_type: match &cli.file_type {
                    cli::FileType::Db => SearchType::Db,
                    cli::FileType::Tsv => SearchType::Tsv,
                },
                file: path,
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
