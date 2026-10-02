# Synergy report v1 (2026-10-03)

Team synergy and diversity against the target in decisions row 29. Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.SynergyReport 300 24` (about 2 min).

**Verdict: synergy is currently a penalty.** Building a team around a keyword loses to a random balanced team in 19 of 27 packages (worst: Slow −24, Rooted −23, Stunned −22, Kill −21, Curse −18); only Lifesteal (+10), Isolated (+6) and Charmed (+5) help. Causes: conditional payoffs are small (+15-25%), and many keywords have a single payer, so the package fills the team with enablers for one hero. The strongest teams found (87-99% against random teams) are stacked strong individuals with almost no active synergy; Cassia is in 5 of the top 10.

_Packages: 300 paired battles each. Uplift = points a team built around the keyword gains over a random balanced team (same opponent, same seed)._

## A. Synergy packages

| Keyword | Uplift vs random team | ± | Package heroes in team (avg) | Appliers / payers in roster |
| --- | --- | --- | --- | --- |
| Lifesteal | +10 | 3 | 3 | 3 / 3 |
| Isolated | +6 | 4 | 2 | 3 / 1 |
| Charmed | +5 | 4 | 2 | 2 / 1 |
| Grouped | +3 | 4 | 4 | 4 / 2 |
| Burn | +2 | 3 | 4 | 5 / 4 |
| Dispel | +2 | 4 | 3 | 4 / 1 |
| Feared | 0 | 0 | 2 | 2 / 1 |
| Shielded | -2 | 3 | 2 | 8 / 3 |
| Summon | -6 | 3 | 2 | 2 / 2 |
| Mark | -7 | 3 | 3 | 2 / 4 |
| Airborne | -7 | 3 | 3 | 4 / 2 |
| Soaked | -7 | 4 | 5 | 7 / 3 |
| Taunted | -8 | 4 | 2 | 8 / 1 |
| Cooldown | -10 | 3 | 2 | 2 / 2 |
| Stealth | -10 | 3 | 3 | 2 / 3 |
| Bleed | -11 | 3 | 4 | 4 / 3 |
| Drained | -11 | 4 | 2 | 3 / 1 |
| Buffed | -11 | 3 | 3 | 8 / 2 |
| Blind | -11 | 4 | 3 | 5 / 2 |
| Asleep | -13 | 3 | 3 | 3 / 2 |
| Energy | -14 | 4 | 3 | 9 / 1 |
| Weakened | -16 | 3 | 2 | 4 / 1 |
| Curse | -18 | 3 | 2 | 2 / 1 |
| Kill | -21 | 3 | 2 | 3 / 2 |
| Stunned | -22 | 4 | 4 | 5 / 1 |
| Rooted | -23 | 3 | 4 | 4 / 3 |
| Slow | -24 | 4 | 4 | 7 / 1 |

## B. Strongest teams found (24 hill-climbing searches, 30 swaps each, scored against the same 120 random balanced opponents)

| Win % | Team | Races | Active synergy keywords |
| --- | --- | --- | --- |
| 99 | Solenne, Ysra, Mireille, Lorelei, Cassia | 2 high 2 ocean 1 nature | Mark, Soaked |
| 99 | Ophira, Solenne, Calypso, Nimue, Valeria | 3 high 1 nature 1 ocean |  |
| 98 | Maelis, Ophira, Sylwen, Aurelle, Cassia | 3 high 1 dark 1 nature |  |
| 98 | Calypso, Isaura, Maelis, Odette, Coralie | 3 ocean 1 arcane 1 dark | Shielded |
| 97 | Ophira, Ysra, Sable, Siora, Vaela | 2 ocean 1 arcane 1 high 1 dark |  |
| 96 | Solenne, Ophira, Selene, Coralie, Cassia | 3 high 1 ocean 1 arcane |  |
| 96 | Calypso, Sylwen, Zaria, Nimue, Valeria | 2 nature 1 high 1 ocean 1 arcane |  |
| 95 | Maelis, Calypso, Isaura, Aurelle, Cassia | 2 high 1 dark 1 ocean 1 arcane |  |
| 94 | Sylwen, Maelis, Siora, Sangrael, Cassia | 2 dark 1 nature 1 high 1 ocean | Lifesteal |
| 93 | Ravenna, Zaria, Nimue, Valeria, Coralie | 1 high 1 ocean 1 nature 1 dark 1 arcane |  |
| 92 | Sylwen, Ophira, Nimue, Lorelei, Corvina | 2 nature 1 ocean 1 dark 1 high |  |
| 92 | Ophira, Aurelle, Lorelei, Seravelle, Cassia | 4 high 1 ocean |  |
| 90 | Ysra, Calypso, Selene, Wren, Cassia | 2 ocean 1 high 1 arcane 1 nature |  |
| 88 | Solenne, Rhiannon, Selene, Liora, Sangrael | 2 dark 1 arcane 1 high 1 nature | Energy |
| 87 | Nerissa, Maelis, Selene, Cassia, Vaela | 2 arcane 1 high 1 ocean 1 dark |  |

Diversity of the top 10: 22 different heroes; most used: Cassia 5/10, Ophira 4/10, Calypso 4/10, Maelis 4/10, Solenne 3/10, Valeria 3/10.
A random balanced team wins 52% against the same gauntlet.
