use super::data::{SearchQuery, SearchResults};
use log::debug;
use std::io::Error;

/// Search a `sqlite` database
#[allow(unused_variables)]
pub(crate) fn search<'a>(query: &'a SearchQuery<'a>) -> Result<SearchResults, Error> {
    debug!("Searching SQLite DB: {:#?}", query);

    todo!()
}
