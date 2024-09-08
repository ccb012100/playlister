use crate::{
    data::{Album, FilterField, SortField},
    sqlite::AlbumSelection,
};
use std::path::PathBuf;

#[derive(Debug, Clone, PartialEq, Eq)]
pub struct SearchRequest<'a> {
    pub selection: AlbumSelection,
    pub filters: Vec<FilterField>,
    pub include_header: bool,
    pub include_playlist_name: bool,
    pub search_term: &'a str,
    pub search_type: SearchFileType,
    pub sort: SortField,
    pub source: &'a PathBuf,
}

#[derive(Debug, Clone, PartialEq, Eq)]
pub struct LastAlbumsRequest<'a> {
    pub num: usize,
    pub selection: AlbumSelection,
    pub source_file_type: SearchFileType,
    pub source: &'a PathBuf,
}

#[derive(Debug, Clone, PartialEq, Eq)]
pub struct SearchResults<'a> {
    pub include_header: bool,
    pub include_playlist_name: bool,
    pub results: Vec<Album>,
    pub search_term: &'a str,
    pub sort: SortField,
}

#[derive(Debug, Copy, Clone, PartialEq, Eq)]
pub enum SearchFileType {
    Sqlite,
    Tsv,
}

impl Album {
    /// `&self` -> `"{artists}\t{album}\t{tracks}\t{year_released}\t{date_added}\t{playlist}"`
    pub fn to_tsv_search_result(&self, include_playlist_name: bool) -> String {
        if include_playlist_name {
            format!(
                "{}\t{}\t{}\t{}\t{}\t{}",
                self.artists,
                self.name,
                self.tracks,
                self.release_year,
                self.date_added,
                self.playlist
            )
        } else {
            format!(
                "{}\t{}\t{}\t{}\t{}",
                self.artists, self.name, self.tracks, self.release_year, self.date_added,
            )
        }
    }
}
