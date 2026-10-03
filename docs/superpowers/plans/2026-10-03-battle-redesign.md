# Pre-battle and Battle Redesign Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Map stage panel, pre-battle on the battlefield with slot swapping and Quick Deploy, and live battles with Auto-Battle or tap-to-cast ultimates whose inputs replay exactly.

**Architecture:** Core gains manual ultimates, an input queue with a per-tick log, a live fight session (start/finish), slot-based setups and Quick Deploy, all engine-free and tested. UI screens step the live session in whole ticks and send taps as inputs.

**Tech Stack:** C# 9 Core + NUnit (tools/csharp/test.ps1), Unity 6.6 UI Toolkit, Unity CLI for play-mode checks.

**Spec:** `docs/superpowers/specs/2026-10-03-prebattle-battle-redesign-design.md`

## Global Constraints

- Hard rules 1-8 (CLAUDE.md): placeholders only, Core engine-free, seeded RNG only, no hand-edited scene YAML (re-run SceneBuilder), no unprompted pop-ups.
- Golden hash must not change: a battle with no inputs and auto ultimates behaves exactly as today.
- Slots: 5 per team; slot i uses battle.json default cell i (`[0,1],[0,3]` front; `[1,0],[1,2],[1,4]` back). A setup is 5 hero ids with "" for an empty slot; 1-5 heroes, no duplicates.
- Campaign auto-chaining always uses Auto-Battle (owner, 2026-10-03).
- Quick Deploy = top 5 by `TeamPower.Hero` at the stage's expected level/stars, ties by roster order; tanks fill front slots first.

## Review Focus

1. Tap on a hero who is stunned or charmed when energy is full → the cast waits until it is legal, then fires; it is never lost or doubled. Test in Task 1.
2. Toggle AUTO on mid-fight while a manual request is pending → the ult casts once. Test in Task 1.
3. Old saves whose last setup is a plain hero list → load as slots in order. Test in Task 2.
4. Skip during a manual fight → rest runs on auto; result equals the replay with the logged inputs. Test in Task 2.
5. Quick Deploy with fewer than 2 tanks → front slots filled by the next strongest heroes. Test in Task 3.

---

### Task 1: Manual ultimates and the input log (Core)

**Files:** Modify `Scripts/Core/Battle/Battle.cs`, `Unit.cs`; Create `Scripts/Core/Battle/BattleInput.cs`; Test `Tests/BattleInputTests.cs`.

**Produces:** `public bool[] ManualUltimates = new bool[2]` on `Battle`; `enum InputKind { CastUltimate, SetAuto }`; `struct BattleInput { int Tick; InputKind Kind; int Unit; int Team; bool On; }`; `void Battle.Queue(BattleInput input)` (Tick ignored on queue; set when applied); `List<BattleInput> Battle.Inputs` (applied, in order); `static BattleResult Battle.Replay(GameData g, TeamSetup a, TeamSetup b, ulong seed, bool[] manual, IList<BattleInput> inputs)`; `Unit.UltRequested`.

Rules: queued inputs apply at the start of the next `Step` before units act. `CastUltimate` applies only for a living hero on a manual team (sets `UltRequested`; logged); otherwise dropped, not logged. `SetAuto(team, on)` sets `ManualUltimates[team] = !on` (logged). In `Act`, the ult branch requires `!ManualUltimates[team] || u.UltRequested`; a successful cast clears `UltRequested`.

- [ ] **Step 1: Failing tests:** `Manual_NeverAutoCasts` (manual team, 30 s, no `Cast` "ult" events from team 0 while energy reaches 100); `Manual_QueuedCastFiresNextTick`; `Manual_RequestWaitsThroughStun` (Review 1); `Manual_IllegalRequestsDropped` (dead unit, enemy unit, auto team → not in `Inputs`); `SetAuto_MidFight_CastsOnce` (Review 2); `Replay_WithInputs_SameHash`; `NoInputs_GoldenHashUnchanged` (existing InvariantTests stays green).
- [ ] **Step 2:** Run `pwsh -File tools/csharp/test.ps1 BattleInput` → FAIL.
- [ ] **Step 3:** Implement.
- [ ] **Step 4:** Full suite green, golden hash unchanged.
- [ ] **Step 5:** Commit "Battle: optional manual ultimates with a replayable input log".

### Task 2: Live fight session and slot setups (Core)

**Files:** Modify `Scripts/Core/Campaign/CampaignState.cs`, `SaveGame.cs`; Test `Tests/CampaignTests.cs`.

**Produces:** `FightSession { Battle Battle; int StageIndex; ulong Seed; TeamSetup Player, Enemy; bool Manual; }`; `FightSession CampaignState.StartFight(GameData g, int stageIndex, IList<string> slots, bool manual)` (validates as `Fight` did; Attempts++; stores LastSetup slots; sets `ManualUltimates[0] = manual`); `FightResult CampaignState.FinishFight(GameData g, FightSession s, long now)` (runs the battle to the end on auto if not over — Skip — then applies first clear and chest settle exactly as `Fight` today); `Fight(g, stage, slots, now)` = Start(manual: false) + Finish. `FightResult` gains `IList<BattleInput> Inputs` and `bool Manual`. `PlayerSetup` takes slots and fills `Cells` from slot indices.

- [ ] **Step 1: Failing tests:** `StartFinish_EqualsFight` (same seed/state → same hash and rewards); `Slots_PlaceHeroesInTheirCells` (hero in slot 3 starts at default cell 3); `Slots_EmptyAllowed_DuplicatesRejected`; `Skip_RunsRestOnAuto_ReplayMatches` (Review 4); `Save_OldHeroList_LoadsAsSlots` (Review 3); `Save_SlotsRoundTrip`.
- [ ] **Step 2:** Run → FAIL. **Step 3:** Implement (update existing CampaignTests calls to the slot form). **Step 4:** Full suite green.
- [ ] **Step 5:** Commit "Campaign: live fight sessions, slot setups, Skip".

### Task 3: Quick Deploy (Core)

**Files:** Create `Scripts/Core/Campaign/QuickDeploy.cs`; Test `Tests/QuickDeployTests.cs`.

**Produces:** `static List<string> QuickDeploy.Pick(GameData g, IEnumerable<string> owned, int level, int stars)` → 5 slots.

- [ ] **Step 1: Failing tests:** `PicksTop5ByPower`; `TanksTakeFrontSlots`; `FewTanks_FrontFilledByStrongest` (Review 5); `TiesBreakByRosterOrder`; `FewerThan5Owned_LeavesEmptySlots`.
- [ ] **Steps 2-4:** FAIL → implement → green. **Step 5:** Commit "Quick Deploy: top 5 by power, tanks in front".

### Task 4: Map stage panel (UI)

**Files:** Modify `UI/CampaignMap/campaign_map.uxml/.uss`, `Scripts/UI/CampaignMapController.cs`.

Tapping an unlocked banner selects it and shows a bottom panel (stage number, enemy tiles with level and stars, Fight → pre-battle); tapping another swaps it; the chest panel stays below. Verify in play mode via the Unity CLI (capture, tap). Commit "Map: stage panel".

### Task 5: Pre-battle on the battlefield (UI)

**Files:** Rewrite `UI/PreBattle/pre_battle.uxml/.uss`, `Scripts/UI/PreBattleController.cs`.

Top: power yours vs enemy. Field: side view, your 5 slots (left) and enemies (right) as tiles at their cells with name and level; tap one of your tiles then another slot to swap (empty slots shown as outlines). Roster strip: cards with race filter tabs (All + 5 races); tap adds to the first empty slot or removes. Buttons: Back, Quick Deploy, Auto-Battle, Battle (both disabled with 0 heroes). Starts with LastSetup slots. Verify in play mode. Commit "Pre-battle: battlefield, slot swapping, Quick Deploy, two fight buttons".

### Task 6: Live battle HUD and tap-to-cast (UI)

**Files:** Modify `UI/Battle/battle.uxml/.uss`, `Scripts/UI/BattleViewController.cs`, `ScreenRouter.cs`, `ResultController.cs`.

`ScreenRouter.Fight(stage, slots, manual)` → `StartFight`, shows battle with the live session (no pre-run). HUD: stage, timer, speed 1×/2×/4×, AUTO toggle (queues `SetAuto`), Skip (`FinishFight` immediately). Units with HP and energy bars. Bottom row: 5 portraits with HP/energy; in manual mode a full-energy portrait grows into a tall card (placeholder for alternate art) and tapping it queues `CastUltimate`. On battle end → `FinishFight` → save → result. Auto-chaining calls `Fight(..., manual: false)`. Verify in play mode: manual fight with a tap-cast, AUTO toggle, Skip, auto-chain of 3 stages. Commit "Battle: live HUD with tap-to-cast ultimates, AUTO and Skip".

### Task 7: Decisions and hand-off

- [ ] Decisions row 1 → "Idle auto-battle with optional manual ultimates (tap a full-energy portrait; Auto-Battle casts them)"; new row 39 for the pre-battle redesign (battlefield, slot swapping, Quick Deploy, Auto-Battle / Battle, map stage panel). CLAUDE.md current phase line.
- [ ] Re-run SceneBuilder; both test suites green; screenshots for the owner. Commit "Battle redesign hand-off".
