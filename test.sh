#!/usr/bin/env bash
set -Eeou pipefail

if [[ $# -gt 0 ]]; then
    term="$*"
else
    term="'Nuff"
fi

cargo build || exit 1

echo sorted_albums "$term"
time target/debug/playlist-util search tsv ~/bin/albums/sorted_albums.tsv "$term" || exit

echo all_albums "$term"
time target/debug/playlist-util search tsv ~/bin/albums/starred_albums.tsv --include-playlist-name "$term"

# time target/debug/playlist-util sync --source ~/playlister.db --destination ~/bin/albums/starred_albums.tsv
