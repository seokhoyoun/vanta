# Vanta.Comm.TestHost.WinForms

WinForms-based manual test host for the current communication porting work.

## Purpose

- start a simulated MELSEC driver
- read and write sample tags from a UI
- inspect raw block memory without a real PLC

## Current Scope

- simulation-only startup
- sample scalar and array tag verification
- tag list grid and tag add/edit/delete management
- direct tag read/write
- block memory read

## Next Extension

- real adapter-backed configuration loading
- real TCP MELSEC connection mode
- composite tag and composite array visualization
