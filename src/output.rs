use comfy_table::presets::ASCII_BORDERS_ONLY_CONDENSED;
use comfy_table::*;
use nu_ansi_term::{AnsiString, AnsiStrings, Color};

use crate::search::SearchResults;

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

    pub(crate) fn search_results_table(search_results: SearchResults) {
        let summary = format!(
            "Found {} results for '{}', sort: {:?}",
            search_results.results.len(),
            search_results.search_term,
            search_results.sort
        );

        match search_results.include_playlist_name {
            true => {
                let mut table = Table::new();
                table
                    .load_preset(ASCII_BORDERS_ONLY_CONDENSED)
                    .set_content_arrangement(ContentArrangement::DynamicFullWidth);

                search_results.results.into_iter().for_each(|result| {
                    table.add_row(result.split('\t').collect::<Vec<&str>>());
                });

                // Right-align Tracks field
                let column = table.column_mut(2).expect("Table has at least 4 columns");
                column.set_cell_alignment(CellAlignment::Right);

                println!("{table}");
                Output::success(&summary);
            }
            false => todo!(),
        }
    }

    pub(crate) fn search_results(search_results: SearchResults) {
        Self::success(&format!(
            "Found {} results for '{}', sort: {:?}",
            search_results.results.len(),
            search_results.search_term,
            search_results.sort
        ));

        match search_results.include_playlist_name {
            true => {
                search_results.results.into_iter().for_each(|result| {
                    println!("{}", result);
                });
            }
            false => todo!(),
        }
    }

    fn print_message(message: &[AnsiString]) {
        println!("{}", AnsiStrings(message));
    }
}
