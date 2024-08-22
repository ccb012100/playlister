use anyhow::{anyhow, Context, Ok, Result};
use data::AlbumTsv;
use std::path::Path;

pub(crate) mod data;
mod sqlite;
mod tsv;

pub(crate) fn sync(source: &Path, destination: &Path) -> Result<()> {
    println!("SOURCE:\t\t {}", source.display());
    println!("DESTINATION:\t {}\n", destination.display());

    let last_added_to_tsv = tsv::get_last_album_added(destination).with_context(|| {
        format!(
            "❌ Failed to get most recent addition to tsv: {:#?} ❌",
            destination
        )
    })?;

    println!(
        "last album added to {:#?}: {}",
        destination, last_added_to_tsv
    );

    let mut starred_albums: Vec<AlbumTsv> = sqlite::get_the_most_recent_starred_albums(source, 0)
        .with_context(|| {
        format!(
            "❌ Failed to get the most recent starred albums: {:#?} ❌",
            destination
        )
    })?;

    let mut albums_to_add: Vec<AlbumTsv> = Vec::new();
    let mut offset = 0;
    let mut found_match = false;
    let max_starred_albums_to_fetch = 100;

    while !found_match && !starred_albums.is_empty() && offset < max_starred_albums_to_fetch {
        offset += starred_albums.len();

        for a in starred_albums {
            if a == last_added_to_tsv {
                println!("\n🔎 Found match: <{}> 🔍", a);
                found_match = true;
                break;
            }

            albums_to_add.push(a);
        }

        starred_albums =
            sqlite::get_the_most_recent_starred_albums(source, offset).with_context(|| {
                format!(
                    "❌ Failed to get the most recent starred albums: {:#?} ❌",
                    destination
                )
            })?;
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

        // albums_to_add.reverse();

        for a in &albums_to_add {
            println!("\t{}", a);
        }

        tsv::add_albums_to_file(albums_to_add, destination)
    }
}
