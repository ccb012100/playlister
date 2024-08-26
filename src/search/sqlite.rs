use super::data::{SearchQuery, SearchResults};
use log::debug;
use anyhow::Error;

/// Search a `sqlite` database
#[allow(unused_variables)]
pub fn search<'a>(query: &'a SearchQuery<'a>) -> Result<SearchResults, Error> {
    debug!("🪵 Searching SQLite DB: {:#?}", query);

    todo!()
}
