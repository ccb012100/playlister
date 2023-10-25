fn main() {
    #[cfg(target_os = "windows")]
    nu_ansi_term::enable_ansi_support();

    println!("Hello, world!");
}
