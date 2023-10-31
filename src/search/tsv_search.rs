use super::SearchQuery;
use super::SearchResults;
use std::{
    error::Error,
    fs::File,
    io::{self, BufRead},
};

use super::Album;

pub(crate) fn search(query: &SearchQuery) -> Result<SearchResults, Box<dyn Error>> {
    let file = File::open(&query.file)?;

    let lines = io::BufReader::new(file).lines();

    let mut results: Vec<String> = Vec::new();
    let query_upper: &String = &query.search_term.to_uppercase();

    for line in lines {
        match line {
            Ok(row) => {
                if row.to_uppercase().contains(query_upper) {
                    // parse row and add to matches
                    match Album::is_valid(&row) {
                        true => {
                            results.push(row);
                        }
                        false => panic!("Invalid row \"{}\"", row),
                    }
                }
            }
            Err(err) => return Err(Box::new(err)),
        };
    }

    Ok(SearchResults {
        results,
        search_term: query.search_term.clone(),
        include_playlist_name: query.include_playlist_name,
        sort: query.sort,
    })
}
