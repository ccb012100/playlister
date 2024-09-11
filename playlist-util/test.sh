#!/usr/bin/env bash
set -Eeou pipefail

if [[ $# -gt 0 ]]; then
    term="$*"
else
    term="'Nuff"
fi

cargo build || exit 1

time target/debug/playlist-util search tsv ~/bin/albums/sorted_albums.tsv "$term" || exit

time target/debug/playlist-util search tsv ~/bin/albums/starred_albums.tsv --include-playlist-name "$term" || exit

time target/debug/playlist-util search tsv ~/bin/albums/starred_albums.tsv --include-playlist-name "$term" --filter artists || exit

time target/debug/playlist-util sync --source ~/playlister.sqlite3 --destination ~/bin/albums/starred_albums.tsv || exit

time target/debug/playlist-util last ~/playlister.sqlite3 7 || exit

time target/debug/playlist-util search sqlite ~/playlister.sqlite3 --include-playlist-name "$term" || exit
