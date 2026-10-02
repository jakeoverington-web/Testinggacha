# Design decisions log

Source of truth for game design. Change a row only with the owner's agreement, and note the date.
Full plan: Gacha Game Design Plan v0.1 (Claude Docs, 2 Oct 2026).

| # | Topic | Decision | Notes | Date |
| --- | --- | --- | --- | --- |
| 1 | Combat | Idle auto-battle | No input during fights; progress continues offline | 2026-10-02 |
| 2 | Team | 5 heroes, 11 formations | Two rows: 5, 4-1, 3-2, 2-3, 1-4. Three rows: 3-1-1, 2-2-1, 2-1-2, 1-3-1, 1-2-2, 1-1-3. Columns of 4-5 are drawn as a compact zig-zag. Counters come from enemy skills (row hits, assassins, nukes), not flat formation bonuses | 2026-10-02 |
| 3 | Team-building | Element counters + faction bonuses | Mythic Heroes-style hybrid | 2026-10-02 |
| 4 | Progression | Campaign + endless tower | Idle loot scales with furthest campaign stage | 2026-10-02 |
| 5 | Duplicates | Raise stars, or feed as fodder | Fodder ascends other heroes | 2026-10-02 |
| 6 | Gear | Per-hero gear slots | Drops from stages; upgradeable | 2026-10-02 |
| 7 | Levels | Shared level sync | Top 5 heroes set the level for all others | 2026-10-02 |
| 8 | Bonds | Not at launch | Reserve data fields only | 2026-10-02 |
| 9 | Banners | One limited rate-up banner | Featured hero joins standard pool when banner ends | 2026-10-02 |
| 10 | Pity | Hard pity that carries over + spark points | Spark lets players pick the featured hero | 2026-10-02 |
| 11 | Launch modes | Arena PvP, Guild + guild boss | Events later via reusable special encounter | 2026-10-02 |
| 12 | Rarity | No rarity tiers; stars only | Confirmed by owner | 2026-10-02 |
| 13 | Engine | Unity, C# | Chosen for official Live2D/Spine support and mobile SDKs | 2026-10-02 |
| 14 | Build order | Systems first, art last | Placeholders until phase 5 | 2026-10-02 |
| 15 | Pop-ups | None unprompted | Banned: purchase offers, sale banners, login reward windows, event announcements, scrolling announcement tickers, anything that appears without a tap. Allowed: anything the player taps to open (skill info, tooltips, panels, confirmations) | 2026-10-02 |
| 16 | Battle layout | Side view: allies left, enemies right | 2 front (nearest centre) + 3 back per side; Team screen uses the same stage. Chosen over top-vs-bottom and diagonal for proven idle feel and one-sprite-per-hero art cost | 2026-10-02 |
| 17 | Team presets | 5 saved presets (team + formation), edited in the Team tab with no enemy shown | Presets can be loaded in any game mode | 2026-10-02 |
| 18 | Pre-battle setup | Campaign, Boss, PvP etc. show the enemy formation; player loads a preset or builds a custom setup | Last-used setup is remembered per game mode and auto-fills the next stage | 2026-10-02 |
| 19 | Battle movement | Real-time, free-flowing | Formation sets starting positions only; heroes then move, close distance and fight (melee run in, ranged hang back). Needs move speed + attack range per hero; sim runs on a fixed timestep with seeded RNG so it stays replayable. Reference: Mythic Heroes battle | 2026-10-02 |
| 20 | Art direction | WLOP is the art guideline: dark painterly fantasy; adult/NSFW content | Replaces the earlier Omni Heroes / Azur Lane direction. Five habits: sharp face and jewellery with loose edges; one saturated accent colour on a desaturated world; jewellery carries the detail; melancholy over cute; one light source plus drifting particles. High Elves warm high-key light, Dark Elves cool low-key light. Element sets each hero's single accent colour. Inspired, never copied (no redraws of his characters or compositions). Every character is an adult and drawn as one, with adult faces and proportions. Mythic Heroes is a reference for systems only. Full study: WLOP Style Study (Claude Docs) | 2026-10-02 |
| 21 | Campaign auto mode | Auto toggle on the current stage | Plays each battle normally, then starts the next stage after a win; stops on a loss. No batch skipping of many stages at once | 2026-10-02 |
| 22 | World theme | Elves only: High Elves and Dark Elves | Tone mixes both: sacred and serene vs sly and dangerous | 2026-10-02 |
| 23 | UI vs art | The UI follows the art | UI colours, backgrounds, shapes and fonts (currently Gold Glass) may be changed wherever they clash with the WLOP guideline | 2026-10-02 |
| 24 | UI theme | Night Gold | Replaces Gold Glass. Neutral black (#0A0A0B), muted gold hairlines (#CDB582), Cormorant SC display + Barlow body, 1px lines and near-square corners, gothic-arch card tops, element shown as a small glowing gem (no coloured card borders), card art lit by faction, diamond Battle button. Chosen over Ink & Silver and Bone & Gold | 2026-10-02 |

## Open questions

- What the hard pity number and spark threshold should be (row 10).
- Pity and spark thresholds; max stars and costs per star.
- Number of elements, factions and gear slots per hero.
- Are High and Dark Elves the two factions, or are there houses within each?
- Idle-loot cap in hours.
- ~~Platform plan for adult art~~ Resolved: personal build, installed directly on the owner's phone; no store release, so no store content rules apply.
