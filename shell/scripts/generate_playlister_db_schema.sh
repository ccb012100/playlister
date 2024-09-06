#!/usr/bin/env sh
set -eu

scriptdir=$(dirname -- "$(readlink -f -- "$0")")
sqlite3 --readonly "$HOME/playlister.sqlite3" ".read $scriptdir/../sql/generate_playlister_db_schema.sqlite"
