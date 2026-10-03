# Campaign gate v1 (phase 1c)

Gate: a player who owns the 4 StageGen reference teams clears chapters 1-5 at the expected level and stars, swapping team after seeing the enemy (decisions row 18; owner's pick 2026-10-03). **Result: passed.**

How the stages were tuned (owner's picks, 2026-10-03): chapter 1 fields 3 enemies and chapter 2 fields 4, without synergy preference on normal stages; each stage's enemy level is the lowest at which the best of the 4 reference teams wins 50-70% (40-55% in a chapter's last 5 stages). Command: `tools/csharp/test.sh -Main Gacha.Tests.Tools.CampaignGate 5 15`.

```
Campaign gate: chapters 1-5 (150 stages), 4 reference teams (StageGen seed 1), up to 15 tries per team per stage.

| Team | Heroes | Stages won |
| --- | --- | --- |
| 1 | eldrith, zaria, nerissa, selene, fenna | 56 |
| 2 | valeria, nyx, ilyra, aurelle, ondine | 75 |
| 3 | hartwen, sylwen, solenne, lunaith, rosalind | 14 |
| 4 | runa, ysra, lucienne, mordessa, elowen | 5 |

Cleared: all 150. Won on the first try: 73. Needed more than 5 tries: 26 (most: 39). Team swaps: 12 (ch01_s07  team 2, ch01_s14  team 3, ch01_s25  team 4, ch01_s28  team 1, ch01_s30  team 2, ch02_s08  team 4, ch02_s09  team 1, ch02_s13  team 2, ch02_s23  team 3, ch02_s26  team 4, ch02_s27  team 1, ch04_s11  team 2).

GATE PASSED
```

Findings:

- Battles barely depend on luck: one team wins a stage almost always or almost never, so the pick of team decides most stages. A single random team cannot clear the campaign (in an earlier tuning, team 4 alone would likely have stalled on 57 of the first 150 stages).
- 26 of 150 stages needed more than 5 tries and the worst needed 39 across all four teams. With levels fixed in phase 1 there is no other lever; phase 2 levelling and the economy sim should shrink this.
- Synergy-built enemy teams beat random balanced teams by a wide margin (Burn and Soaked gates sat at about half the expected level before the early-chapter changes), which matches row 29's aim.
