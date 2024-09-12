use playlist_util::data::{search::SearchResults, Album};

use crate::output::Output;

use comfy_table::*;

use log::trace;
use nu_ansi_term::{AnsiString, Color};
use std::io::{self, stdout, IsTerminal, Write};

pub struct SearchOutput();

impl SearchOutput {
    /// Print search results to stdout in table format
    pub fn print_search_results_table(search_results: &SearchResults) {
        if search_results.results.is_empty() {
            Self::no_results(search_results);
        } else if stdout().is_terminal() {
            Self::print_albums_table(
                &search_results.results,
                search_results.include_header,
                search_results.include_playlist_name,
            );

            Self::search_summary(search_results);
        } else {
            // print with no formatting if output is not a terminal
            Self::print_search_results(search_results);
        }
    }

    /// Print last n albums added to stdout in table format
    pub fn print_last_n_albums_table(albums: &Vec<Album>, n: usize) {
        debug_assert_eq!(albums.len(), n);

        Self::print_albums_table(albums, true, true);

        Output::print_stderr(&[
            Color::Default.paint("\t--- last "),
            Color::Fixed(12).bold().paint(n.to_string()),
            Color::Default.paint(" albums added ---\n"),
        ]);
    }

    /// Print last n albums added to stdout with no formatting
    pub fn print_last_n_albums(albums: &[Album], n: usize) {
        debug_assert_eq!(albums.len(), n);

        Self::print_albums(albums, true, true);

        Output::print_stderr(&[
            Color::Default.paint("\t--- last "),
            Color::Fixed(12).bold().paint(n.to_string()),
            Color::Default.paint(" albums added ---\n"),
        ]);
    }

    /// print search results to stdout with no formatting
    pub fn print_search_results(search_results: &SearchResults) {
        trace!("🪵 search_results called with {:#?}", search_results);

        if search_results.results.is_empty() {
            Self::no_results(search_results);
        } else {
            Self::print_albums(
                &search_results.results,
                search_results.include_header,
                search_results.include_playlist_name,
            );
            Self::search_summary(search_results);
        }
    }

    fn print_albums(albums: &[Album], header: bool, playlist_name: bool) {
        let mut lock = io::stdout().lock();

        if header {
            writeln!(
                lock,
                "{}",
                Self::get_header_fields(playlist_name).join("\t")
            )
            .expect("writeln shouldn't fail");
        }

        albums.iter().for_each(|result| {
            writeln!(lock, "{}", result.to_tsv_search_result(playlist_name))
                .expect("writeln shouldn't fail");
        });
    }

    /// Print message for empty search results to stderr
    fn no_results(search_results: &SearchResults) {
        assert!(search_results.results.is_empty());

        let strings: &[AnsiString] = &[
            Color::Fixed(208).paint("\t--- No results found for "),
            Color::Fixed(205).bold().paint(search_results.search_term),
            Color::Fixed(208).paint(" ---\n"),
        ];

        Output::print_stderr(strings)
    }

    /// Print summary of search to stderr
    fn search_summary(search_results: &SearchResults) {
        assert!(!search_results.results.is_empty());

        let strings: &[AnsiString] = &[
            Color::Default.paint("\t--- "),
            Color::Fixed(12)
                .bold()
                .paint(search_results.results.len().to_string()),
            Color::Default.paint(" results found for "),
            Color::Fixed(205).bold().paint(search_results.search_term),
            Color::Default.paint(", sort: "),
            Color::Fixed(99)
                .bold()
                .paint(format!("{:?}", search_results.sort)),
            Color::Default.paint(" ---\n"),
        ];

        Output::print_stderr(strings);
    }

    /// get the search results headers
    fn get_header_fields<'a>(include_playlist: bool) -> Vec<&'a str> {
        if include_playlist {
            vec![
                "Artists",
                "Album",
                "Tracks",
                "Year",
                "Date Added",
                "Playlist",
            ]
        } else {
            vec!["Artists", "Album", "Tracks", "Year", "Date Added"]
        }
    }

    fn print_albums_table(albums: &Vec<Album>, header: bool, playlist_name: bool) {
        let mut table = Table::new();

        table
            .load_preset(presets::UTF8_HORIZONTAL_ONLY)
            .set_content_arrangement(ContentArrangement::DynamicFullWidth);

        if header {
            table.set_header(Row::from(Self::get_header_fields(playlist_name)));
        }

        for album in albums {
            let tracks = album.tracks.to_string();
            let release_year = &album.release_year.0.to_string();

            let display_fields: Vec<&str> = if playlist_name {
                vec![
                    &album.artists.0,
                    &album.name.0,
                    &tracks,
                    &release_year,
                    &album.date_added.0,
                    &album.playlist.0,
                ]
            } else {
                vec![
                    &album.artists.0,
                    &album.name.0,
                    &tracks,
                    &release_year,
                    &album.date_added.0,
                ]
            };

            table.add_row(display_fields);
        }

        // Right-align Tracks field
        let column = table.column_mut(2).expect("Table has at least 4 columns");
        column.set_cell_alignment(CellAlignment::Right);

        println!("{table}");
    }
}
