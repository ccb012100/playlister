#!/usr/bin/env bash
set -eou pipefail

repo=$(dirname -- "$(readlink -f -- "$0")")

ln -sv "$repo"/../dbsearch.py "$HOME"/bin/playlister.py
ln -sv "$repo"/../playlister.sh "$HOME"/bin/playlister.sh
