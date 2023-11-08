use crate::search::SearchResults;
use comfy_table::*;
use nu_ansi_term::{AnsiString, AnsiStrings, Color};

pub(crate) struct Output();

impl Output {
    pub(crate) fn success(message: &str) {
        let message: &[AnsiString] = &[Color::Green.bold().paint(message)];

        Self::print_to_stderr(message);
    }

    pub(crate) fn info(message: &str) {
        let message: &[AnsiString] = &[Color::Blue.paint(message)];

        Self::print_to_stderr(message);
    }

    #[allow(dead_code)]
    pub(crate) fn warn(message: &str) {
        let message: &[AnsiString] = &[Color::Yellow.paint(message)];

        Self::print_to_stderr(message);
    }

    pub(crate) fn search_results_table(search_results: &SearchResults) {
        if search_results.results.is_empty() {
            Self::no_results(search_results);
            return;
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

    pub(crate) fn search_results(search_results: &SearchResults) {
        if search_results.results.is_empty() {
            Self::no_results(search_results);
            return;
        }

        Self::search_summary(search_results);

        if search_results.include_header {
            let fields: Vec<&str> = Self::get_header_fields(search_results.include_playlist_name);

            println!("{}", fields.join("\t"));
        }

        search_results.results.iter().for_each(|result| {
            println!("{}", result.to_tsv(search_results.include_playlist_name));
        });
    }

    fn print_to_stderr(message: &[AnsiString]) {
        eprintln!("{}", AnsiStrings(message));
    }

    fn no_results(search_results: &SearchResults) {
        assert!(search_results.results.is_empty());

        let strings: &[AnsiString] = &[
            Color::Fixed(208).paint("\t--- No results found for "),
            Color::Fixed(205).bold().paint(search_results.search_term),
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
            Color::Fixed(205).bold().paint(search_results.search_term),
            Color::Default.paint(", sort: "),
            Color::Fixed(99)
                .bold()
                .paint(format!("{:?}", search_results.sort)),
            Color::Default.paint(" ---\n"),
        ];
        print!("{}", AnsiStrings(strings));
    }

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
}
