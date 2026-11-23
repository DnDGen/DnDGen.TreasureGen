# Implementation Plan: DnDGen.TreasureGen Theme Song

**Branch**: `spec-kit-readme-theme-song` | **Date**: 2026-11-23 | **Spec**: [spec.md](spec.md)

## Summary

Record a pirate shanty in `docs/theme-song`, with its Title, Style, Lyrics, Sources, Suno Notes,
and album art, test the record and the README's links to it, and add the badge, the section, and
the DnDGen/.github patch. Everything follows DnDGen.RollGen's spec 002.

## Technical Context

**Language/Version**: C# on `net10.0`. **Testing**: `DnDGen.TreasureGen.Tests.Integration`, which
copies `README.md` and every Markdown and PNG file under `docs/` to its output (spec 001, T002).

## Constitution Check

| Label | Check | Result |
|---|---|---|
| I.1 | The record's tests, the cover, and the README section are RollGen's, with TreasureGen's names. | Pass |
| I.3 | No comments added. | Pass |
| II | The tests fail until the Suno link exists, which is the signal the song isn't done. | Pass |
| II.3.c | The tests read only the build output. | Pass |
| III.1.c | The README's Theme Song section is new; no stale text. | Pass |
| README as contract | The theme song is in `docs/theme-song`, as the constitution says. | Pass |
| Versioning | PATCH: README and docs only. | Pass |

## Decisions

- **D1: Language is English**, with the leitmotif as the only Latin.
- **D2: The shanty is call and response**, shantyman and crew, the pairing the genre is built on.
  Responses are in parentheses, as in the main theme.
- **D3: Lyrics sing the README's high-level ideas** (what it generates, levels, powers, types of
  item, magic, specific items, getting the generators, the .NET version), not its Reference tables.

## Project Structure

```text
docs/theme-song/                  # README.md (record), cover.html, cover.png, Cinzel.ttf, Cinzel-OFL.txt
specs/002-treasuregen-theme-song/ # spec, plan, tasks, dndgen-github.patch
DnDGen.TreasureGen.Tests.Integration/
├── Docs/ThemeSong/ReadmeTests.cs # the record's tests
└── ReadmeTests.cs                # the README's theme song tests
```

## Revision: title and ship (2026-11-23)

- **D4: The song is "Haul Away", and the ship is the *Argenteus Re*.** The leitmotif is the ship's
  name, sung back by the crew, since "Haul away" is the line sung most. The leitmotif still returns
  from the main theme, as the org README asks of a generator's song.
- **D5: Answers are ordinary lines, not parentheses**, as DnDGen.Infrastructure learned: voices are
  assigned in each section's tag and the Style, with an em dash before each answer, and no inline
  voice tags.
- **D6: Verses rhyme in couplets**, so some lines add a few words to the README's phrase; Sources
  rows quote only the README part.
