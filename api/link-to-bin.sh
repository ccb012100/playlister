#!/usr/bin/env bash
set -Eeou pipefail
repo=$(dirname -- "$(readlink -f -- "$0")")

ln -sv "$repo" "$HOME"/bin/playlister_api
