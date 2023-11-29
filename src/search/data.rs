use anyhow::anyhow;
use anyhow::Result;
use std::{str::FromStr, path::PathBuf};

#[derive(Debug, Clone)]
pub(crate) struct SearchQuery<'a> {
    pub file: &'a PathBuf,
    pub include_header: bool,
    pub include_playlist_name: bool,
    pub search_term: &'a str,
    pub search_type: SearchType,
    pub sort: SortFields,
}

#[derive(Debug, Clone)]
pub(crate) struct SearchResults<'a> {
    pub include_header: bool,
    pub include_playlist_name: bool,
    pub results: Vec<Album>,
    pub search_term: &'a str,
    pub sort: SortFields,
}

#[derive(Clone, Copy, Debug)]
pub(crate) enum SearchType {
    Db,
    Tsv,
}

#[derive(Clone, Copy, Debug)]
pub(crate) enum SortFields {
    Added,
    Album,
    Artists,
    Year,
}

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
            return Err(anyhow!(format!(
                "Value {} must contain 6 tab-separated fields",
                s
            )));
        }

        let artists = fields[0];
        let album = fields[1];
        let tracks = fields[2];
        let year_released = fields[3];
        let date_added = fields[4];
        let playlist = fields[5];

        Ok(Album::new(
            album,
            artists,
            date_added,
            playlist,
            tracks,
            year_released,
        ))
    }

    type Err = anyhow::Error;
}

impl Album {
    pub(crate) fn new(
        album: &str,
        artists: &str,
        date_added: &str,
        playlist: &str,
        tracks: &str,
        year_released: &str,
    ) -> Self {
        Album {
            album: album.to_owned(),
            artists: artists.to_owned(),
            date_added: date_added.to_owned(),
            playlist: playlist.to_owned(),
            tracks: tracks.to_owned(),
            year_released: year_released.to_owned(),
        }
    }

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
