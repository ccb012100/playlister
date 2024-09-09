use crate::{
    data::{
        Album, AlbumArtists, AlbumName, DateAdded, FilterField, Playlist, ReleaseYear, SortField,
        TrackCount,
    },
    sqlite::{self, AlbumSelection},
};

use super::data::{LastAlbumsRequest, SearchRequest, SearchResults};
use anyhow::Error;
use log::{debug, trace};
use rusqlite::{Connection, OpenFlags};

/// Search a `sqlite` database
pub fn search<'a>(request: &'a SearchRequest<'a>) -> Result<SearchResults, Error> {
    trace!("🪵 Searching SQLite DB: {request:#?}");

    let results = get_albums_from_database(request)?;

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

    sqlite::get_albums(request.source, request.num, 0, &request.selection)
}

/// Search for albums in `sqlite` file
fn get_albums_from_database<'a>(
    request: &'a SearchRequest<'a>,
) -> Result<Vec<Album>, anyhow::Error> {
    debug_assert_ne!(request.filters.len(), 0);

    let conn = Connection::open_with_flags(
        request.source,
        OpenFlags::SQLITE_OPEN_READ_ONLY
            | OpenFlags::SQLITE_OPEN_URI
            | OpenFlags::SQLITE_OPEN_NO_MUTEX,
    )?;

    let mut stmt = conn.prepare(&build_search_query(request))?;

    let albums = stmt.query_map(
        &[
            (":limit", &100.to_string()),
            (":offset", &0.to_string()),
            (":term", &request.search_term.to_string()),
        ],
        |row| {
            Ok(Album::new(
                AlbumName(row.get(1)?),
                AlbumArtists(row.get(0)?),
                TrackCount(row.get::<usize, u16>(2)?),
                ReleaseYear(row.get::<usize, String>(3)?.parse::<i32>().unwrap()),
                DateAdded(row.get(4)?),
                Playlist(row.get(5)?),
            ))
        },
    )?;

    let mut results: Vec<Album> = Vec::new();

    for a in albums {
        results.push(a?);
    }

    Ok(results)
}

fn build_search_query(request: &SearchRequest) -> String {
    let mut where_clauses = Vec::<&str>::new();

    if request.filters.contains(&FilterField::Album) {
        where_clauses.push("album like format('%%%s%%', :term)");
    }
    if request.filters.contains(&FilterField::Artists) {
        where_clauses.push("artists like format('%%%s%%', :term)");
    }
    if request.filters.contains(&FilterField::Playlist) {
        where_clauses.push("playlist like format('%%%s%%', :term)");
    }

    format!(
        "select * from AlbumsView where ({}) {} group by {} limit :limit OFFSET :offset",
        match where_clauses.len() {
            1 => where_clauses[0].to_string(),
            _ => where_clauses.join(" or ").to_string(),
        },
        match request.selection {
            AlbumSelection::All => "",
            AlbumSelection::Starred =>
                if request.filters.contains(&FilterField::Playlist) {
                    debug!("Ignoring AlbumSelection::Starred because sort filters contains FilterField::Playlist");
                    ""
                } else {
                    "and playlist like 'starred%'"
                },
        },
        match request.sort {
            SortField::Added => "added_at",
            SortField::Album => "album, artists",
            SortField::Artists => "artists, album",
            SortField::Playlist => "playlists, artists, album",
            SortField::Year => "release_date, artists, album",
        }
    )
}
