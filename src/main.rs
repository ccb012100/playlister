use anyhow::Error;
use clap::Parser;
use cli::Cli;
use log::debug;

mod cli;
mod output;
mod search;
mod sync;

fn main() -> Result<(), Error> {
    let cli = Cli::parse();

    cli.initialize_logger();

    debug!("🪵 parsed Cli: {:#?}", &cli);

    cli.run_subcommand()
}
