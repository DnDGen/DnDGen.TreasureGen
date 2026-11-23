---

description: "Task list for README public-API tests"
---

# Tasks: README Documents the Public API

**Input**: [plan.md](plan.md), [spec.md](spec.md)

**Tests**: The tests are the feature. They are written first and seen to fail for the expected
reason before the README changes that make them pass (II).

- **(maintainer)**: Only the maintainer can do this task.

## Phase 1: Setup

- [X] T001 Introduce Spec Kit 1.2.2 (`specify init --here --integration claude --script ps`), the
  DnDGen constitution template override, and TreasureGen's constitution 1.0.0
- [X] T002 Copy `README.md` to the output of `DnDGen.TreasureGen.Tests.Integration`

## Phase 2: User Story 1 - A new or changed member can't go undocumented (P1) 🎯 MVP

- [ ] T003 (maintainer) Approve the test list:
  1. `Constructors_AreAllDocumented`: if I add, remove, or change a public constructor, I expect
     the README's Constructor tables to list exactly the constructors there are, by type.
  2. `Methods_AreAllDocumented`: the same for every public method, with parameter types, names,
     and defaults, against the Method tables.
  3. `Properties_AreAllDocumented`: the same for public properties and fields (not the fields of
     constants classes), against the Property tables.
  4. `ConstantsClasses_AreAllDocumented`: if I add or remove a `*Constants` class, I expect the
     README's Constants table to list exactly those there are.
- [X] T004 [US1] Write the four tests in `ReadmeTests.cs` and see them fail against today's README
- [X] T005 [US1] Rewrite `README.md` with a Reference section until they pass

## Phase 3: User Story 2 - The README's code samples work (P2)

- [X] T006 [US2] Write a test for each README sample
- [X] T007 [US2] Correct the samples until the tests pass

## Phase 4: Polish

- [ ] T008 Run every test project; confirm coverage of `DnDGen.TreasureGen.dll` didn't drop (no package code changed; not measured yet)
- [X] T009 Remove one documented row and change one default, confirm each fails only the expected
  test naming the member; then revert
