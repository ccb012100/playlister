use std::process::exit;

use clap::Parser;
use cli::{db_is_valid, tsv_is_valid, Cli};
use output::{error_output, success_output};

mod cli;
pub mod output;

fn main() {
    let cli = Cli::parse();

    match &cli.db {
        true => {
            if !db_is_valid(&cli.file) {
                error_output(&format!(
                    "Filename {} is not in valid format \"*.<sql|sqlite|sqlite3|db>\"",
                    cli.file
                ));
                exit(0)
            }
        }
        false => {
            if !tsv_is_valid(&cli.file) {
                error_output(&format!(
                    "Filename {} is not in valid format \"*.tsv\"",
                    cli.file
                ));
                exit(0)
            }
        }
    }

    success_output("Finished running!")
}
