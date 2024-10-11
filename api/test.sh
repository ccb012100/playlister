#!/usr/bin/env bash
set -Eeou pipefail

scriptdir=$(dirname -- "$(readlink -f -- "$0")")

echo '> dotnet test' "$scriptdir"
dotnet test "$scriptdir"
