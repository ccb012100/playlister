#!/usr/bin/env bash
set -eou pipefail

repo=$(dirname -- "$(readlink -f -- "$0")")

ln -sv "$repo"/../dbsearch.py "$HOME"/bin/playlist-dbsearch.py
ln -sv "$repo"/../search.sh "$HOME"/bin/playlist-search.sh
