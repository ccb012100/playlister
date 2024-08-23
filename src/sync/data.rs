use std::{
    fmt,
    io::{Error, ErrorKind},
    str::FromStr,
};

use chrono::{Datelike, Utc};
use regex::Regex;

/* Structs */

#[derive(Clone, Debug)]
pub(crate) struct AlbumName(pub(crate) String);

#[derive(Clone, Debug)]
pub(crate) struct AlbumArtist(pub(crate) String);

#[derive(Clone, Debug)]
pub(crate) struct DateAdded(pub(crate) String);

#[derive(Clone, Debug)]
pub(crate) struct Playlist(pub(crate) String);

#[derive(Clone, Copy, Debug)]
pub(crate) struct ReleaseYear(pub(crate) i32);

#[derive(Clone, Copy, Debug)]
pub(crate) struct TrackCount(pub(crate) u16);

#[derive(Clone, Debug, PartialEq, Eq)]
pub(crate) struct AlbumTsv(pub(crate) String);

#[derive(Clone, Debug)]
pub(crate) struct Album {
    /// The name of the album
    name: AlbumName,
    /// The recording artist(s)
    artist: AlbumArtist,
    /// The number of tracks on the album
    tracks: TrackCount,
    /// The year the album was released
    release_year: ReleaseYear,
    /// The date the album was added to the starred playlist, in format `YYYY-MM-DD HH:MM:SS`
    date_added: DateAdded,
    /// The name of the starred playlist the album was added to
    playlist: Playlist,
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

impl fmt::Display for AlbumArtist {
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
            self.name, self.artist, self.tracks, self.release_year, self.date_added, self.playlist
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

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        match s.trim().parse::<u16>() {
            Ok(tc) => {
                if tc > 750 || tc == 0 {
                    return Err(Error::new(
                        ErrorKind::InvalidData,
                        "Track Count must be between 1 and 750",
                    ));
                }

                Ok(TrackCount(tc))
            }
            Err(err) => Err(Error::new(ErrorKind::InvalidData, err)),
        }
    }
}

impl FromStr for AlbumName {
    type Err = Error;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        if s.is_empty() {
            return Err(Error::new(
                ErrorKind::InvalidData,
                "AlbumName cannot be empty",
            ));
        }

        let trimmed = s.trim().to_string();

        if trimmed.is_empty() {
            return Err(Error::new(
                ErrorKind::InvalidData,
                "AlbumName cannot be white space only",
            ));
        }

        Ok(AlbumName(trimmed))
    }
}

impl FromStr for AlbumArtist {
    type Err = Error;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        if s.is_empty() {
            return Err(Error::new(
                ErrorKind::InvalidData,
                "AlbumArtist cannot be empty",
            ));
        }

        let trimmed = s.trim().to_string();

        if trimmed.is_empty() {
            return Err(Error::new(
                ErrorKind::InvalidData,
                "AlbumArtist cannot be white space only",
            ));
        }

        Ok(AlbumArtist(trimmed))
    }
}

impl FromStr for Playlist {
    type Err = Error;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        if s.is_empty() {
            return Err(Error::new(
                ErrorKind::InvalidData,
                "Playlist cannot be empty",
            ));
        }

        let trimmed = s.trim().to_string();

        if trimmed.is_empty() {
            return Err(Error::new(
                ErrorKind::InvalidData,
                "Playlist cannot be white space only",
            ));
        }

        Ok(Playlist(trimmed))
    }
}

impl FromStr for ReleaseYear {
    type Err = Error;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        match s.trim().parse::<i32>() {
            Ok(year) => {
                // 1900 is arbitrary, but seems to encompass Spotify albums with incorrect dates
                let min_year: i32 = 1900;
                let current_year: i32 = Utc::now().year();

                if year < min_year || year > current_year {
                    return Err(Error::new(
                        ErrorKind::InvalidData,
                        format!(
                            "Invalid value: {}. Release Year must be between {} and {}",
                            year, min_year, current_year
                        ),
                    ));
                }

                Ok(ReleaseYear(year))
            }
            Err(err) => Err(Error::new(ErrorKind::InvalidData, err)),
        }
    }
}

impl FromStr for AlbumTsv {
    type Err = Error;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        if s.is_empty() {
            return Err(Error::new(
                ErrorKind::InvalidData,
                "AlbumTsv cannot be empty",
            ));
        }

        let trimmed = s.trim().to_string();

        if trimmed.is_empty() {
            return Err(Error::new(
                ErrorKind::InvalidData,
                "AlbumTsv cannot be white space only",
            ));
        }

        let parts: Vec<&str> = s.split('\t').collect();

        if parts.len() != 6 {
            return Err(Error::new(
                ErrorKind::InvalidData,
                "AlbumTsv must be a string with 6 tab-separated values",
            ));
        }

        for part in &parts {
            if part.trim().is_empty() {
                return Err(Error::new(
                    ErrorKind::InvalidData,
                    format!("AlbumTsv cannot have any empty fields: {}", s),
                ));
            }
        }

        if parts[2].parse::<u16>().is_err() {
            return Err(Error::new(
                ErrorKind::InvalidData,
                format!("Invalid TrackCount value: {}", parts[2]),
            ));
        } else if parts[3].parse::<i32>().is_err() {
            return Err(Error::new(
                ErrorKind::InvalidData,
                format!("Invalid ReleaseYear value: {}", parts[3]),
            ));
        }

        Ok(AlbumTsv(s.to_string()))
    }
}
impl FromStr for DateAdded {
    type Err = Error;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        if s.is_empty() {
            return Err(Error::new(
                ErrorKind::InvalidData,
                "DateAdded cannot be empty",
            ));
        }

        let trimmed = s.trim().to_string();

        if trimmed.is_empty() {
            return Err(Error::new(
                ErrorKind::InvalidData,
                "DateAdded cannot be white space only",
            ));
        }

        match Regex::new(r"(?im)^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$")
            .unwrap()
            .is_match(&trimmed)
        {
            true => Ok(DateAdded(trimmed)),
            false => Err(Error::new(
                ErrorKind::InvalidData,
                format!(
                    "DateAdded is invalid: '{}'. Must be in format YYYY-MM-DD HH:MM:SS",
                    trimmed
                ),
            )),
        }
    }
}

/* Associated methods */

impl Album {
    pub fn new(
        name: AlbumName,
        artist: AlbumArtist,
        tracks: TrackCount,
        release_year: ReleaseYear,
        date_added: DateAdded,
        playlist: Playlist,
    ) -> Self {
        Self {
            name,
            artist,
            tracks,
            release_year,
            date_added,
            playlist,
        }
    }

    pub(crate) fn to_tsv_entry(&self) -> AlbumTsv {
        AlbumTsv(format!(
            "{}\t{}\t{}\t{}\t{}\t{}",
            self.artist, self.name, self.tracks, self.release_year, self.date_added, self.playlist
        ))
    }
}

impl AlbumTsv {
    pub(crate) fn _to_album(&self) -> Album {
        let parts: Vec<&str> = self.0.split('\t').collect();
        assert_eq!(parts.len(), 6);

        Album {
            name: AlbumName::from_str(parts[1]).unwrap(),
            artist: AlbumArtist::from_str(parts[0]).unwrap(),
            tracks: TrackCount::from_str(parts[2]).unwrap(),
            release_year: ReleaseYear::from_str(parts[3]).unwrap(),
            date_added: DateAdded::from_str(parts[4]).unwrap(),
            playlist: Playlist::from_str(parts[5]).unwrap(),
        }
    }
}

#[cfg(test)]
mod tests {
    use crate::sync::data::{
        Album, AlbumArtist, AlbumName, AlbumTsv, DateAdded, Playlist, ReleaseYear, TrackCount,
    };

    #[test]
    fn album_new() {
        let name = AlbumName("foo".to_string());
        let artist = AlbumArtist("bar".to_string());
        let date_added = DateAdded("baz".to_string());
        let playlist = Playlist("bat".to_string());
        let release_year = ReleaseYear(2018);
        let tracks = TrackCount(20);
        let album = Album::new(name, artist, tracks, release_year, date_added, playlist);

        assert_eq!(album.name.0, "foo");
        assert_eq!(album.artist.0, "bar");
        assert_eq!(album.date_added.0, "baz");
        assert_eq!(album.playlist.0, "bat");
        assert_eq!(album.release_year.0, 2018);
        assert_eq!(album.tracks.0, 20);
    }

    #[test]
    fn album_to_tsv_entry() {
        let actual = Album::new(
            AlbumName("foo".to_string()),
            AlbumArtist("bar".to_string()),
            TrackCount(20),
            ReleaseYear(2018),
            DateAdded("baz".to_string()),
            Playlist("bat".to_string()),
        )
        .to_tsv_entry();

        let expected = AlbumTsv("bar\tfoo\t20\t2018\tbaz\tbat".to_string());

        assert_eq!(actual, expected);
    }
}
