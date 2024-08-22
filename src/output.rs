use nu_ansi_term::{AnsiString, AnsiStrings};

pub mod search;

pub(crate) struct Output();

impl Output {
    /// print to stderr
    pub(crate) fn print_stderr(message: &[AnsiString]) {
        eprintln!("{}", AnsiStrings(message));
    }
}
