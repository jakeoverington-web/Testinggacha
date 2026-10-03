# Synergy report v3 (after roster pass 10, 2026-10-03)

Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.SynergyReport 300 32` (about 3 min).

**Verdict: synergy now shapes every top team.** Pass 10 merged 27 keywords into 12 packages (decisions row 30). Ahead of a random team: Soaked +14, Wounds +9, Burn +6, Lifesteal +5, Stealth 0. Near zero (fake random-hero packages average -5): Gathered, Dread, Hindered (-2 to -3). Behind: Disrupted -7, Exposed -8, Guarded -8, Tempo -10. Every one of the 10 strongest teams runs at least one synergy; they use 8 of the 12 packages and 28 different heroes; no hero is in more than 4 of the 10 (target 3).

_Packages: 300 paired battles each. Uplift = points a team built around the keyword gains over a random balanced team (same opponent, same seed)._

## A. Synergy packages

| Keyword | Uplift vs random team | ± | Package heroes in team (avg) | Appliers / payers in roster |
| --- | --- | --- | --- | --- |
| Soaked | +14 | 3 | 5 | 7 / 4 |
| Wounds | +9 | 4 | 5 | 6 / 5 |
| Burn | +6 | 4 | 3 | 5 / 4 |
| Lifesteal | +5 | 3 | 3 | 3 / 2 |
| Stealth | 0 | 3 | 3 | 2 / 3 |
| Gathered | -2 | 4 | 4 | 11 / 2 |
| Dread | -3 | 4 | 4 | 7 / 5 |
| Hindered | -3 | 4 | 5 | 19 / 8 |
| Disrupted | -7 | 4 | 4 | 11 / 4 |
| Exposed | -8 | 4 | 4 | 8 / 6 |
| Guarded | -8 | 4 | 3 | 15 / 4 |
| Tempo | -10 | 4 | 4 | 10 / 5 |

## B. Strongest teams found (32 hill-climbing searches, 30 swaps each, scored against the same 120 random balanced opponents)

| Win % | Team | Races | Active synergy keywords |
| --- | --- | --- | --- |
| 99 | Calypso, Sable, Maelis, Thessaly, Coralie | 3 dark 2 ocean | Stealth |
| 97 | Nyx, Ysra, Nerissa, Thessaly, Sangrael | 3 dark 2 ocean | Hindered, Stealth |
| 97 | Ophira, Maelis, Ilyra, Mordessa, Valeria | 3 high 2 dark | Burn, Hindered |
| 97 | Caelith, Ophira, Solenne, Mireille, Cassia | 3 high 1 nature 1 arcane | Exposed |
| 96 | Ophira, Maelis, Zaria, Nimue, Vaela | 2 arcane 1 nature 1 high 1 dark | Burn |
| 96 | Ilyra, Isaura, Lucienne, Selene, Vaela | 3 arcane 2 high | Disrupted, Tempo |
| 95 | Calypso, Sylwen, Rhiannon, Odette, Coralie | 3 ocean 2 nature | Guarded, Hindered |
| 95 | Sylwen, Ilyra, Nimue, Valeria, Cassia | 3 high 2 nature | Hindered |
| 95 | Calypso, Maelis, Isaura, Siora, Cassia | 2 ocean 1 dark 1 arcane 1 high | Disrupted, Dread |
| 95 | Ophira, Calypso, Elara, Cassia, Coralie | 3 high 2 ocean | Guarded |
| 89 | Solenne, Ravenna, Selene, Tempra, Valeria | 2 high 2 arcane 1 dark |  |
| 88 | Nerissa, Siora, Lorelei, Liora, Sangrael | 3 ocean 2 dark | Disrupted, Hindered |
| 87 | Solenne, Rhiannon, Selene, Tempra, Coralie | 2 arcane 1 ocean 1 high 1 nature | Tempo |
| 86 | Kaida, Calypso, Siora, Wren, Coralie | 3 ocean 2 nature | Hindered |
| 85 | Ophira, Aurelle, Elowen, Cassia, Vaela | 3 high 2 arcane | Hindered |

Gauntlet top 10: synergies per team 1/2/2/1/1/2/2/1/2/1; 8 different packages.
Diversity of the top 10: 28 different heroes; most used: Calypso 4/10, Maelis 4/10, Ophira 4/10, Cassia 4/10, Coralie 3/10, Ilyra 3/10.
A random balanced team wins 51% against the same gauntlet.

## C. Round robin among the 24 strongest teams found (each pair: 4 seeds, both sides)

| Rank | Round-robin win % | Team | Races | Active synergy keywords |
| --- | --- | --- | --- | --- |
| 1 | 91 | Ophira, Maelis, Ilyra, Mordessa, Valeria | 3 high 2 dark | Burn, Hindered |
| 2 | 86 | Calypso, Sable, Maelis, Thessaly, Coralie | 3 dark 2 ocean | Stealth |
| 3 | 79 | Caelith, Ophira, Solenne, Mireille, Cassia | 3 high 1 nature 1 arcane | Exposed |
| 4 | 74 | Calypso, Sylwen, Rhiannon, Odette, Coralie | 3 ocean 2 nature | Guarded, Hindered |
| 5 | 74 | Ophira, Maelis, Zaria, Nimue, Vaela | 2 arcane 1 nature 1 high 1 dark | Burn |
| 6 | 69 | Ophira, Calypso, Elara, Cassia, Coralie | 3 high 2 ocean | Guarded |
| 7 | 68 | Sylwen, Ilyra, Nimue, Valeria, Cassia | 3 high 2 nature | Hindered |
| 8 | 63 | Ilyra, Isaura, Lucienne, Selene, Vaela | 3 arcane 2 high | Disrupted, Tempo |
| 9 | 59 | Calypso, Maelis, Isaura, Siora, Cassia | 2 ocean 1 dark 1 arcane 1 high | Disrupted, Dread |
| 10 | 57 | Nyx, Ysra, Nerissa, Thessaly, Sangrael | 3 dark 2 ocean | Hindered, Stealth |
| 11 | 53 | Solenne, Ravenna, Selene, Tempra, Valeria | 2 high 2 arcane 1 dark |  |
| 12 | 51 | Solenne, Rhiannon, Selene, Tempra, Coralie | 2 arcane 1 ocean 1 high 1 nature | Tempo |
| 13 | 48 | Ophira, Aurelle, Elowen, Cassia, Vaela | 3 high 2 arcane | Hindered |
| 14 | 46 | Ravenna, Venna, Vesper, Fenna, Briar | 3 nature 2 dark | Wounds |
| 15 | 44 | Nerissa, Siora, Lorelei, Liora, Sangrael | 3 ocean 2 dark | Disrupted, Hindered |
| 16 | 38 | Kaida, Calypso, Siora, Wren, Coralie | 3 ocean 2 nature | Hindered |
| 17 | 36 | Ophira, Aurelle, Lorelei, Seravelle, Cassia | 4 high 1 ocean |  |
| 18 | 32 | Calypso, Ophira, Thessaly, Cassia, Draxa | 2 high 1 dark 1 ocean 1 arcane | Stealth |
| 19 | 31 | Ysra, Nerissa, Nimue, Sangrael, Vaela | 2 ocean 1 dark 1 arcane 1 nature | Gathered, Hindered |
| 20 | 27 | Calypso, Halcyra, Siora, Coralie, Thalassa | 5 ocean | Soaked |
| 21 | 23 | Sylwen, Selene, Ondine, Lorelei, Cassia | 2 ocean 1 nature 1 high 1 arcane | Disrupted, Exposed, Hindered, Soaked |
| 22 | 22 | Ysra, Halcyra, Marisol, Pelagia, Coralie | 5 ocean | Hindered, Soaked, Tempo |
| 23 | 16 | Ilyra, Maelis, Thessaly, Draxa, Sangrael | 3 dark 1 high 1 arcane | Burn, Tempo |
| 24 | 13 | Zaria, Mordessa, Velisande, Elowen, Vaela | 3 arcane 2 dark | Lifesteal |

Round-robin top 10: synergies per team 2/1/1/2/1/1/1/2/2/2; 8 different packages; 28 different heroes; most used: Ophira 4/10, Maelis 4/10, Calypso 4/10, Cassia 4/10, Ilyra 3/10, Coralie 3/10.
