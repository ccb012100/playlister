#!/usr/bin/env bash
set -Eeou pipefail

if [[ $# -gt 0 ]]; then
    term="$*"
fi

echo sm funk
cargo run -- tsv ~/src/playlist-search/albums/sorted_albums.tsv search "${term:-funk}"

echo sa hammond
cargo run -- tsv ~/src/playlist-search/albums/all_albums.tsv search "${term:-hammond}"
