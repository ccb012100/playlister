use crate::sync::anyhow;
use anyhow::{self, Context, Result};
use log::debug;
use std::{
    fs::OpenOptions,
    io::{BufRead, BufReader, Write},
    path::Path,
    str::FromStr,
};

use crate::sync::data::AlbumTsv;

use super::data::Album;

pub fn get_last_album_added(file: &Path) -> Result<AlbumTsv> {
    let open_file = OpenOptions::new().read(true).open(file).with_context(|| {
        format!(
            "❌ Failed to open file from PathBuf for reading: {:#?} ❌",
            &file
        )
    })?;

    let lines: Vec<_> = BufReader::new(open_file)
        .lines()
        .map(|line| line.unwrap())
        .collect();

    let last_entry = lines
        .last()
        .with_context(|| format!("❌ Failed to get last entry from PathBuf: {:#?} ❌", &file))?;

    Ok(AlbumTsv::from_str(last_entry)?)
}

/// Append the `albums` to the end of `file`, oldest to newest.
///
/// This does not assume any sort order to `albums`.
pub fn add_albums_to_file(albums: Vec<Album>, file: &Path) -> Result<()> {
    let mut tsvs = Vec::<AlbumTsv>::new();

    for album in albums {
        #[cfg(debug_assertions)]
        {
            match album.validate() {
                Ok(_) => {}
                Err(err) => panic!("{}", err),
            }
        }
        tsvs.push(album.to_tsv_entry());
    }

    add_tsv_albums_to_file(tsvs, file)
}

fn add_tsv_albums_to_file(albums: Vec<AlbumTsv>, file: &Path) -> Result<()> {
    debug!(
        "🪵 add_albums_to_file called with: albums={:#?} file={:#?}",
        albums, file
    );

    if albums.is_empty() {
        return Err(anyhow!(
            "\n❌ add_albums_to_file called with empty albums vector! ❌"
        ));
    }

    let mut open_file = OpenOptions::new()
        .append(true)
        .open(file)
        .with_context(|| {
            format!(
                "❌ Failed to open file from PathBuf for writing: {:#?} ❌",
                &file
            )
        })?;

    let mut sorted = albums.clone();

    sorted.sort_by(|a, b| {
        a.0.split('\t').collect::<Vec<&str>>()[4].cmp(b.0.split('\t').collect::<Vec<&str>>()[4])
    });

    debug!("sorted albums: {:#?}", sorted);

    for s in &sorted {
        s.validate()?;
        write!(open_file, "\n{}", s.0)?;
    }

    println!(
        "\n📝 Wrote {} albums to file {} 📝",
        sorted.len(),
        file.display()
    );

    Ok(())
}
