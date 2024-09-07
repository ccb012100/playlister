pub mod data;
mod sqlite;
mod tsv;

use anyhow::{Context, Result};
use data::LastQuery;
use log::{debug, trace};

use self::data::{SearchQuery, SearchResults, SearchFileType};

pub fn search<'a>(query: &'a SearchQuery<'a>) -> Result<SearchResults<'a>> {
    trace!("🪵 search called with: {:#?}", query);

    match query.search_type {
        SearchFileType::Sqlite => {
            debug!("Searching SQLite DB...");
            sqlite::search(query).with_context(|| format!("Search failed: {:#?}", query))
        }
        SearchFileType::Tsv => {
            debug!("Searching TSV...");
            tsv::search(query).with_context(|| format!("Search failed: {:#?}", query))
        }
    }
}

pub fn last<'a>(query: &'a LastQuery<'a>) -> Result<Vec<crate::data::Album>> {
    trace!("🪵 search called with: {:#?}", query);

    match query.source_file_type {
        SearchFileType::Sqlite => {
            debug!("Getting last n albums from SQLite DB...");
            sqlite::last(query).with_context(|| format!("LastQuery failed: {:#?}", query))
        }
        SearchFileType::Tsv => unimplemented!("The SQLite data is the source of truth"),
    }
}
