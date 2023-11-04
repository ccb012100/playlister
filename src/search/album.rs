use crate::search::SortFields;
use anyhow::anyhow;
use anyhow::Result;
use std::str::FromStr;

#[derive(Debug, Clone)]
pub(crate) struct Album {
    // row[0]
    pub artists: String,
    // row[1]
    pub album: String,
    // row[2]
    pub tracks: String,
    // row[3]
    pub year_released: String,
    // row[4]
    pub date_added: String,
    // row[5]
    pub playlist: String,
}

impl FromStr for Album {
    fn from_str(s: &str) -> Result<Self> {
        let fields: Vec<&str> = s.split('\t').collect();

        if fields.len() != 6 {
            return Err(anyhow!(format!("Value {} must contain 6 tab-separated fields", s)));
        }

        Ok(Album {
            artists: fields[0].to_owned(),
            album: fields[1].to_owned(),
            tracks: fields[2].to_owned(),
            year_released: fields[3].to_owned(),
            date_added: fields[4].to_owned(),
            playlist: fields[5].to_owned(),
        })
    }

    type Err = anyhow::Error;
}

impl Album {
    pub(crate) fn to_tsv(&self, include_playlist_name: bool) -> String {
        match include_playlist_name {
            true => format!(
                "{}\t{}\t{}\t{}\t{}\t{}",
                self.artists,
                self.album,
                self.tracks,
                self.year_released,
                self.date_added,
                self.playlist
            ),
            false => format!(
                "{}\t{}\t{}\t{}\t{}",
                self.artists, self.album, self.tracks, self.year_released, self.date_added,
            ),
        }
    }

    pub(crate) fn sort_by_field(mut albums: Vec<Album>, sortfield: SortFields) -> Vec<Album> {
        match sortfield {
            SortFields::Artists => albums.sort_by(|a, b| {
                (
                    &a.artists,
                    &a.album,
                    &a.year_released,
                    &a.date_added,
                    &a.playlist,
                )
                    .cmp(&(
                        &b.artists,
                        &b.album,
                        &b.year_released,
                        &b.date_added,
                        &b.playlist,
                    ))
            }),
            SortFields::Album => albums.sort_by(|a, b| {
                (
                    &a.album,
                    &a.artists,
                    &a.year_released,
                    &a.date_added,
                    &a.playlist,
                )
                    .cmp(&(
                        &b.album,
                        &b.artists,
                        &b.year_released,
                        &b.date_added,
                        &b.playlist,
                    ))
            }),
            SortFields::Year => albums.sort_by(|a, b| {
                (
                    &a.year_released,
                    &a.artists,
                    &a.album,
                    &a.date_added,
                    &a.playlist,
                )
                    .cmp(&(
                        &b.year_released,
                        &b.artists,
                        &b.album,
                        &b.date_added,
                        &b.playlist,
                    ))
            }),
            SortFields::Added => albums.sort_by(|a, b| {
                (
                    &a.date_added,
                    &a.artists,
                    &a.album,
                    &a.year_released,
                    &a.playlist,
                )
                    .cmp(&(
                        &b.date_added,
                        &b.artists,
                        &b.album,
                        &b.year_released,
                        &b.playlist,
                    ))
            }),
        }

        albums
    }
}
