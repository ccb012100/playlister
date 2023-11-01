use crate::search::SearchResults;
use comfy_table::presets::ASCII_BORDERS_ONLY_CONDENSED;
use comfy_table::*;
use nu_ansi_term::{AnsiString, AnsiStrings, Color};

pub(crate) struct Output();

impl Output {
    pub(crate) fn success(message: &str) {
        let message: &[AnsiString] = &[Color::Green.bold().paint(message)];

        Self::print_message(message);
    }

    pub(crate) fn info(message: &str) {
        let message: &[AnsiString] = &[Color::Blue.paint(message)];

        Self::print_message(message);
    }

    #[allow(dead_code)]
    pub(crate) fn warn(message: &str) {
        let message: &[AnsiString] = &[Color::Yellow.paint(message)];

        Self::print_message(message);
    }

    pub(crate) fn error(message: &str) {
        let message = format!("Error: {}", message);
        let message: &[AnsiString] = &[Color::Red.bold().paint(message)];

        Self::print_message(message);
    }

    pub(crate) fn search_results_table(search_results: &SearchResults) {
        if search_results.results.is_empty() {
            Self::no_results(search_results);
            return;
        }

        let mut table = Table::new();
        table
            .load_preset(ASCII_BORDERS_ONLY_CONDENSED)
            .set_content_arrangement(ContentArrangement::DynamicFullWidth);

        for album in &search_results.results {
            let tracks = album.tracks.to_string();
            let mut display_fields = vec![
                &album.artists,
                &album.album,
                &tracks,
                &album.year_released,
                &album.date_added,
            ];

            if search_results.include_playlist_name {
                display_fields.push(&album.playlist)
            };

            table.add_row(display_fields);
        }

        // Right-align Tracks field
        let column = table.column_mut(2).expect("Table has at least 4 columns");
        column.set_cell_alignment(CellAlignment::Right);

        println!("{table}");
        Self::search_summary(search_results);
    }

    pub(crate) fn search_results(search_results: &SearchResults) {
        if search_results.results.is_empty() {
            Self::no_results(search_results);
            return;
        }

        Self::search_summary(search_results);

        match search_results.include_playlist_name {
            true => {
                for result in &search_results.results {
                    println!("{}", result.to_tsv(search_results.include_playlist_name));
                }
            }
            false => todo!(),
        }
    }

    fn print_message(message: &[AnsiString]) {
        println!("{}", AnsiStrings(message));
    }

    fn no_results(search_results: &SearchResults) {
        assert!(search_results.results.is_empty());

        let strings: &[AnsiString] = &[
            Color::Fixed(208).paint("\t--- No results found for "),
            Color::Fixed(205).bold().paint(&search_results.search_term),
            Color::Fixed(208).paint(" ---\n"),
        ];

        print!("{}", AnsiStrings(strings));
    }

    fn search_summary(search_results: &SearchResults) {
        assert!(!search_results.results.is_empty());

        let strings: &[AnsiString] = &[
            Color::Default.paint("\t--- "),
            Color::Fixed(12)
                .bold()
                .paint(search_results.results.len().to_string()),
            Color::Default.paint(" results found for "),
            Color::Fixed(205).bold().paint(&search_results.search_term),
            Color::Default.paint(", sort: "),
            Color::Fixed(99)
                .bold()
                .paint(format!("{:?}", search_results.sort)),
            Color::Default.paint(" ---\n"),
        ];
        print!("{}", AnsiStrings(strings));
    }
}
