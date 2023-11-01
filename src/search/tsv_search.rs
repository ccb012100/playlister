use super::SearchQuery;
use super::{album::Album, SearchResults};
use std::{
    fs::File,
    io::{self, BufRead, Error},
    str::FromStr,
};

pub(crate) fn search(query: &SearchQuery) -> Result<SearchResults, Error> {
    let file = File::open(&query.file)?;

    let lines = io::BufReader::new(file).lines();

    let mut results: Vec<Album> = Vec::new();
    let query_upper: &String = &query.search_term.to_uppercase();

    for line in lines {
        match line {
            Ok(row) => {
                if row.to_uppercase().contains(query_upper) {
                    // parse row and add to matches
                    match Album::from_str(&row) {
                        Ok(album) => results.push(album),
                        Err(err) => return Err(err),
                    }
                }
            }
            Err(err) => return Err(err),
        };
    }


    Ok(SearchResults {
        results: Album::sort_by_field(results, query.sort),
        search_term: query.search_term.clone(),
        include_playlist_name: query.include_playlist_name,
        sort: query.sort,
    })
}
