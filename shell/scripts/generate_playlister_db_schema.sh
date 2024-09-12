#!/usr/bin/env sh
set -eu
# FIXME: this only seems to work if run from the directory "playlister/shell/"; running from "playlister/shell/scripts" just prints to stdout
scriptdir=$(dirname -- "$(readlink -f -- "$0")")
sqlite3 --readonly "$HOME/playlister.sqlite3" ".read $scriptdir/../sql/generate_playlister_db_schema.sqlite"
