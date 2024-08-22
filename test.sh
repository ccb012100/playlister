#!/usr/bin/env bash
set -Eeou pipefail

if [[ $# -gt 0 ]]; then
    term="$*"
else
    term="'Nuff"
fi

cargo build || exit 1

echo sorted_albums "$term"
target/debug/playlist-util tsv ~/bin/albums/sorted_albums.tsv search "$term" || exit 1

echo all_albums "$term"
target/debug/playlist-util tsv ~/bin/albums/all_albums.tsv search --include-playlist-name "$term"
