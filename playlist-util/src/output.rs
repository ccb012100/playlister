use nu_ansi_term::{AnsiString, AnsiStrings};

pub mod search;

pub struct Output();

impl Output {
    /// print to stderr
    pub fn print_stderr(message: &[AnsiString]) {
        eprintln!("{}", AnsiStrings(message));
    }
}
