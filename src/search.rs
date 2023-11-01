mod db_search;
mod tsv_search;

use std::{
    io::{Error, ErrorKind},
    path::PathBuf,
    str::FromStr,
};

use crate::output::Output;

pub(crate) fn search(query: &SearchQuery) -> Result<SearchResults, Error> {
    match query.search_type {
        SearchType::Db => {
            Output::info("Searching DB...");
            db_search::search(query)
        }
        SearchType::Tsv => {
            Output::info("Searching TSV...");
            tsv_search::search(query)
        }
    }
}

#[derive(Debug, Clone)]
pub(crate) struct SearchQuery {
    pub search_term: String,
    pub search_type: SearchType,
    pub file: PathBuf,
    pub include_playlist_name: bool,
    pub sort: SortFields,
}

#[derive(Clone, Copy, Debug)]
pub(crate) enum SearchType {
    Db,
    Tsv,
}

#[derive(Clone, Copy, Debug)]
pub(crate) enum SortFields {
    Artists,
    Album,
    Released,
    Added,
}

#[derive(Debug, Clone)]
pub(crate) struct SearchResults {
    pub results: Vec<Album>,
    pub include_playlist_name: bool,
    pub sort: SortFields,
    pub search_term: String,
}

#[allow(dead_code)]
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
    type Err = Error;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let fields: Vec<&str> = s.split('\t').collect();

        if fields.len() != 6 {
            return Err(Error::new(
                ErrorKind::InvalidData,
                format!("Value {} must contain 6 tab-separated fields", s),
            ));
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
}
