#!/usr/bin/env bash
# Fast local test loop (no Unity needed). Usage: tools/csharp/test.sh [name-filter]
# Needs PowerShell 7 (pwsh): set PWSH=/path/to/pwsh if it is not on PATH.
PWSH="${PWSH:-$(command -v pwsh || echo /opt/pwsh/pwsh)}"
exec "$PWSH" -NoProfile -NoLogo -File "$(dirname "$0")/test.ps1" "$@"
