use super::SearchRequest;
use super::SearchResults;
use crate::data::{Album, AlbumTsv};

use anyhow::{Context, Result};
use log::{debug, trace};
use std::{
    fs::{File, OpenOptions},
    io::{self, BufRead},
    str::FromStr,
};

/// Search a `.tsv` file
pub fn search<'a>(request: &'a SearchRequest<'a>) -> Result<SearchResults<'a>> {
    trace!("🪵 search called with: {:#?}", request);

    let file: File = OpenOptions::new()
        .read(true)
        .open(request.source)
        .with_context(|| format!("Failed to open File from PathBuf: {:#?}", &request.source))?;

    let lines = io::BufReader::new(file).lines();

    let mut results: Vec<Album> = Vec::new();
    let search_query_upper: &String = &request.search_term.to_uppercase();

    for line in lines {
        let row: String = line.with_context(|| {
            format!(
                "Failed to get line from BufReader of file {}",
                &request.source.file_name().unwrap().to_str().unwrap()
            )
        })?;

        // if the text contains the search term, parse it and add it to the matches
        if row.to_uppercase().contains(search_query_upper) {
            let album: Album = AlbumTsv::from_str(&row)
                .with_context(|| format!("failed to parse value to Album: {}", &row))?
                .to_album();

            results.push(album);
        }
    }

    debug!("🪵 Found {} matches.", results.len());

    Album::filter_by_field(&mut results, request.search_term, &request.filters);

    debug!("🪵 Found {} matches after filtering.", results.len());

    Album::sort_by_field(&mut results, &request.sort);

    Ok(SearchResults {
        results,
        search_term: request.search_term,
        include_header: request.include_header,
        include_playlist_name: request.include_playlist_name,
        sort: request.sort,
    })
}
