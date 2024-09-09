use crate::data::{Album, AlbumArtists, AlbumName, DateAdded, Playlist, ReleaseYear, TrackCount};
use anyhow::Result;
use rusqlite::{Connection, OpenFlags};
use std::path::PathBuf;

#[derive(Debug, Clone, PartialEq, Eq)]
pub enum AlbumSelection {
    All,
    Starred,
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

    let mut stmt = conn.prepare(&format!(
        "select * from AlbumsView {} limit :limit OFFSET :offset",
        match selection {
            AlbumSelection::All => "",
            AlbumSelection::Starred => "where playlist like 'starred%'",
        }
    ))?;

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
