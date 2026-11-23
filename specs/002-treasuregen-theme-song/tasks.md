---

description: "Task list for the DnDGen.TreasureGen theme song"
---

# Tasks: DnDGen.TreasureGen Theme Song

**Input**: [plan.md](plan.md), [spec.md](spec.md)

- **(maintainer)**: Only the maintainer can do this task. `/speckit-implement` stops and hands it
  over.

## Phase 1: Setup

- [X] T001 Copy every Markdown and PNG file under `docs/` to the integration test output
- [X] T002 [P] Add `docs/theme-song/Cinzel.ttf` and `Cinzel-OFL.txt`, unchanged from DnDGen/.github

## Phase 2: Tests

- [ ] T003 (maintainer) Approve the test list, RollGen's with TreasureGen's names:
  - Record: `HasCorrectSectionsInOrder`; `SunoField_FitsSunoLimit` for Title, Style, and Lyrics;
    `Sources_HasCorrectTable`; `Sources_EveryQuotedPhraseIsStillInTheReadme`;
    `AlbumArt_LinksToTheSong`; `AlbumArt_ImageExists`.
  - README: `ThemeSong_HasBadgeBesideTheBuildBadge`; `ThemeSong_HasSectionAtTheEnd`;
    `ThemeSong_LinksToTheRecord`; `ThemeSong_LinksToTheSameSongAsTheRecord`;
    `DocumentLinks_AreAbsolute`.
- [X] T004 Write the record's tests in `Docs/ThemeSong/ReadmeTests.cs`
- [X] T005 Write the README's theme song tests in `ReadmeTests.cs`. (The song was drafted in the
  same sitting, so they were not seen failing before the record existed; the three that remain
  failing are the Suno link tests, as intended.)

## Phase 3: The Song

- [X] T006 Draft the Title, Style, and Lyrics from the README, and their Sources
- [ ] T007 (maintainer) Review the draft lyrics and style, and confirm the genre and English
- [X] T008 Draw the album art, recoloured from the main theme's cover, and render it
- [ ] T009 (maintainer) Approve the album art
- [ ] T010 (maintainer) Generate drafts in Suno, each Sampling the main theme's "Rogante
  Marendor" snippet (3:00.0 to 3:17.1) with Variety all the way down; pick and publish the take
- [ ] T011 (maintainer) Add the Suno link to the record (`TODO_SUNO_LINK`, twice) and the README
  (the badge, and the Theme Song section's link to the song), and record what was learned in
  Suno Notes

## Phase 4: The README

- [X] T012 Add the Theme Song section to the end of the README, linking to the record
- [ ] T013 (maintainer) Open the DnDGen/.github pull request from `dndgen-github.patch`, with
  `SUNO_LINK` replaced by the Suno link

## Phase 5: Polish

- [ ] T014 Once T011 is done, run every test project and confirm coverage of
  `DnDGen.TreasureGen.dll` didn't drop
