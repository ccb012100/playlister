use crate::{
    data::{
        Album, AlbumArtists, AlbumName, DateAdded, FilterField, Playlist, ReleaseYear, TrackCount,
    },
    search::data::SearchRequest,
};
use anyhow::Result;
use rusqlite::{Connection, OpenFlags};
use std::path::PathBuf;

#[derive(Debug, Clone, PartialEq, Eq)]
pub enum AlbumSelection {
    All,
    Starred,
}

/// Search a `sqlite` file
pub fn search<'a>(request: &'a SearchRequest<'a>) -> Result<Vec<Album>, anyhow::Error> {
    debug_assert_ne!(request.filters.len(), 0);

    let conn = Connection::open_with_flags(
        request.source,
        OpenFlags::SQLITE_OPEN_READ_ONLY
            | OpenFlags::SQLITE_OPEN_URI
            | OpenFlags::SQLITE_OPEN_NO_MUTEX,
    )?;

    let mut stmt = conn.prepare(&search_albums_query(request))?;

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

    let mut x: Vec<Album> = Vec::new();

    for album in albums {
        x.push(album?);
    }

    Ok(x)
}

pub fn get_albums(
    db: &PathBuf,
    limit: usize,
    offset: usize,
    selection: &AlbumSelection,
) -> Result<Vec<Album>, anyhow::Error> {
    let conn = Connection::open_with_flags(
        db,
        OpenFlags::SQLITE_OPEN_READ_ONLY
            | OpenFlags::SQLITE_OPEN_URI
            | OpenFlags::SQLITE_OPEN_NO_MUTEX,
    )?;

    let mut stmt = conn.prepare(&select_from_albums_query(selection))?;

    let albums = stmt.query_map(
        &[
            (":limit", &limit.to_string()),
            (":offset", &offset.to_string()),
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

    let mut x: Vec<Album> = Vec::new();

    for album in albums {
        x.push(album?);
    }

    Ok(x)
}

fn select_from_albums_query(selection: &AlbumSelection) -> String {
    format!(
        "select GROUP_CONCAT(artist, '; ') as artists, album, track_count, release_date, added_at, playlist
from
(
    select
        art.name as artist,
        a.name as album,
        a.id as album_id,
        a.total_tracks as track_count,
        substr(a.release_date, 1, 4) as release_date,
        pt.added_at,
        p.name as playlist,
        p.id as playlist_id
    from Album a
    join albumartist aa on aa.album_id = a.id
    join artist art on art.id = aa.artist_id
    join track t on t.album_id = a.id
    join playlisttrack pt on pt.track_id = t.id
    join playlist p on p.id = pt.playlist_id
    {}
    group by a.id, art.id, p.id
    order by p.id, a.id, art.name
)
group by album_id, playlist_id
order by added_at DESC
limit :limit OFFSET :offset",
        match selection {
            AlbumSelection::All => "",
            AlbumSelection::Starred => "where p.name like 'starred%'",
        }
    )
}

fn search_albums_query(request: &SearchRequest) -> String {
    let mut where_clauses = Vec::<&str>::new();

    if request.filters.contains(&FilterField::Album) {
        where_clauses.push("a.name like format('%%%s%%', :term)");
    }
    if request.filters.contains(&FilterField::Artists) {
        where_clauses.push("art.name like format('%%%s%%', :term)");
    }
    if request.filters.contains(&FilterField::Playlist) {
        where_clauses.push("p.name like format('%%%s%%', :term)");
    }

    format!(
    "select GROUP_CONCAT(artist, '; ') as artists, album, track_count, release_date, added_at, playlist
from
(
    select
        art.name as artist,
        a.name as album,
        a.id as album_id,
        a.total_tracks as track_count,
        substr(a.release_date, 1, 4) as release_date,
        pt.added_at,
        p.name as playlist,
        p.id as playlist_id
    from Album a
    join albumartist aa on aa.album_id = a.id
    join artist art on art.id = aa.artist_id
    join track t on t.album_id = a.id
    join playlisttrack pt on pt.track_id = t.id
    join playlist p on p.id = pt.playlist_id
    where ({}) {}
    group by a.id, art.id, p.id
    order by p.id, a.id, art.name
)
group by album_id, playlist_id
order by added_at DESC
limit :limit OFFSET :offset",
    match where_clauses.len() {
        1 => where_clauses[0].to_string(),
        _ => where_clauses.join(" or ").to_string(),
    },
    match request.selection {
        AlbumSelection::All => "",
        AlbumSelection::Starred =>
        if request.filters.contains(&FilterField::Playlist) { "" } else {"and p.name like 'starred%'"}
    })
}
