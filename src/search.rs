mod db_search;
mod tsv_search;

use std::{error::Error, path::PathBuf};

use crate::output::Output;

pub(crate) fn search(query: &SearchQuery) -> Result<SearchResults, Box<dyn Error>> {
    match query.search_type {
        SearchType::Db => {
            Output::info("Searching DB...");
            db_search::search(query)
        }
        SearchType::Tsv => {
            Output::info("Searching TSV...");
            tsv_search::search(query)
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
pub(crate) struct SearchResults {
    pub results: Vec<String>,
    pub include_playlist_name: bool,
    pub sort: SortFields,
    pub search_term: String,
}

#[allow(dead_code)]
#[derive(Debug, Clone)]
pub(crate) struct Album {
    // row[0]
    pub artists: String,
    // row[1]
    pub album: String,
    // row[2]
    pub tracks: u8,
    // row[3]
    pub year_released: String,
    // row[4]
    pub date_added: String,
    // row[5]
    pub playlist: String,
}

impl Album {
    fn is_valid(value: &str) -> bool {
        let fields: Vec<&str> = value.split('\t').collect();

        if fields.len() != 6 {
            Output::error(&format!(
                "Value \"{}\" can not be split into 6 tab-separated values.",
                value
            ));
            return false;
        }

        match str::parse::<u8>(fields[2]) {
            Ok(_) => true,
            Err(_) => {
                Output::error(&format!("Value \"{}\" has an invalid track count.", value));
                false
            }
        }
    }
}
