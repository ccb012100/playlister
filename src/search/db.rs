use super::data::{SearchQuery, SearchResults};
use log::debug;
use std::io::Error;

#[allow(unused_variables)]
pub(crate) fn search<'a>(query: &'a SearchQuery<'a>) -> Result<SearchResults, Error> {
    debug!("Searching DB: {:#?}", query);
    todo!()
}
