use crate::data::search::{LastAlbumsRequest, SearchFileType, SearchRequest, SearchResults};

use anyhow::{Context, Result};
use log::{debug, trace};

mod sqlite;
mod tsv;

pub fn search<'a>(request: &'a SearchRequest<'a>) -> Result<SearchResults<'a>> {
    trace!("🪵 search called with: {:#?}", request);

    match request.search_type {
        SearchFileType::Sqlite => {
            debug!("Searching SQLite DB...");
            sqlite::search(request).with_context(|| format!("Search failed: {:#?}", request))
        }
        SearchFileType::Tsv => {
            debug!("Searching TSV...");
            tsv::search(request).with_context(|| format!("Search failed: {:#?}", request))
        }
    }
}

pub fn last<'a>(request: &'a LastAlbumsRequest<'a>) -> Result<Vec<crate::data::Album>> {
    trace!("🪵 search called with: {:#?}", request);

    match request.source_file_type {
        SearchFileType::Sqlite => {
            debug!("Getting last n albums from SQLite DB...");
            sqlite::last(request)
                .with_context(|| format!("LastAlbumRequest failed: {:#?}", request))
        }
        SearchFileType::Tsv => unimplemented!("The SQLite data is the source of truth"),
    }
}
