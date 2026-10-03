# Synergy report v2 (after roster pass 9, 2026-10-03)

Team synergy and diversity against decisions row 29. Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.SynergyReport 300 32` (about 2-3 min). New in v2: section C, a round robin among the strongest teams found, because every strong team beats random opponents 90-100% and that ranking hits a ceiling.

**Verdict: better, not done.** Against the pass 9 targets: the best packages beat a random team by 14 (Soaked, Burn; target 15); the top 10 teams use 10-11 different packages (target 6) and 24-29 different heroes; most top teams run 1-2 synergies, a few still none (target 2 each); Ophira is in 5 of the round-robin top 10 (target at most 3). Packages are still a net penalty in 19 of 27 keywords. The yardstick itself leans negative: fake packages made of random heroes average -5 with a spread of about ±13 (package-probe-v1). The losing packages are thin keywords whose enablers are modest tanks and supports (Rooted, Kill, Energy, Cooldown, Taunted, Summon, Mark, Weakened). Next step proposed: fewer, deeper packages.

_Packages: 300 paired battles each. Uplift = points a team built around the keyword gains over a random balanced team (same opponent, same seed)._

## A. Synergy packages

| Keyword | Uplift vs random team | ± | Package heroes in team (avg) | Appliers / payers in roster |
| --- | --- | --- | --- | --- |
| Soaked | +14 | 3 | 5 | 7 / 4 |
| Burn | +14 | 4 | 3 | 5 / 4 |
| Lifesteal | +8 | 4 | 3 | 3 / 3 |
| Bleed | +3 | 4 | 4 | 4 / 4 |
| Stealth | -1 | 4 | 3 | 2 / 3 |
| Grouped | -1 | 4 | 4 | 4 / 3 |
| Isolated | -2 | 4 | 2 | 3 / 2 |
| Dispel | -2 | 3 | 3 | 4 / 1 |
| Stunned | -3 | 3 | 4 | 5 / 3 |
| Shielded | -3 | 4 | 2 | 8 / 3 |
| Charmed | -4 | 4 | 2 | 2 / 2 |
| Asleep | -7 | 3 | 3 | 3 / 3 |
| Curse | -7 | 3 | 3 | 2 / 2 |
| Blind | -8 | 4 | 3 | 5 / 2 |
| Slow | -8 | 3 | 5 | 7 / 3 |
| Buffed | -9 | 3 | 3 | 8 / 2 |
| Taunted | -10 | 3 | 2 | 8 / 2 |
| Drained | -10 | 4 | 2 | 3 / 2 |
| Airborne | -11 | 4 | 3 | 4 / 3 |
| Kill | -12 | 4 | 2 | 3 / 2 |
| Mark | -12 | 3 | 3 | 2 / 4 |
| Summon | -13 | 3 | 2 | 2 / 2 |
| Energy | -14 | 3 | 3 | 9 / 2 |
| Weakened | -14 | 3 | 3 | 4 / 2 |
| Cooldown | -15 | 3 | 2 | 2 / 2 |
| Feared | -15 | 4 | 2 | 2 / 2 |
| Rooted | -20 | 3 | 4 | 4 / 5 |

## B. Strongest teams found (32 hill-climbing searches, 30 swaps each, scored against the same 120 random balanced opponents)

| Win % | Team | Races | Active synergy keywords |
| --- | --- | --- | --- |
| 99 | Ophira, Ysra, Aurelle, Cassia, Thalassa | 3 high 2 ocean | Soaked, Stunned |
| 98 | Sable, Ysra, Calypso, Thessaly, Sangrael | 3 dark 2 ocean | Bleed, Stealth |
| 98 | Ophira, Calypso, Siora, Cassia, Coralie | 3 ocean 2 high |  |
| 98 | Ophira, Maelis, Ilyra, Mordessa, Valeria | 3 high 2 dark | Burn, Stunned |
| 98 | Calypso, Rhiannon, Kaida, Nimue, Coralie | 3 nature 2 ocean | Airborne, Summon |
| 97 | Ophira, Ysra, Sable, Nimue, Vaela | 1 arcane 1 nature 1 high 1 ocean 1 dark |  |
| 95 | Solenne, Sable, Maelis, Odette, Coralie | 2 ocean 2 dark 1 high | Shielded |
| 95 | Nyx, Solenne, Sylwen, Aurelle, Cassia | 3 high 1 dark 1 nature | Mark |
| 94 | Maelis, Calypso, Thessaly, Cassia, Vaela | 2 dark 1 high 1 arcane 1 ocean | Stealth |
| 93 | Caelith, Maelis, Nimue, Cassia, Vaela | 2 arcane 1 high 1 nature 1 dark | Curse, Dispel |
| 93 | Ysra, Solenne, Nimue, Sangrael, Cassia | 2 high 1 dark 1 nature 1 ocean | Lifesteal, Mark |
| 93 | Calypso, Ilyra, Siora, Valeria, Cassia | 3 high 2 ocean |  |
| 90 | Ophira, Sylwen, Selene, Ondine, Vaela | 2 arcane 1 high 1 nature 1 ocean | Stunned |
| 89 | Solenne, Nerissa, Aurelle, Liora, Sangrael | 2 dark 2 high 1 ocean | Weakened |
| 89 | Maelis, Calypso, Thessaly, Liora, Coralie | 3 dark 2 ocean | Burn, Stealth |

Gauntlet top 10: synergies per team 2/2/0/2/2/0/1/1/1/2; 11 different packages.
Diversity of the top 10: 24 different heroes; most used: Cassia 5/10, Ophira 4/10, Calypso 4/10, Maelis 4/10, Ysra 3/10, Sable 3/10.
A random balanced team wins 51% against the same gauntlet.

## C. Round robin among the 24 strongest teams found (each pair: 4 seeds, both sides)

| Rank | Round-robin win % | Team | Races | Active synergy keywords |
| --- | --- | --- | --- | --- |
| 1 | 88 | Ophira, Maelis, Ilyra, Mordessa, Valeria | 3 high 2 dark | Burn, Stunned |
| 2 | 87 | Sable, Ysra, Calypso, Thessaly, Sangrael | 3 dark 2 ocean | Bleed, Stealth |
| 3 | 80 | Ophira, Calypso, Siora, Cassia, Coralie | 3 ocean 2 high |  |
| 4 | 79 | Ophira, Ysra, Sable, Nimue, Vaela | 1 arcane 1 nature 1 high 1 ocean 1 dark |  |
| 5 | 70 | Ophira, Ysra, Aurelle, Cassia, Thalassa | 3 high 2 ocean | Soaked, Stunned |
| 6 | 69 | Calypso, Rhiannon, Kaida, Nimue, Coralie | 3 nature 2 ocean | Airborne, Summon |
| 7 | 64 | Isaura, Ravenna, Selene, Liora, Runa | 3 arcane 2 dark | Drained |
| 8 | 58 | Ophira, Sylwen, Selene, Ondine, Vaela | 2 arcane 1 high 1 nature 1 ocean | Stunned |
| 9 | 51 | Nyx, Solenne, Sylwen, Aurelle, Cassia | 3 high 1 dark 1 nature | Mark |
| 10 | 50 | Solenne, Sable, Maelis, Odette, Coralie | 2 ocean 2 dark 1 high | Shielded |
| 11 | 43 | Ravenna, Vesper, Seren, Sangrael, Vaela | 3 dark 2 arcane | Bleed, Taunted |
| 12 | 43 | Caelith, Maelis, Nimue, Cassia, Vaela | 2 arcane 1 high 1 nature 1 dark | Curse, Dispel |
| 13 | 43 | Calypso, Nimue, Liora, Seravelle, Cassia | 2 high 1 ocean 1 nature 1 dark |  |
| 14 | 42 | Maelis, Calypso, Thessaly, Liora, Coralie | 3 dark 2 ocean | Burn, Stealth |
| 15 | 41 | Calypso, Nimue, Noctelle, Lorelei, Cassia | 2 ocean 1 nature 1 high 1 dark | Charmed, Stealth |
| 16 | 40 | Solenne, Nerissa, Aurelle, Liora, Sangrael | 2 dark 2 high 1 ocean | Weakened |
| 17 | 39 | Ysra, Solenne, Nimue, Sangrael, Cassia | 2 high 1 dark 1 nature 1 ocean | Lifesteal, Mark |
| 18 | 39 | Astraea, Aurelle, Elowen, Valeria, Cassia | 3 high 2 arcane |  |
| 19 | 36 | Maelis, Calypso, Thessaly, Cassia, Vaela | 2 dark 1 high 1 arcane 1 ocean | Stealth |
| 20 | 35 | Calypso, Ilyra, Siora, Valeria, Cassia | 3 high 2 ocean |  |
| 21 | 33 | Solenne, Nimue, Zephyra, Fenna, Valeria | 3 nature 2 high | Asleep |
| 22 | 30 | Ophira, Aurelle, Lorelei, Seravelle, Cassia | 4 high 1 ocean |  |
| 23 | 21 | Nerissa, Zaria, Selene, Pelagia, Draxa | 3 arcane 2 ocean | Burn, Slow |
| 24 | 18 | Maelis, Ophira, Nimue, Seravelle, Valeria | 3 high 1 dark 1 nature | Stunned |

Round-robin top 10: synergies per team 2/2/0/0/2/2/1/1/1/1; 10 different packages; 29 different heroes; most used: Ophira 5/10, Sable 3/10, Ysra 3/10, Calypso 3/10, Cassia 3/10, Coralie 3/10.
