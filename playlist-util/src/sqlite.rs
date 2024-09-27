use crate::data::*;
use search::SearchRequest;

use anyhow::Result;
use log::debug;
use rusqlite::{Connection, OpenFlags, Params};
use std::path::PathBuf;

#[derive(Debug, Clone, PartialEq, Eq)]
pub enum AlbumSelection {
    All,
    Starred,
}

pub fn get_albums_by_added_at_desc(
    db: &PathBuf,
    limit: usize,
    offset: usize,
    selection: &AlbumSelection,
) -> Result<Vec<Album>, anyhow::Error> {
    let query = format!(
        "select * from PlaylistAlbum {} ORDER BY added_at DESC limit :limit OFFSET :offset;",
        match selection {
            AlbumSelection::All => "",
            AlbumSelection::Starred => "where playlist like 'starred%'",
        }
    );

    let params = &[
        (":limit", &limit.to_string()),
        (":offset", &offset.to_string()),
    ];

    run_db_query(&query, db, params)
}

// Search for albums in `sqlite` file
pub fn get_albums_from_database<'a>(
    request: &'a SearchRequest<'a>,
) -> Result<Vec<Album>, anyhow::Error> {
    debug_assert_ne!(request.filters.len(), 0);

    let query = build_search_query(request);

    let params = &[
        (":limit", &100.to_string()),
        (":offset", &0.to_string()),
        (":term", &request.search_term.to_string()),
    ];

    run_db_query(&query, request.source, params)
}

fn run_db_query<'a, T: Params>(
    query: &'a str,
    db: &'a PathBuf,
    params: T,
) -> std::result::Result<Vec<Album>, anyhow::Error> {
    let conn = Connection::open_with_flags(
        db,
        OpenFlags::SQLITE_OPEN_READ_ONLY
            | OpenFlags::SQLITE_OPEN_URI
            | OpenFlags::SQLITE_OPEN_NO_MUTEX,
    )?;

    let mut stmt = conn.prepare(query)?;

    let albums = stmt.query_map(params, |row| {
        Ok(Album::new(
            AlbumName(row.get(1)?),
            AlbumArtists(row.get(0)?),
            TrackCount(row.get::<usize, u16>(2)?),
            ReleaseYear(row.get::<usize, String>(3)?.parse::<i32>().unwrap()),
            DateAdded(row.get(5)?),
            Playlist(row.get(4)?),
        )
        .unwrap_or_else(|_| panic!("row was not a valid Album: {:#?}", row)))
    })?;

    let mut x: Vec<Album> = Vec::new();

    for album in albums {
        x.push(album.unwrap());
    }

    Ok(x)
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
        "select * from PlaylistAlbum where ({}) {} order by {} limit :limit OFFSET :offset;",
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
            SortField::Playlist => "playlist, artists, album",
            SortField::Year => "release_year, artists, album",
        }
    )
}
