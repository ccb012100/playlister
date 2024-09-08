use crate::{data::Album, sqlite};

use super::data::{LastAlbumsRequest, SearchRequest, SearchResults};
use anyhow::Error;
use log::{debug, trace};

/// Search a `sqlite` database
pub fn search<'a>(request: &'a SearchRequest<'a>) -> Result<SearchResults, Error> {
    trace!("🪵 Searching SQLite DB: {:#?}", request);

    let mut results = sqlite::search(request)?;

    debug!("🪵 Found {} matches.", results.len());

    Album::filter_by_field(&mut results, request.search_term, &request.filters);

    debug!("🪵 Found {} matches after filtering.", results.len());

    Album::sort_by_field(&mut results, &request.sort);

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

    sqlite::get_albums(request.source, request.num, 0, &request.selection)
}
