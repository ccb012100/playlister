use std::path::PathBuf;

use crate::output::info_output;

#[derive(Clone, Copy, Debug)]
pub(crate) enum SearchType {
    Db,
    Tsv,
}

#[derive(Clone, Copy, Debug)]
pub(crate) enum SortFields {
    Artists,
    Album,
    Released,
    Added,
}

#[derive(Debug, Clone)]
pub(crate) struct SearchResults {}

#[allow(dead_code)]
#[derive(Debug, Clone)]
pub(crate) struct SearchQuery {
    pub term: String,
    pub search_type: SearchType,
    pub file: PathBuf,
    pub include_playlist_name: bool,
    pub sort: SortFields,
}

#[derive(Debug, Clone)]
pub(crate) struct SearchError;

pub(crate) fn search(query: &SearchQuery) -> Result<SearchResults, SearchError> {
    match query.search_type {
        SearchType::Db => {
            info_output("Searching DB...");
            todo!()
        }
        SearchType::Tsv => {
            info_output("Searching TSV...");
            todo!()
        }
    }
}
