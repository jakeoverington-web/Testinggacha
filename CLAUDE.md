# Testinggacha — working rules for Claude

Vertical (portrait) idle auto-battle hero collector for mobile, built in Unity.
Design source of truth: `docs/design/decisions.md`. If code and that file disagree, the file wins — flag it, don't guess.

## Current phase

Phase 1 — Core loop greybox: (a) roster ✓ (60 heroes as data; change heroes via tools/roster passes) (b) battle rules ✓ (roster pass 10, 12 synergy packages, decisions rows 28-30) (c) campaign + idle loot ✓ greybox (600 generated stages, idle chest, map/pre-battle/battle/result screens, auto mode; gate passed: docs/design/campaign-gate-v1.md). Battle redesign ✓ (decisions rows 1, 39: live battles, tap-to-cast ultimates, pre-battle on the battlefield). Phase 2 hero progression ✓ (decisions rows 6, 7, 31, 40: Contract slots, 10 stars, fodder and Sigils, resets, gear, dev panel; economy gate passed). Next: Phase 3 gacha when the owner says so. Designed for phase 4: towers and Boss mode (docs/superpowers/specs/2026-10-03-towers-boss-mode-design.md).
Phases: 1 Core loop → 2 Hero progression → 3 Gacha and roster → 4 Modes → 5 Art pass.
Do not start work from a later phase unless asked.

## Working with the owner

- One question at a time; stop immediately when the owner says "Wait".
- Confirm before committing a design decision; the owner often changes their mind, so draft on a branch and merge to main only after approval.
- Before finalizing a roster pass, show every changed kit text in one table.
- No parallel or background agents without an explicit OK. Downloads need a yes with file, source and size stated. No recolour mock-ups.

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
- Core logic changes come with Edit Mode tests (NUnit) in `Assets/_Game/Tests/` (method in `docs/design/testing.md`). Battle rules: `docs/design/battle-rules.md`.
- Small commits with clear messages. Never commit secrets, store keys or final art.
- When a design decision changes, update `docs/design/decisions.md` in the same commit.

## Commands

- `pwsh -File tools/csharp/test.ps1 [filter]` - all tests (~3 s, no Unity); needs PowerShell 7 (Windows PowerShell 5.1 lacks Roslyn). `tools/csharp/test.sh` on Linux/macOS.
- `pwsh -File tools/csharp/test.ps1 -Main Gacha.Tests.Tools.<Tool> [args]` - balance tools: NicheReport, SynergyReport, PackageProbe, InteractionAudit, HeroProbe, Variant, Ablation; campaign tools: StageGen (writes Data/stages.json, ~9 min), CampaignGate (table in docs/design/testing.md).
- `cd tools/roster; python check.py <pass>; python build.py` (on Windows set `PYTHONUTF8=1`, or the em dash in heroes.json is written in the wrong encoding) - after editing kits.py or adding a PASSES entry in revisions.py (every edit records why).
- `unity command run_tests --mode editor --timeout 300 --result-only` - same tests inside the open Unity Editor (owner's PC only; Unity CLI ships with Hub, Pipeline package installed). Args are `--name value`, not `name=value`.
- `python tools/roster/package_audit.py` - every hero's package claims must match her kit (decision row 30); expect 0 findings.

## Gotchas

- Golden replay hash changes on purpose with any rule or kit change: run tests, paste the printed "new hash" into InvariantTests.GoldenHash with a comment.
- Stage levels in stages.json are sim-tuned against the current kits: after any kit, rule or progression change, re-run StageGen and CampaignGate and commit the new stages.json with the change.
- Play mode via the Unity CLI: the Editor freezes at frame 1 when unfocused; eval `UnityEngine.Application.runInBackground = true` first (runtime only). Tap UI with a NavigationSubmitEvent via eval; `capture_game_view` returns the image inline (save_path lands under Assets/, so avoid it). After editing USS/UXML, eval `UnityEditor.AssetDatabase.Refresh()`.
- Passive texts must contain every number in their data (training-ground test), including condition windows (`stun+3`) and trigger cooldowns.
- A synergy package names one condition, on the enemy OR your team (row 30); a skill name must never equal a package name.
- New report versions replace old ones in docs/design (git rm the old file).
- heroes.json is the source of truth; the Hero Compendium (Claude Docs) is a synced copy.
- Unity 6.6 (row 13). Commit the .meta files Unity generates. Unity CLI (beta) only works for an agent on the same PC.

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
- IDs are lowercase snake_case and stable forever (`seravelle`, `ch01_s08`); display names live in data.
- Placeholder and art files are named by slot: `hero_<id>_fullbody`, `hero_<id>_card`, `hero_<id>_icon`, `banner_<id>_splash`.
- Art slot sizes: full-body 1:2, card 3:4, icon 1:1, banner splash 9:16. Do not change without updating the decisions log.
- Reference resolution: 1080 × 2340 portrait; keep UI inside safe areas.
- Art guideline: WLOP-inspired, elves only: 5 core races (Lumarin/High, Noctyr/Dark, Verdani/Nature, Thalyri/Ocean, Aethari/Arcane) with sub-races (decisions rows 3, 20, 22). Lore and tone: World Bible, decisions row 27 (central conflict: the Starfall War; dark tones, grief not gore). The UI follows the art (row 23). Every character is an adult.
