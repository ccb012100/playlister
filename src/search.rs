mod album;
mod db_search;
mod tsv_search;

use self::album::Album;
use crate::output::Output;
use anyhow::Context;
use anyhow::Result;
use std::path::PathBuf;

pub(crate) fn search(query: &SearchQuery) -> Result<SearchResults> {
    match query.search_type {
        SearchType::Db => {
            Output::info("Searching DB...");
            db_search::search(query).with_context(|| format!("Search failed: {:#?}", query))
        }
        SearchType::Tsv => {
            Output::info("Searching TSV...");
            tsv_search::search(query).with_context(|| format!("Search failed: {:#?}", query))
        }
    }
}

#[derive(Debug, Clone)]
pub(crate) struct SearchQuery {
    pub search_term: String,
    pub search_type: SearchType,
    pub file: PathBuf,
    pub include_playlist_name: bool,
    pub sort: SortFields,
}

#[derive(Debug, Clone)]
pub(crate) struct SearchResults {
    pub results: Vec<Album>,
    pub include_playlist_name: bool,
    pub sort: SortFields,
    pub search_term: String,
}

#[derive(Clone, Copy, Debug)]
pub(crate) enum SearchType {
    Db,
    Tsv,
}

#[derive(Clone, Copy, Debug)]
pub(crate) enum SortFields {
    Artists,
    Album,
    Year,
    Added,
}
