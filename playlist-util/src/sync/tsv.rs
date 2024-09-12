use crate::sync::anyhow;
use anyhow::{self, Context, Result};
use log::{debug, trace};
use std::{
    fs::OpenOptions,
    io::{BufRead, BufReader, Write},
    path::Path,
    str::FromStr,
};

use crate::data::{Album, AlbumTsv};

/// Get the last album added to the TSV file represented by `file`
pub fn get_last_album_added(file: &Path) -> Result<AlbumTsv> {
    let open_file = OpenOptions::new().read(true).open(file).with_context(|| {
        format!(
            "❌ Failed to open file from PathBuf for reading: {:#?}",
            &file
        )
    })?;

    let lines: Vec<_> = BufReader::new(open_file)
        .lines()
        .map(|line| line.unwrap())
        .collect();

    let last_entry = lines
        .last()
        .with_context(|| format!("❌ Failed to get last entry from PathBuf: {:#?}", &file))?;

    Ok(AlbumTsv::from_str(last_entry)?)
}

/// Append the `albums` to the end of `file`, oldest to newest.
///
/// This does not assume any sort order to `albums`.
pub fn add_albums_to_file(mut albums: Vec<Album>, file: &Path) -> Result<()> {
    trace!(
        "🪵 add_albums_to_file called with: albums={:#?} file={:#?}",
        albums,
        file
    );

    if albums.is_empty() {
        return Err(anyhow!(
            "\n❌ add_albums_to_file called with empty albums vector!"
        ));
    }

    Album::sort_by_field(&mut albums, &crate::data::SortField::Added);

    debug!("sorted albums: {:#?}", albums);

    let mut tsvs = Vec::<AlbumTsv>::new();

    albums.into_iter().for_each(|album| {
        #[cfg(debug_assertions)]
        {
            match album.validate() {
                Ok(_) => {}
                Err(err) => panic!("{}", err),
            }
        }
        tsvs.push(album.to_tsv_entry());
    });

    let mut open_file = OpenOptions::new()
        .append(true)
        .open(file)
        .with_context(|| {
            format!(
                "❌ Failed to open file from PathBuf for writing: {:#?}",
                &file
            )
        })?;

    for t in &tsvs {
        write!(open_file, "\n{}", t.0)?;
    }

    println!(
        "\n📝 Wrote {} albums to file {} 📝",
        tsvs.len(),
        file.display()
    );

    Ok(())
}
