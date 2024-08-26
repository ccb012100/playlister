use anyhow::{anyhow, Context, Ok, Result};
use data::Album;
use log::debug;
use std::path::Path;

pub mod data;
mod sqlite;
mod tsv;

pub fn sync(source: &Path, destination: &Path) -> Result<()> {
    debug!(
        "🪵 sync called with: source={:#?} destination={:#?}",
        source, destination
    );

    let last_added_to_tsv = tsv::get_last_album_added(destination).with_context(|| {
        format!(
            "❌ Failed to get most recent addition to tsv: {:#?} ❌",
            destination
        )
    })?;

    println!(
        "\nlast album added to {:#?}:\n\n\t{}",
        destination, last_added_to_tsv
    );

    let mut starred_albums: Vec<Album> = sqlite::get_the_most_recent_starred_albums(source, 0)
        .with_context(|| {
            format!(
                "❌ Failed to get the most recent starred albums: {:#?} ❌",
                destination
            )
        })?;

    debug!(
        "🪵 most recent starred albums, offset=0: {:#?}",
        starred_albums
    );

    let mut albums_to_add: Vec<Album> = Vec::new();
    let mut offset = 0;
    let mut found_match = false;
    let max_starred_albums_to_fetch = 100;
    let last_album_added = last_added_to_tsv.to_album();

    while !found_match && !starred_albums.is_empty() && offset < max_starred_albums_to_fetch {
        offset += starred_albums.len();

        for album in starred_albums {
            if album == last_album_added {
                debug!("\n🪵 Found match: {:#?} 🔍", album);
                found_match = true;
                break;
            }

            albums_to_add.push(album);
        }

        if found_match {
            break;
        }

        starred_albums =
            sqlite::get_the_most_recent_starred_albums(source, offset).with_context(|| {
                format!(
                    "❌ Failed to get the most recent starred albums: {:#?} ❌",
                    destination
                )
            })?;

        debug!(
            "🪵 most recent starred albums, offset={}: {:#?}",
            offset, starred_albums
        );
    }

    if !found_match {
        Err(anyhow!(
            "\n❌ Searched {} records. Unable to find a match in {:#?} for: <{}> ❌",
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
