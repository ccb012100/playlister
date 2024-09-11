use anyhow::Result;
use log::trace;
use std::path::PathBuf;

use crate::{data::Album, sqlite};

pub fn get_the_most_recent_starred_albums(db: &PathBuf, offset: usize) -> Result<Vec<Album>> {
    trace!(
        "🪵 get_the_most_recent_starred_albums called with db={:#?}, offset={}",
        db,
        offset
    );

    let limit = 25;

    sqlite::get_albums(db, limit, offset, &sqlite::AlbumSelection::Starred)
}
