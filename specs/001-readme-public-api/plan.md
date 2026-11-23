# Implementation Plan: README Documents the Public API

**Branch**: `dotnet-10-upgrade` | **Date**: 2026-11-23 | **Spec**: [spec.md](spec.md)

## Summary

Add tests to a new integration `ReadmeTests` that compare, by reflection, every public member of
every public type in `DnDGen.TreasureGen.dll` with the members listed in the README's Constructor,
Method, and Property tables under that type's heading, and the `*Constants` classes with its
Constants table. Then rewrite the README with a Reference section that makes them pass, and
turn each of its samples into a test. The helpers follow DnDGen.RollGen's `ReadmeTests`.

## Technical Context

**Language/Version**: C# on `net10.0`. **Dependencies**: NUnit 5 (tests only).
**Testing**: `DnDGen.TreasureGen.Tests.Integration`, which gains the README in its output.
**Constraints**: The README is packed into the package, so it reads well on NuGet.

## Constitution Check

| Label | Check | Result |
|---|---|---|
| I.1 | `GetLinesOutsideCodeBlocks`, `GetTableRows`, `WriteMember`, and the table kinds are RollGen's. | Pass |
| I.2 | Per-type headings and the Constants table answer TreasureGen's same-named members and 1,000 constants; they don't replace RollGen's pattern. | Pass |
| I.3 | No comments added. | Pass |
| II | Tests are written first and seen to fail against today's README (T004) before the README changes (T005). | Pass |
| II.1.c | Constructors, methods, properties, constants classes, and each sample have their own case. | Pass |
| II.2.a | The whole public API is checked, not a sample. | Pass |
| II.2.c (TreasureGen) | No package code changes. | Pass |
| II.3.a | Each test compares whole lists, so a failure shows every missing and extra item. | Pass |
| II.3.c | The tests read only the build output. | Pass |
| III.1.c | The README's wrong samples are corrected. | Pass |
| README as contract | This feature is the contract's test. | Pass |
| Versioning | PATCH: README, tests, Spec Kit. | Pass |

## Decisions

- **D1: Signatures.** As RollGen's D1: `Name<T>(type name = default, ...)` with C# keywords.
- **D2: Found types.** `typeof(Treasure).Assembly.GetExportedTypes()`, nested types included.
- **D3: Per-type headings.** Rows count only under the `###` heading equal to the type's name
  (`Outer.Inner` when nested), written `Type.member` in failures.
- **D4: Constants classes** are listed in a Constants table, by name. Their methods and properties
  are in the type's own Method and Property tables.
- **D5: Overrides of `object`'s members** (`ToString`, `Equals`, `GetHashCode`) are excluded.

## Project Structure

```text
README.md                                                      # rewritten, Reference added
DnDGen.TreasureGen.Tests.Integration/
├── DnDGen.TreasureGen.Tests.Integration.csproj                # copies README.md to the output
└── ReadmeTests.cs                                             # document and sample tests
```
