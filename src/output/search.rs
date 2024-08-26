use crate::{output::Output, search::data::SearchResults};
use comfy_table::*;
use log::debug;
use nu_ansi_term::{AnsiString, Color};
use std::io::{self, stdout, IsTerminal, Write};

pub struct SearchOutput();

impl SearchOutput {
    // Print search results to stdout
    pub fn search_results_table(search_results: &SearchResults) {
        if search_results.results.is_empty() {
            Self::no_results(search_results);
            return;
        }

        // print without formatting if not a tty
        if !stdout().is_terminal() {
            Self::search_summary(search_results)
        }

        let mut table = Table::new();

        table
            .load_preset(presets::UTF8_HORIZONTAL_ONLY)
            .set_content_arrangement(ContentArrangement::DynamicFullWidth);

        if search_results.include_header {
            table.set_header(Row::from(Self::get_header_fields(
                search_results.include_playlist_name,
            )));
        }

        for album in &search_results.results {
            let tracks = album.tracks.to_string();

            let display_fields = match search_results.include_playlist_name {
                true => vec![
                    &album.artists,
                    &album.album,
                    &tracks,
                    &album.year_released,
                    &album.date_added,
                    &album.playlist,
                ],
                false => vec![
                    &album.artists,
                    &album.album,
                    &tracks,
                    &album.year_released,
                    &album.date_added,
                ],
            };

            table.add_row(display_fields);
        }

        // Right-align Tracks field
        let column = table.column_mut(2).expect("Table has at least 4 columns");
        column.set_cell_alignment(CellAlignment::Right);

        println!("{table}");
        Self::search_summary(search_results);
    }

    /// print search results to stdout with no formatting
    pub fn search_results(search_results: &SearchResults) {
        debug!("🪵 search_results called with {:#?}", search_results);
        if search_results.results.is_empty() {
            Self::no_results(search_results);
            return;
        }

        Self::search_summary(search_results);

        let mut lock = io::stdout().lock();

        if search_results.include_header {
            writeln!(
                lock,
                "{}",
                Self::get_header_fields(search_results.include_playlist_name).join("\t")
            )
            .expect("writeln shouldn't fail");
        }

        search_results.results.iter().for_each(|result| {
            writeln!(
                lock,
                "{}",
                result.to_tsv(search_results.include_playlist_name)
            )
            .unwrap();
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
        match include_playlist {
            true => {
                vec![
                    "Artists",
                    "Album",
                    "Tracks",
                    "Year",
                    "Date Added",
                    "Playlist",
                ]
            }
            false => vec!["Artists", "Album", "Tracks", "Year", "Date Added"],
        }
    }
}
