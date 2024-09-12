use crate::data::Album;

use anyhow::{anyhow, Context, Ok, Result};
use log::{debug, trace};
use std::path::PathBuf;

mod tsv;

/// Sync `source` and `destination`
pub fn sync(source: &PathBuf, destination: &PathBuf) -> Result<()> {
    trace!(
        "🪵 sync called with: source={:#?} destination={:#?}",
        source,
        destination
    );

    const LIMIT: usize = 25;
    const MAX_STARRED_ALBUMS_TO_FETCH: usize = LIMIT * 2;

    let last_added_to_tsv = tsv::get_last_album_added(destination).with_context(|| {
        format!(
            "❌ Failed to get most recent addition to tsv: {:#?}",
            destination
        )
    })?;

    println!(
        "\nlast album added to {:#?}:\n\n\t{}",
        destination, last_added_to_tsv
    );

    let last_album_added = last_added_to_tsv.to_album();

    debug!("\n🪵 Last album as Album: {:#?} 🔍", last_album_added);

    let mut albums_to_add: Vec<Album> = Vec::new();
    let mut found_match = false;
    let mut offset = 0;

    while !found_match && offset < MAX_STARRED_ALBUMS_TO_FETCH {
        let starred_albums: Vec<Album> = crate::sqlite::get_albums_by_added_at_desc(
                source,
                LIMIT,
                offset,
                &crate::sqlite::AlbumSelection::Starred,
            )
            .with_context(|| {
                 format!(
                    "❌ Failed to get the most recent starred albums from source={:#?} limit={} offset={}",
                    source,
                    LIMIT,
                    offset
                )
            })?;

        if starred_albums.is_empty() {
            break;
        }

        offset += starred_albums.len();

        for album in starred_albums {
            if album == last_album_added {
                debug!("\n🪵 Found match: {:#?} 🔍", album);
                found_match = true;
                break;
            }

            albums_to_add.push(album);
        }
    }

    if !found_match {
        Err(anyhow!(
            "\n❌ Searched {} records. Unable to find a match in {:#?} for: <{}>",
            offset,
            source,
            last_added_to_tsv,
        ))
    } else if albums_to_add.is_empty() {
        println!("\n📢 Nothing to add; up to date! 📢");
        Ok(())
    } else {
        println!(
            "\nℹ️ {} albums to sync from {:#?}:\n",
            albums_to_add.len(),
            source
        );

        for a in &albums_to_add {
            println!("\t{}", a);
        }

        tsv::add_albums_to_file(albums_to_add, destination)
    }
}
