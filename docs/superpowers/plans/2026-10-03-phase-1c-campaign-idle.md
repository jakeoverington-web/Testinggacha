# Phase 1c: Campaign and Idle Loot Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** A playable greybox campaign: 600 sim-checked stages, an idle chest that fills from your highest stage, and placeholder map, pre-battle, battle and result screens with auto mode.

**Architecture:** All rules (stat scaling, idle maths, campaign state, save format) are engine-free C# in `Scripts/Core`, tested by `tools/csharp/test.ps1`. A sim tool in `Tests/Tools` generates and difficulty-checks `Data/stages.json`. Unity code in `Scripts/Game` and `Scripts/UI` only loads data, keeps time, saves, and draws UI Toolkit screens.

**Tech Stack:** Unity 6.6 (6000.6.4f1), C# 9, UI Toolkit (UXML + USS), NUnit via the local runner and Unity Test Runner, Unity CLI 1.0.0-beta.12 + Pipeline 0.8.0-exp.1 for in-editor checks.

**Spec:** Game Modes and Progression Plan (Claude Docs, https://claude.ai/artifact/17E9F6w9foTABtEW6mxMtL), sections Campaign and Idle rewards; `docs/design/decisions.md` rows 4, 15, 17, 18, 21, 31, 33, 34, 35.

## Global Constraints

- Hard rules 1-8 in CLAUDE.md apply: placeholders only (flat colour + label), content is data, Core has no `UnityEngine`, all randomness through `Gacha.Core.Rng`, no hand-edited `.unity`/`.prefab`/`.asset` YAML, no unprompted pop-ups.
- Namespaces `Gacha.Core.*`, `Gacha.Game.*`, `Gacha.UI.*`. IDs stable snake_case: stage ids `ch01_s01` … `ch20_s30`.
- Campaign: 20 chapters × 30 stages = 600; every 10th stage a gate, stage 30 the chapter boss; chapters 1-4 Gilded Spire (high), 5-8 Ashmourne (dark), 9-12 Elderwild (nature), 13-16 Drowned Reach (ocean), 17-20 Unlit Observatory (arcane).
- Stats grow 4% of their level-1 value per level: multiplier `1 + 0.04 × (level − 1)` (×8.96 at 200) on HP, ATK, DEF. Star multipliers 1★-10★: 1.00, 1.05, 1.10, 1.15, 1.20, 1.30, 1.40, 1.50, 1.60, 1.70.
- Expected level at stage n = `clamp(round(n / 3), 1, 200)`; expected stars = `1 + (chapter − 1) / 4` (integer division: 1★ in ch 1-4 … 5★ in ch 17-20). In phase 1 your team always fights at the expected level and stars of the stage it is fighting (owner, 2026-10-03).
- Idle rates per hour at highest cleared stage s ≥ 1: gold `7000 × 1.0075^(s−30)`, Hero XP `3000 × 1.0075^(s−30)`, Starlight `24 × 1.006^(s−30)`; s = 0 earns nothing. 24 h hard cap since last collect (row 33). Hourglass = 2 h at the current rate, outside the cap.
- First clear pays 1 h of that stage's idle gold and Hero XP (tunable in phase 2's economy sim).
- New unlocks show as badges on the map, never pop-ups (rows 15, 35). Auto mode plays, walks and plays the next stage until a loss (row 21).
- Out of scope (later phases or hooks only): levelling and star spending, gear and gear drops, Race Sigil and Hourglass drops from idle, presets (row 17), the 11 formations (default formation only), Fallen Stars amounts (phase 3), Android data loading.
- No Linear ticket yet: commits reference "phase 1c" until the owner gives one.

## Review Focus

1. Clock goes backwards (phone time changed) → the chest never loses or gains loot; negative elapsed counts as 0. Test in Task 3.
2. App closed for days → chest shows exactly 24 h of loot, not more. Test in Task 3.
3. Stage cleared while the chest is part-full → loot so far keeps the old rate, later loot the new one. Test in Task 3.
4. Corrupt or missing save file → game starts fresh instead of crashing; a newer-version save is refused, not misread. Test in Task 4.
5. Replaying an already-cleared stage → no second first-clear reward and no change to idle rate. Test in Task 4.

---

### Task 1: Level and star stat scaling

**Files:**
- Create: `Assets/_Game/Data/progression.json`, `Assets/_Game/Scripts/Core/Progression/StatScaling.cs`
- Modify: `Assets/_Game/Scripts/Core/Battle/Battle.cs:8-14` (TeamSetup), `:55-63` (AddTeam)
- Test: `Assets/_Game/Tests/ProgressionTests.cs`

**Interfaces:**
- Produces: `Gacha.Core.Progression.StatScaling` with `static StatScaling FromJson(string json)`, `double LevelMult(int level)`, `double StarMult(int stars)`, `double Mult(int level, int stars)`, `int MaxLevel`; `GameData.Progression` (loaded by `GameData.Load` when `progression.json` exists). `TeamSetup.Scales : List<double>` (null = 1 for every hero), multiplied with `StatScale`.
- `progression.json`: `{ "levelGrowth": 0.04, "maxLevel": 200, "starMult": [1.0, 1.05, 1.10, 1.15, 1.20, 1.30, 1.40, 1.50, 1.60, 1.70] }`

- [ ] **Step 1: Write failing tests** `LevelMult_GrowsFourPercentPerLevel` (LevelMult(1)=1.0, LevelMult(200)=8.96 ±1e-9), `StarMult_MatchesTable` (StarMult(1)=1.0, StarMult(5)=1.20, StarMult(10)=1.70), `StarAndLevel_OutOfRange_Clamp` (LevelMult(0)=LevelMult(1), LevelMult(250)=LevelMult(200), StarMult(11)=1.70), `TeamScales_MultiplyBaseStats` (Lab with `Scales = {2.0}` on one ally: its MaxHp = base HP × 2 × hpScale).
- [ ] **Step 2: Run** `pwsh -File tools/csharp/test.ps1 Progression` → FAIL (types missing).
- [ ] **Step 3: Implement** StatScaling, progression.json, `TeamSetup.Scales`, apply `Scales[i]` alongside `StatScale` in AddTeam.
- [ ] **Step 4: Run** `pwsh -File tools/csharp/test.ps1` → all pass, golden hash unchanged (Scales null keeps old behaviour).
- [ ] **Step 5: Commit** "Phase 1c: level and star stat scaling (rows 31, 34)".

### Task 2: JSON writer

**Files:**
- Modify: `Assets/_Game/Scripts/Core/Data/Json.cs`
- Test: `Assets/_Game/Tests/JsonTests.cs`

**Interfaces:**
- Produces: `static string Json.Write(object value, bool pretty = false)` for `Dictionary<string, object>`, `List<object>`, string, double, int, long, bool, null. Keys written in insertion order; doubles with invariant culture, shortest round-trip (`"R"`).

- [ ] **Step 1: Write failing tests** `Write_ThenParse_RoundTrips` (nested dict/list/string with quotes and `\n`/number 0.1/bool/null equal after Parse), `Write_UsesInvariantCulture` (1.5 → `"1.5"` under `de-DE`), `Write_EscapesControlChars`.
- [ ] **Step 2: Run** `pwsh -File tools/csharp/test.ps1 Json` → FAIL.
- [ ] **Step 3: Implement** `Json.Write`.
- [ ] **Step 4: Run** all tests → pass.
- [ ] **Step 5: Commit** "Phase 1c: Json.Write for saves and generated data".

### Task 3: Idle chest and wallet

**Files:**
- Create: `Assets/_Game/Data/economy.json`, `Assets/_Game/Scripts/Core/Economy/IdleRates.cs`, `IdleChest.cs`, `Wallet.cs`
- Test: `Assets/_Game/Tests/EconomyTests.cs`

**Interfaces:**
- Produces: `IdleRates.FromJson(string)`; `double IdleRates.PerHour(string resource, int highestStage)`; `string[] IdleRates.Resources` = `{"gold","heroXp","starlight"}`; `double CapHours`, `double HourglassHours`.
- `Wallet`: `Dictionary<string, double> Amounts`; `void Add(string res, double amount)`; `double Get(string res)`.
- `IdleChest`: fields `long LastSettled` (unix s), `double AccruedSeconds`, `Dictionary<string,double> Held`; `void Settle(long now, int highestStage, IdleRates r)` (adds loot for `min(max(0, now−LastSettled), cap−AccruedSeconds)` seconds at that stage's rate, sets LastSettled = now even when now < LastSettled); `void Collect(long now, int highestStage, IdleRates r, Wallet w)` (settle, move Held to wallet, zero AccruedSeconds); `void UseHourglass(int highestStage, IdleRates r, Wallet w)`.
- `economy.json`: `{ "idle": { "capHours": 24, "hourglassHours": 2, "rates": { "gold": {"at30": 7000, "growth": 0.0075}, "heroXp": {"at30": 3000, "growth": 0.0075}, "starlight": {"at30": 24, "growth": 0.006} } } }`

- [ ] **Step 1: Write failing tests:**
  - `Rates_MatchPlanTable` — PerHour("gold",30)=7000; PerHour("gold",600) within 1% of 490,000; PerHour("heroXp",600) within 1% of 210,000; PerHour("starlight",600) within 3% of 720; PerHour(*,0)=0.
  - `Chest_FillsLinearly` — settle 1 h after start at stage 30 → Held gold 7000.
  - `Chest_StopsAt24h` — settle after 5 days → Held gold = 24 × 7000 (Review Focus 2).
  - `Chest_ClockBackwards_AddsNothing` — settle at t=3600 then t=0 then t=3600 → Held gold 7000, not 14,000 or less (Review Focus 1).
  - `Chest_RateChange_SplitsAtSettle` — 1 h at stage 30, settle, 1 h at stage 31 → gold = 7000 + 7000×1.0075 (Review Focus 3).
  - `Collect_MovesLootAndResetsCap`, `Hourglass_PaysTwoHours_IgnoringCap`.
- [ ] **Step 2: Run** `pwsh -File tools/csharp/test.ps1 Economy` → FAIL.
- [ ] **Step 3: Implement** the three classes and economy.json; `GameData` gains `IdleRates Idle` (loaded when economy.json exists).
- [ ] **Step 4: Run** all tests → pass.
- [ ] **Step 5: Commit** "Phase 1c: idle chest with 24 h hard cap (row 33)".

### Task 4: Stages data model and campaign state

**Files:**
- Create: `Assets/_Game/Scripts/Core/Campaign/StageDef.cs`, `CampaignState.cs`, `SaveGame.cs`
- Test: `Assets/_Game/Tests/CampaignTests.cs` (uses a 3-stage inline stages JSON, not the real file)

**Interfaces:**
- Consumes: Task 1 `StatScaling`, Task 2 `Json.Write`, Task 3 `IdleChest`, `Wallet`, `IdleRates`.
- `stages.json` schema: `{ "stages": [ { "id": "ch01_s01", "index": 1, "chapter": 1, "stage": 1, "gate": false, "boss": false, "packages": ["Burn"], "enemies": [ { "hero": "cassia", "level": 1, "stars": 1 } ], "power": 1234, "firstClear": { "gold": 5640, "heroXp": 2420 } } ] }`
- Produces: `StageDef` (fields above; `static List<StageDef> ListFromJson(string)`); `GameData.Stages : List<StageDef>` indexed by `index − 1`.
- `static int Expected.Level(int stageIndex)`, `static int Expected.Stars(int chapter)` (Global Constraints formulas) in `CampaignState.cs`.
- `static long TeamPower.Of(GameData g, IList<string> heroes, int level, int stars)` = round(Σ (HP × 0.1 + ATK + DEF) × Mult(level, stars)).
- `CampaignState`: `int HighestCleared`; `Wallet Wallet`; `IdleChest Chest`; `bool Auto`; `Dictionary<string, List<string>> LastSetup` (key = mode, "campaign" now; row 18); `ulong Seed`; `int Attempts`.
  - `TeamSetup PlayerSetup(GameData g, int stageIndex, IList<string> heroes)` and `TeamSetup EnemySetup(GameData g, int stageIndex)` — per-hero `Scales` from `Mult(level, stars)`.
  - `FightResult Fight(GameData g, int stageIndex, IList<string> heroes, long now)` — throws `InvalidOperationException` if stageIndex > HighestCleared + 1 or > Stages.Count; seed = `new Rng(Seed ^ (ulong)Attempts).NextULong()`, Attempts++; stores LastSetup; on a win of HighestCleared + 1: settle chest at the old stage, HighestCleared++, add firstClear to Wallet. Returns `FightResult { BattleResult Battle; bool FirstClear; int StageIndex }`.
  - `bool AutoContinues(FightResult r)` = Auto && win && stage < Stages.Count.
- `SaveGame`: `const int Version = 1`; `static string Write(CampaignState s)`; `static CampaignState Read(string json)` → returns a fresh state for null/empty/unparseable text; throws `InvalidDataException` when the file's version > Version.

- [ ] **Step 1: Write failing tests:** `Expected_LevelAndStars` (Level(1)=1, Level(300)=100, Level(600)=200; Stars(1)=1, Stars(5)=2, Stars(20)=5); `Fight_NextStageWin_GrantsFirstClearOnce` and `Replay_ClearedStage_NoSecondReward_NoRateChange` (Review Focus 5; use a stage the reference team wins: a dummy enemy); `Fight_SkippingAhead_Throws`; `Fight_IsDeterministic_ForSameSaveSeed`; `AutoContinues_StopsOnLossAndAtLastStage`; `Save_RoundTrips`; `Save_Corrupt_StartsFresh`; `Save_NewerVersion_Throws` (Review Focus 4).
- [ ] **Step 2: Run** `pwsh -File tools/csharp/test.ps1 Campaign` → FAIL.
- [ ] **Step 3: Implement** StageDef, Expected, TeamPower, CampaignState, SaveGame; `GameData.Load` reads stages.json when present.
- [ ] **Step 4: Run** all tests → pass.
- [ ] **Step 5: Commit** "Phase 1c: campaign state, first clears, auto mode and saves (rows 21, 34)".

### Task 5: Stage generator and the phase 1c gate

**Files:**
- Create: `Assets/_Game/Tests/Tools/StageGen.cs`, `Assets/_Game/Tests/Tools/CampaignGate.cs`, `Assets/_Game/Data/stages.json` (generated), `docs/design/campaign-gate-v1.md` (tool output)
- Test: `Assets/_Game/Tests/StagesDataTests.cs`

**Interfaces:**
- Consumes: Tasks 1, 4.
- `StageGen.Main(args: seed=1, battlesPerCheck=40)` writes `Data/stages.json` with `Json.Write(pretty: true)`.
- Packages: the 12 names of row 30, in that row's order. Chapter c teaches packages `P[(2c−2) % 12]` and `P[(2c−1) % 12]`, so all 12 appear by chapter 6.
- Enemy team (seeded `Rng(seed + index)`): role template tank, dps, dps, healer, support; prefer heroes whose `applies` or `payoffs` include a chapter package; at least 2 of the homeland race; no duplicates. Gate stages (10, 20, 30): all 5 must carry the stage's one package (`P` of `(stage/10 − 1) % 2` of the chapter's pair; stage 30 uses the first), roles relaxed if needed.
- Difficulty: reference = 4 teams of template tank, dps, dps, healer, support drawn with `Rng(seed)` from the whole roster, at `Expected` level and stars. Enemy stars = Expected.Stars(chapter); enemy level = Expected.Level + offset. Pick the smallest integer offset in [−20, +40] whose reference win rate (4 teams × battlesPerCheck/4 seeds) is ≤ 70% for stages 1-25 of a chapter, ≤ 55% for stages 26-30 (the ramp). Clamp final level to [1, 200].
- `CampaignGate.Main(args: chapters=5, tries=5)`: each reference team plays stages 1 … chapters×30 in order at Expected level/stars, up to `tries` seeds per stage; prints a markdown table (team, last stage cleared, stages needing > 1 try) and returns exit code 0 only if every team clears all.

- [ ] **Step 1: Write failing tests** in StagesDataTests (read the real file): `Has600StagesWithStableIds` (ids ch01_s01 … ch20_s30 in order), `EveryEnemyIsARosterHero_LevelsAndStarsInRange` (level 1-200, stars 1-5), `GateStagesShareOnePackage`, `AllTwelvePackagesAppearByChapter6`, `FirstClearMatchesOneHourOfIdle` (±1).
- [ ] **Step 2: Run** `pwsh -File tools/csharp/test.ps1 StagesData` → FAIL (file missing).
- [ ] **Step 3: Implement** StageGen; run `pwsh -File tools/csharp/test.ps1 -Main Gacha.Tests.Tools.StageGen` (expect a few minutes; prints per-chapter offsets).
- [ ] **Step 4: Run** all tests → pass.
- [ ] **Step 5: Implement** CampaignGate; run `pwsh -File tools/csharp/test.ps1 -Main Gacha.Tests.Tools.CampaignGate 5 5` → exit 0. Save its output as `docs/design/campaign-gate-v1.md`. If it fails, report to the owner before changing the generator rules.
- [ ] **Step 6: Commit** "Phase 1c: 600 generated stages and the chapters 1-5 gate"; add a testing.md row for StageGen and CampaignGate.

### Task 6: Unity glue — data, clock, save, scene

**Files:**
- Create: `Assets/_Game/Scripts/Game/Gacha.Game.asmdef` (refs Gacha.Core), `GameService.cs`, `Assets/_Game/Scripts/Editor/Gacha.Editor.asmdef` (Editor only), `SceneBuilder.cs`; generated `Assets/_Game/Scenes/Main.unity` and `Assets/_Game/UI/PanelSettings.asset` (+ .meta files)
- Modify: `ProjectSettings/EditorBuildSettings.asset` only through SceneBuilder

**Interfaces:**
- `GameService : MonoBehaviour` singleton: `GameData Data`, `CampaignState State`, `long Now` (UTC unix seconds), `void Save()` (to `Application.persistentDataPath/save.json`, write temp then replace), loads on Awake, saves on pause/quit. Data from `Application.dataPath + "/_Game/Data"` (editor only; Android loading is out of scope).
- `SceneBuilder` menu item `Gacha/Build Main Scene`: creates the scene with one GameObject holding `GameService` and a `UIDocument` using the generated PanelSettings (reference resolution 1080 × 2340, scale with screen size, match height), adds it to build settings.

- [ ] **Step 1:** Write files; run `unity command recompile` then `unity command recompile_status` → completed; `unity command console_status` → no compile errors.
- [ ] **Step 2:** Run the menu item via the CLI (`unity command menu` with path `Gacha/Build Main Scene`); confirm Main.unity exists and `git status` shows only the expected new files.
- [ ] **Step 3:** `unity command run_tests --mode editor --timeout 300 --result-only` → all pass.
- [ ] **Step 4: Commit** "Phase 1c: Unity glue, save file and generated main scene".

### Task 7: Screens — map, pre-battle, battle, result

**Files:**
- Create: `Assets/_Game/UI/Common/common.uss`, `UI/CampaignMap/campaign_map.uxml/.uss`, `UI/PreBattle/pre_battle.uxml/.uss`, `UI/Battle/battle.uxml/.uss`, `UI/Result/result.uxml/.uss`; `Assets/_Game/Scripts/UI/Gacha.UI.asmdef`, `ScreenRouter.cs`, `CampaignMapController.cs`, `PreBattleController.cs`, `BattleViewController.cs`, `ResultController.cs`

**Interfaces:**
- Consumes: Task 4 `CampaignState`, Task 6 `GameService`.
- `ScreenRouter.Show(string screen, object arg = null)` swaps one root VisualElement; screens: "map", "prebattle" (arg stage index), "battle" (arg `FightResult` — plays it back), "result".
- Map: current chapter's 30 banners (stage number + recommended power), token on HighestCleared + 1, chapter arrows, idle chest showing per-hour rates and held loot with a Collect button, Auto toggle, unlock badges for ch 2-6 items (row 35; badge only, no pop-up).
- Pre-battle: enemy five (name, role, level, stars as flat colour tiles with labels), your five slots pre-filled from LastSetup, a scrollable 60-hero picker, Fight button.
- Battle: replays the sim by re-running a `Battle` with the same seed and setups, stepping `RunFor(Time.deltaTime × speed)` (1×/2×), units as flat colour boxes by race with name and HP bar at their X/Y.
- Result: win/loss, first-clear rewards, new idle gold/h, buttons Next / Retry / Map; when `AutoContinues` the next stage starts after 1.5 s.
- Placeholder colours by race only; no art; touch targets ≥ 44 px at 1080 × 2340.

- [ ] **Step 1:** Build map + router; enter play mode via CLI, capture a screenshot, check the map shows 30 banners and the chest.
- [ ] **Step 2:** Pre-battle and battle view; play stage 1 in play mode; screenshot mid-battle.
- [ ] **Step 3:** Result + auto mode; with Auto on, confirm 3 stages run back to back and it stops on a forced loss (pick 1 weak hero).
- [ ] **Step 4:** Restart play mode and confirm progress and chest survive (save/load).
- [ ] **Step 5:** All tests (local runner and Unity) pass; commit "Phase 1c: greybox campaign screens and auto mode".

### Task 8: Docs and hand-off

- [ ] Update CLAUDE.md "Current phase" (1c done), `docs/design/testing.md` rows for new tools, and the plan doc's build-order label "6 star ranks" → "10 star ranks".
- [ ] Show the owner screenshots and the gate report; merge `feature/phase-1c-campaign` to main only after approval.
