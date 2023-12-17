use super::SearchQuery;
use super::{data::Album, SearchResults};
use anyhow::{Context, Result};
use log::debug;
use std::{
    fs::File,
    io::{self, BufRead},
    str::FromStr,
};

/// Search a `.tsv` file
pub(crate) fn search<'a>(query: &'a SearchQuery<'a>) -> Result<SearchResults<'a>> {
    debug!("search called with: {:#?}", query);

    let file: File = File::open(query.file)
        .with_context(|| format!("Failed to open File from PathBuf: {:#?}", &query.file))?;

    let lines = io::BufReader::new(file).lines();

    let mut results: Vec<Album> = Vec::new();
    let query_upper: &String = &query.search_term.to_uppercase();

    for line in lines {
        let row: String = line.with_context(|| {
            format!(
                "Failed to get line from BufReader of file {}",
                &query.file.file_name().unwrap().to_str().unwrap()
            )
        })?;

        // if the text contains the search term, parse it and add it to the matches
        if row.to_uppercase().contains(query_upper) {
            let album: Album = Album::from_str(&row)
                .with_context(|| format!("failed to parse value to Album: {}", &row))?;

            results.push(album);
        }
    }

    debug!("Found {} matches.", results.len());

    Ok(SearchResults {
        results: Album::sort_by_field(results, query.sort),
        search_term: query.search_term,
        include_header: query.include_header,
        include_playlist_name: query.include_playlist_name,
        sort: query.sort,
    })
}
