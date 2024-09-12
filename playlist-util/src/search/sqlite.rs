use crate::data::search::{LastAlbumsRequest, SearchRequest, SearchResults};
use crate::{data::Album, sqlite};

use anyhow::Error;
use log::{debug, trace};

/// Search a `sqlite` database
pub fn search<'a>(request: &'a SearchRequest<'a>) -> Result<SearchResults, Error> {
    trace!("🪵 Searching SQLite DB: {request:#?}");

    let results = sqlite::get_albums_from_database(request)?;

    debug!("🪵 Found {} matches.", results.len());

    Ok(SearchResults {
        results,
        search_term: request.search_term,
        include_header: request.include_header,
        include_playlist_name: request.include_playlist_name,
        sort: request.sort,
    })
}

/// Get the last n albums from a `sqlite` database
pub fn last<'a>(request: &'a LastAlbumsRequest<'a>) -> Result<Vec<Album>, Error> {
    trace!("🪵 Getting last n albums from SQLite DB: {:#?}", request);

    sqlite::get_albums_by_added_at_desc(request.source, request.num, 0, &request.selection)
}
