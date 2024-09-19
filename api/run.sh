#!/usr/bin/env bash
set -Eeou pipefail

scriptdir=$(dirname -- "$(readlink -f -- "$0")")

echo '> dotnet run --project' "$scriptdir/Playlister/Playlister.csproj"
dotnet run --project "$scriptdir/Playlister/Playlister.csproj"
