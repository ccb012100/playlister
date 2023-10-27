use nu_ansi_term::{AnsiString, Color, AnsiStrings};

pub(crate) fn success_output(message: &str) {
    let message: &[AnsiString] = &[Color::Green.bold().paint(message)];

    print_message(message);
}

#[allow(dead_code)]
pub(crate) fn info_output(message: &str) {
    let message: &[AnsiString] = &[Color::Blue.paint(message)];

    print_message(message);
}

#[allow(dead_code)]
pub(crate) fn warn_output(message: &str) {
    let message: &[AnsiString] = &[Color::Yellow.paint(message)];

    print_message(message);
}

pub(crate) fn error_output(message: &str) {
    let message = format!("Error: {}", message);
    let message: &[AnsiString] = &[Color::Red.bold().paint(message)];

    print_message(message);
}

fn print_message(message: &[AnsiString]) {
    println!("{}", AnsiStrings(message));
}
