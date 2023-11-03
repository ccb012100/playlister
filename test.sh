#!/usr/bin/env bash
set -Eeou pipefail

if [[ $# -gt 0 ]]; then
    term="$*"
else
    term="hammond"
fi

cargo build || exit 1

echo sorted_albums "$term"
target/debug/playlist-util tsv ~/src/playlist-search/albums/sorted_albums.tsv search "$term" || exit 1

echo all_albums "$term"
target/debug/playlist-util tsv ~/src/playlist-search/albums/all_albums.tsv search --include-playlist-name "$term"
