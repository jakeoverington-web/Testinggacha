# Testinggacha — working rules for Claude

Vertical (portrait) idle auto-battle hero collector for mobile, built in Unity.
Design source of truth: `docs/design/decisions.md`. If code and that file disagree, the file wins — flag it, don't guess.

## Current phase

Phase 1 — Core loop greybox, in this order: (a) roster design: 60 heroes as data (Hero Compendium; heroes.json, combat.json; change heroes via tools/roster passes); (b) battle rules built for exactly those kits (real-time, everyone acts at once, race counters + team bonuses); (c) campaign stages and idle loot.
Phases: 1 Core loop → 2 Hero progression → 3 Gacha and roster → 4 Modes → 5 Art pass.
Do not start work from a later phase unless asked.

## Hard rules

1. **Art comes last.** Use placeholders only (flat colour + label). Never block work on art.
2. **Content is data.** Heroes, stages, banners, gear and tuning numbers live in JSON under `Assets/_Game/Data/`. Adding content = editing data, not code.
3. **Logic is engine-free.** Game rules (battle sim, gacha, progression, economy) go in `Assets/_Game/Scripts/Core/` with no `UnityEngine` references, so they are unit-testable and deterministic.
4. **Seeded randomness only.** All random rolls go through the shared seeded RNG in Core, so battles and pulls can be replayed in tests.
5. **No hand-editing `.unity` / `.prefab` / `.asset` YAML** beyond trivial fixes. Build UI with UI Toolkit (UXML + USS text files) and wire it in C#.
6. **Stay in scope.** Deferred features (bonds, dorm, events, free formation, daily dungeons) get hooks only, never implementations, unless the decisions log changes.
7. **Build hooks, not features, for later.** Formation positions are grid coordinates; the guild boss is a reusable "special encounter"; hero data reserves room for bond fields.
8. **No unprompted pop-ups.** Never show purchase offers, sale banners, login reward windows, scrolling announcement tickers or announcements the player did not tap for. Player-opened overlays (skill info, tooltips, confirmations) are fine.

## Workflow

- One feature or ticket per chat. Start from the Linear ticket; end with a commit referencing it.
- Plan before code for anything over ~50 lines; ask before changing a design decision.
- Core logic changes come with Edit Mode tests (NUnit) in `Assets/_Game/Tests/`.
- Small commits with clear messages. Never commit secrets, store keys or final art.
- When a design decision changes, update `docs/design/decisions.md` in the same commit.

## Saving tokens and time

- Read only the files the task needs; use the folder map below instead of scanning the repo.
- Prefer editing data files over code. Prefer small edits over rewrites.
- Keep docs short: decisions as table rows, not essays.

## Folder map

```
Assets/_Game/
  Scripts/Core/     engine-free rules: Battle, Gacha, Progression, Economy, Rng
  Scripts/Game/     Unity glue: scenes, services, save/load
  Scripts/UI/       UI Toolkit controllers, one per screen
  UI/               UXML layouts + USS styles, one folder per screen
  Data/             JSON content: heroes, stages, banners, gear, tuning
  Placeholders/     placeholder sprites, named by final slot
  Tests/            Edit Mode tests for Core
docs/design/        decisions log and design notes
```

## Conventions

- C# namespaces: `Gacha.Core.*`, `Gacha.Game.*`, `Gacha.UI.*`.
- IDs are lowercase snake_case and stable forever (`seraphine`, `ch01_s08`); display names live in data.
- Placeholder and art files are named by slot: `hero_<id>_fullbody`, `hero_<id>_card`, `hero_<id>_icon`, `banner_<id>_splash`.
- Art slot sizes: full-body 1:2, card 3:4, icon 1:1, banner splash 9:16. Do not change without updating the decisions log.
- Reference resolution: 1080 × 2340 portrait; keep UI inside safe areas.
- Art guideline: WLOP-inspired, elves only: 5 core races (High, Dark, Arcane, Ocean, Nature; names provisional) with sub-races (decisions rows 3, 20, 22). The UI follows the art (row 23). Every character is an adult.
