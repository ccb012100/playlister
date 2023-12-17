pub(crate) mod data;
mod sqlite;
mod tsv;

use anyhow::Context;
use anyhow::Result;
use log::{debug, info};

use self::data::{SearchQuery, SearchResults, SearchType};

pub(crate) fn search<'a>(query: &'a SearchQuery<'a>) -> Result<SearchResults<'a>> {
    debug!("search called with: {:#?}", query);

    match query.search_type {
        SearchType::Sqlite => {
            info!("Searching SQLite DB...");
            sqlite::search(query).with_context(|| format!("Search failed: {:#?}", query))
        }
        SearchType::Tsv => {
            info!("Searching TSV...");
            tsv::search(query).with_context(|| format!("Search failed: {:#?}", query))
        }
    }
}
