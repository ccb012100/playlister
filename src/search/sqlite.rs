use crate::{data::Album, sqlite};

use super::data::{LastQuery, SearchQuery, SearchResults};
use anyhow::Error;
use log::trace;

/// Search a `sqlite` database
pub fn search<'a>(query: &'a SearchQuery<'a>) -> Result<SearchResults, Error> {
    trace!("🪵 Searching SQLite DB: {:#?}", query);

    todo!()
}

/// Get the last n albums from a `sqlite` database
pub fn last<'a>(query: &'a LastQuery<'a>) -> Result<Vec<Album>, Error> {
    trace!("🪵 Getting last n albums from SQLite DB: {:#?}", query);

    sqlite::get_albums(query.source, query.num, 0, &query.selection)
}
