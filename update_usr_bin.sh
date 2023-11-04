#!/usr/bin/env bash
set -Eeou pipefail

scriptdir=$(dirname -- "$(readlink -f -- "$0")")
cargo build --release || exit 1

util="$HOME"/bin/playlist-util

if [[ -f "$util" ]]; then
    rm "$util"
fi

cp -uv "$scriptdir"/target/release/playlist-util "$util"

echo DONE
