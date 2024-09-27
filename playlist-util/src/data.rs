use std::{
    fmt::{self},
    io::{Error, ErrorKind},
    result,
    str::FromStr,
};

use chrono::{Datelike, Utc};
use regex::Regex;

pub mod search;

/* Structs */

#[derive(Clone, Debug, PartialEq, Eq, PartialOrd, Ord)]
pub struct AlbumName(pub String);

#[derive(Clone, Debug, PartialEq, Eq, PartialOrd, Ord)]
pub struct AlbumArtists(pub String);

#[derive(Clone, Debug, PartialEq, Eq, PartialOrd, Ord)]
pub struct DateAdded(pub String);

#[derive(Clone, Debug, PartialEq, Eq, PartialOrd, Ord)]
pub struct Playlist(pub String);

#[derive(Clone, Copy, Debug, PartialEq, Eq, PartialOrd, Ord)]
pub struct ReleaseYear(pub i32);

#[derive(Clone, Copy, Debug, PartialEq, Eq, PartialOrd, Ord)]
pub struct TrackCount(pub u16);

#[derive(Clone, Debug, PartialEq, Eq, PartialOrd, Ord)]
pub struct AlbumTsv(pub String);

#[derive(Clone, Debug, PartialEq, Eq, PartialOrd, Ord)]
pub struct Album {
    /// The recording artist(s)
    pub artists: AlbumArtists, /* tsv_row_fields[0] */
    /// The name of the album
    pub name: AlbumName, /* tsv_row_fields[1] */
    /// The number of tracks on the album
    pub tracks: TrackCount, /* tsv_row_fields[2] */
    /// The year the album was released
    pub release_year: ReleaseYear, /* tsv_row_fields[3] */
    /// The date the album was added to the starred playlist, in format `YYYY-MM-DD HH:MM:SS`
    pub date_added: DateAdded, /* tsv_row_fields[4] */
    /// The name of the starred playlist the album was added to
    pub playlist: Playlist, /* tsv_row_fields[5] */
}

#[derive(Debug, Copy, Clone, PartialEq, Eq, PartialOrd, Ord)]
pub enum SortField {
    Added,
    Album,
    Artists,
    Playlist,
    Year,
}

#[derive(Debug, Copy, Clone, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub enum FilterField {
    Artists,
    Album,
    Playlist,
}

/* fmt::Display */

impl fmt::Display for Playlist {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "{}", self.0)
    }
}

impl fmt::Display for DateAdded {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "{}", self.0)
    }
}

impl fmt::Display for AlbumName {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "{}", self.0)
    }
}

impl fmt::Display for AlbumArtists {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "{}", self.0)
    }
}

impl fmt::Display for TrackCount {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "{}", self.0)
    }
}

impl fmt::Display for ReleaseYear {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "{}", self.0)
    }
}

impl fmt::Display for Album {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(
            f,
            r#"Album
{{
    name:       {}
    artist:     {}
    tracks:     {}
    released:   {}
    added:      {}
    playlist:   {}
}}"#,
            self.name, self.artists, self.tracks, self.release_year, self.date_added, self.playlist
        )
    }
}

impl fmt::Display for AlbumTsv {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "{}", self.0)
    }
}

/* FromStr */

impl FromStr for TrackCount {
    type Err = Error;

    fn from_str(s: &str) -> result::Result<TrackCount, Error> {
        match s.trim().parse::<u16>() {
            Ok(tc) => {
                if is_valid_track_count(&tc) {
                    Ok(TrackCount(tc))
                } else {
                    Err(invalid_data_error("Track Count must be between 1 and 350"))
                }
            }
            Err(err) => Err(Error::new(ErrorKind::InvalidData, err)),
        }
    }
}

impl FromStr for AlbumName {
    type Err = Error;

    fn from_str(s: &str) -> result::Result<Self, Self::Err> {
        if s.is_empty() {
            return Err(invalid_data_error("AlbumName cannot be empty"));
        }

        let trimmed = s.trim().to_string();

        if trimmed.is_empty() {
            return Err(invalid_data_error("AlbumName cannot be white space only"));
        }

        Ok(AlbumName(trimmed))
    }
}

impl FromStr for AlbumArtists {
    type Err = Error;

    fn from_str(s: &str) -> result::Result<Self, Self::Err> {
        if s.is_empty() {
            return Err(invalid_data_error("AlbumArtist cannot be empty"));
        }

        let trimmed = s.trim().to_string();

        if trimmed.is_empty() {
            return Err(invalid_data_error("AlbumArtist cannot be white space only"));
        }

        Ok(AlbumArtists(trimmed))
    }
}

impl FromStr for Playlist {
    type Err = Error;

    fn from_str(s: &str) -> result::Result<Self, Self::Err> {
        if s.is_empty() {
            return Err(invalid_data_error("Playlist cannot be empty"));
        }

        let trimmed = s.trim().to_string();

        if trimmed.is_empty() {
            Err(invalid_data_error("Playlist cannot be white space only"))
        } else {
            Ok(Playlist(trimmed))
        }
    }
}

impl FromStr for ReleaseYear {
    type Err = Error;

    fn from_str(s: &str) -> result::Result<Self, Self::Err> {
        match s.trim().parse::<i32>() {
            Ok(year) => {
                if is_valid_release_year(&year) {
                    Ok(ReleaseYear(year))
                } else {
                    Err(invalid_data_error(&format!("Invalid value: {}", year)))
                }
            }
            Err(err) => Err(Error::new(ErrorKind::InvalidData, err)),
        }
    }
}

impl FromStr for AlbumTsv {
    type Err = Error;

    /// Returns an `Error` result if the string is not valid
    fn from_str(s: &str) -> result::Result<Self, Self::Err> {
        match AlbumTsv::validate_str(s) {
            Ok(_) => Ok(AlbumTsv(s.to_string())),
            Err(err) => Err(Error::new(ErrorKind::InvalidData, err)),
        }
    }
}

impl FromStr for DateAdded {
    type Err = Error;

    fn from_str(s: &str) -> result::Result<Self, Self::Err> {
        if s.is_empty() {
            return Err(invalid_data_error("DateAdded cannot be empty"));
        }

        let trimmed = s.trim().to_string();

        if trimmed.is_empty() {
            return Err(invalid_data_error("DateAdded cannot be white space only"));
        }

        if is_valid_date_added(&trimmed) {
            Ok(DateAdded(trimmed))
        } else {
            Err(invalid_data_error(&format!(
                "DateAdded is invalid: '{}'. Must be in format YYYY-MM-DD HH:MM:SS",
                trimmed
            )))
        }
    }
}

/* Associated methods */

impl Album {
    /// Create new instance of `Album`.
    ///
    /// Returns an `Error` result if the data is invalid.
    pub fn new(
        name: AlbumName,
        artists: AlbumArtists,
        tracks: TrackCount,
        release_year: ReleaseYear,
        date_added: DateAdded,
        playlist: Playlist,
    ) -> anyhow::Result<Self> {
        let album = Self {
            name,
            artists,
            tracks,
            release_year,
            date_added,
            playlist,
        };

        album.validate()?;

        Ok(album)
    }

    pub fn to_tsv_entry(&self) -> AlbumTsv {
        AlbumTsv(format!(
            "{}\t{}\t{}\t{}\t{}\t{}",
            self.artists, self.name, self.tracks, self.release_year, self.date_added, self.playlist
        ))
    }

    /// sort `albums` by `sortfield`
    pub fn sort_by_field(albums: &mut [Album], sortfield: &SortField) {
        match sortfield {
            SortField::Artists => albums.sort_by(|a, b| {
                (
                    &a.artists,
                    &a.name,
                    &a.release_year,
                    &a.date_added,
                    &a.playlist,
                )
                    .cmp(&(
                        &b.artists,
                        &b.name,
                        &b.release_year,
                        &b.date_added,
                        &b.playlist,
                    ))
            }),
            SortField::Album => albums.sort_by(|a, b| {
                (
                    &a.name,
                    &a.artists,
                    &a.release_year,
                    &a.date_added,
                    &a.playlist,
                )
                    .cmp(&(
                        &b.name,
                        &b.artists,
                        &b.release_year,
                        &b.date_added,
                        &b.playlist,
                    ))
            }),
            SortField::Year => albums.sort_by(|a, b| {
                (
                    &a.release_year,
                    &a.artists,
                    &a.name,
                    &a.date_added,
                    &a.playlist,
                )
                    .cmp(&(
                        &b.release_year,
                        &b.artists,
                        &b.name,
                        &b.date_added,
                        &b.playlist,
                    ))
            }),
            SortField::Added => albums.sort_by(|a, b| {
                (
                    &a.date_added,
                    &a.artists,
                    &a.name,
                    &a.release_year,
                    &a.playlist,
                )
                    .cmp(&(
                        &b.date_added,
                        &b.artists,
                        &b.name,
                        &b.release_year,
                        &b.playlist,
                    ))
            }),
            SortField::Playlist => albums.sort_by(|a, b| {
                (
                    &a.playlist,
                    &a.artists,
                    &a.name,
                    &a.release_year,
                    &a.date_added,
                )
                    .cmp(&(
                        &b.playlist,
                        &b.artists,
                        &b.name,
                        &b.release_year,
                        &b.date_added,
                    ))
            }),
        }
    }

    /// filter `albums` on `filters`
    pub fn filter_by_field(albums: &mut Vec<Album>, search_term: &str, filters: &[FilterField]) {
        if albums.is_empty() || filters.is_empty() {
            return; // albums;
        }

        let term_caps = search_term.to_uppercase();

        albums.retain(
            |Album {
                 artists: artist,
                 name,
                 playlist,
                 ..
             }: &Album| {
                (filters.contains(&FilterField::Album)
                    && name.0.to_uppercase().contains(&term_caps))
                    || (filters.contains(&FilterField::Artists)
                        && artist.0.to_uppercase().contains(&term_caps))
                    || (filters.contains(&FilterField::Playlist)
                        && playlist.0.to_uppercase().contains(&term_caps))
            },
        );
    }

    fn validate(&self) -> anyhow::Result<()> {
        let mut errors = Vec::new();

        if self.name.0.trim().is_empty() {
            errors.push(format!("\tInvalid name: {}", self.name));
        }
        if self.artists.0.trim().is_empty() {
            errors.push(format!("\tInvalid artist: {}", self.artists));
        }
        if self.playlist.0.trim().is_empty() {
            errors.push(format!("\tInvalid name: {}", self.playlist));
        }
        if !is_valid_date_added(&self.date_added.0) {
            errors.push(format!("\tInvalid date_added: {}", self.date_added));
        }
        if !is_valid_release_year(&self.release_year.0) {
            errors.push(format!("\tInvalid release_year: {}", self.release_year));
        }
        if !is_valid_track_count(&self.tracks.0) {
            errors.push(format!("\tInvalid tracks: {}", self.tracks));
        }

        if errors.is_empty() {
            Ok(())
        } else {
            Err(anyhow::anyhow!("Failed validation:\n{}", errors.join("\n")))
        }
    }
}

impl AlbumTsv {
    pub fn to_album(&self) -> anyhow::Result<Album> {
        let parts: Vec<&str> = self.0.split('\t').collect();
        assert_eq!(parts.len(), 6);

        Album::new(
            AlbumName::from_str(parts[1]).unwrap(),
            AlbumArtists::from_str(parts[0]).unwrap(),
            TrackCount::from_str(parts[2]).unwrap(),
            ReleaseYear::from_str(parts[3]).unwrap(),
            DateAdded::from_str(parts[4]).unwrap(),
            Playlist::from_str(parts[5]).unwrap(),
        )
    }

    pub fn validate_str(s: &str) -> anyhow::Result<()> {
        let mut errors = Vec::<String>::new();

        if s.is_empty() {
            return Err(anyhow::anyhow!("Failed validation:\nCannot be empty"));
        } else if s.trim().is_empty() {
            return Err(anyhow::anyhow!(
                "Failed validation:\n\tCannot be white space only"
            ));
        }

        let parts: Vec<&str> = s.split('\t').collect();

        if parts.len() == 6 {
            for part in &parts {
                if part.trim().is_empty() {
                    errors.push("\tCannot have any empty fields.".to_string());
                }
            }

            if parts[2].parse::<u16>().is_err() {
                errors.push(format!("\tInvalid TrackCount value: {}", parts[2]));
            } else if parts[3].parse::<i32>().is_err() {
                errors.push(format!("\tInvalid ReleaseYear value: {}", parts[3]));
            }

            if errors.is_empty() {
                Ok(())
            } else {
                Err(anyhow::anyhow!("Failed validation:\n{}", errors.join("\n")))
            }
        } else {
            Err(anyhow::anyhow!(
                "Failed validation:\n\tMust be a string with 6 tab-separated values.".to_string()
            ))
        }
    }

    pub fn validate(&self) -> anyhow::Result<()> {
        AlbumTsv::validate_str(&self.0)
    }
}

fn invalid_data_error(s: &str) -> Error {
    Error::new(ErrorKind::InvalidData, s)
}

fn is_valid_track_count(u: &u16) -> bool {
    // 350 is somewhat arbitrary but seems sufficient
    (1..=350).contains(u)
}

fn is_valid_date_added(s: &str) -> bool {
    // TODO: make less forgiving of invalid dates
    Regex::new(r"(?im)^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$")
        .unwrap()
        .is_match(s)
}

fn is_valid_release_year(i: &i32) -> bool {
    // 1900 is somewhat arbitrary, but seems to encompass Spotify albums with incorrect dates
    (1900..=Utc::now().year()).contains(i)
}

#[cfg(test)]
mod tests {
    use chrono::Datelike;
    use chrono::Utc;

    use super::*;

    #[test]
    fn album_new() {
        let album = Album::new(
            AlbumName("foo".to_string()),
            AlbumArtists("bar".to_string()),
            TrackCount(20),
            ReleaseYear(2018),
            DateAdded("baz".to_string()),
            Playlist("bat".to_string()),
        )
        .expect("album data should be valid");

        assert_eq!(album.name.0, "foo");
        assert_eq!(album.artists.0, "bar");
        assert_eq!(album.date_added.0, "baz");
        assert_eq!(album.playlist.0, "bat");
        assert_eq!(album.release_year.0, 2018);
        assert_eq!(album.tracks.0, 20);
    }

    #[test]
    fn valid_album() {
        let album = Album::new(
            AlbumName("foo".to_string()),
            AlbumArtists("bar".to_string()),
            TrackCount(20),
            ReleaseYear(2018),
            DateAdded("2024-08-06 17:55:45".to_string()),
            Playlist("bat".to_string()),
        );

        assert!(album.is_ok());
    }

    #[test]
    fn invalid_album() {
        let assert_invalid = |actual: anyhow::Result<Album>, expected: &str| {
            assert!(actual.is_err_and(|e| e.to_string() == expected))
        };

        let expected_error = "Failed validation:\n\tInvalid name: \n\tInvalid date_added: baz";

        let album = Album::new(
            AlbumName("".to_string()),
            AlbumArtists("bar".to_string()),
            TrackCount(20),
            ReleaseYear(2018),
            DateAdded("baz".to_string()),
            Playlist("bat".to_string()),
        );

        assert_invalid(album, expected_error);

        let expected_error = "Failed validation:\n\tInvalid release_year: 2030";

        let album = Album::new(
            AlbumName("foo bar quux".to_string()),
            AlbumArtists("bar".to_string()),
            TrackCount(20),
            ReleaseYear(2030),
            DateAdded("2024-08-06 17:55:45".to_string()),
            Playlist("bat baz #120".to_string()),
        );

        assert_invalid(album, expected_error);
    }

    #[test]
    fn album_to_tsv_entry() {
        let expected = AlbumTsv("bar\tfoo\t20\t2018\tbaz\tbat".to_string());

        let actual = Album::new(
            AlbumName("foo".to_string()),
            AlbumArtists("bar".to_string()),
            TrackCount(20),
            ReleaseYear(2018),
            DateAdded("baz".to_string()),
            Playlist("bat".to_string()),
        )
        .expect("should be valid")
        .to_tsv_entry();

        assert_eq!(actual, expected);
    }

    #[test]
    fn valid_release_years() {
        for year in 1900..=Utc::now().year() {
            assert!(is_valid_release_year(&year));
        }
    }

    #[test]
    fn invalid_release_years() {
        for year in 1500..1900 {
            assert!(!is_valid_release_year(&year));
        }
        for year in Utc::now().year() + 1..2200 {
            assert!(!is_valid_release_year(&year));
        }
    }

    #[test]
    fn valid_track_counts() {
        for tc in 1..=350 {
            assert!(is_valid_track_count(&tc));
        }
    }

    #[test]
    fn invalid_track_counts() {
        assert!(!is_valid_track_count(&0));

        for tc in 351..=500 {
            assert!(!is_valid_track_count(&tc));
        }
    }
}
