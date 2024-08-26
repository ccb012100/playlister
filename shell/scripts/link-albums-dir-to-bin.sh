#!/usr/bin/env bash
set -Eeou pipefail
script=$(dirname -- "$(readlink -f -- "$0")")

ln -sv "$script"/../albums "$HOME"/bin/
