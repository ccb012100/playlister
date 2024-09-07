#!/usr/bin/env bash
set -Eeou pipefail

if [[ $# -gt 0 ]]; then
    term="$*"
else
    term="'Nuff"
fi

cargo build || exit 1

# echo sorted_albums "$term"
# time target/debug/playlist-util search tsv ~/bin/albums/sorted_albums.tsv "$term" || exit

# echo search all_albums for "'$term'"
# time target/debug/playlist-util search tsv ~/bin/albums/starred_albums.tsv --include-playlist-name "$term"

# echo search all_albums for "'$term'", filter by artist
# time target/debug/playlist-util search tsv ~/bin/albums/starred_albums.tsv --include-playlist-name "$term" --filter artists

# time target/debug/playlist-util sync --source ~/playlister.sqlite3 --destination ~/bin/albums/starred_albums.tsv

if [[ $# -gt 0 ]]; then
    # shellcheck disable=SC2048
    # shellcheck disable=SC2086
    time target/debug/playlist-util last --source ~/playlister.sqlite3 $*
else
    time target/debug/playlist-util last --source ~/playlister.sqlite3
fi
