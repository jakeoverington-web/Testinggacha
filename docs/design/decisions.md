# Design decisions log

Source of truth for game design. Change a row only with the owner's agreement, and note the date.
Full plan: Gacha Game Design Plan v0.1 (Claude Docs, 2 Oct 2026).

| # | Topic | Decision | Notes | Date |
| --- | --- | --- | --- | --- |
| 1 | Combat | Idle auto-battle | No input during fights; progress continues offline | 2026-10-02 |
| 2 | Team | 5 heroes | Fixed 2 front + 3 back at first; positions stored as grid coordinates for free formation later | 2026-10-02 |
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
| 15 | Pop-ups | None unprompted | Banned: purchase offers, sale banners, login reward windows, event announcements, anything that appears without a tap. Allowed: anything the player taps to open (skill info, tooltips, panels, confirmations) | 2026-10-02 |

## Open questions

- What the hard pity number and spark threshold should be (row 10).
- Pity and spark thresholds; max stars and costs per star.
- Number of elements, factions and gear slots per hero.
- Idle-loot cap in hours.
- Platform plan for adult art (store-safe build + uncensored version elsewhere?).
