# Package probe v1 (after roster pass 9, 2026-10-03)

Why a synergy package wins or loses. Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.PackageProbe 200` and `... PackageProbe 200 null` (control experiment), about 3 min each.

**Findings.**
- **Payoffs now pay.** Switching the payers' passives off costs +13 to +24 points in Bleed, Charmed, Burn, Soaked, Stealth, Isolated and Grouped. This column overstates a little: switching off any 3 random heroes' passives costs about +4 (null run).
- **The heroes' base is the gap.** With payoffs off, most packages start 10-30 points behind a random team; that matches the sum of their heroes' individual strength in the niche report (Rooted: six heroes averaging about -5).
- **Control payoffs needed the 3 s window.** Before it, stun, knock-up, sleep, fear and charm were on the target for 4-8% of the team's damage; with it, 11-34%.
- **The yardstick leans negative.** 16 fake packages of random heroes average -4.6 with a spread of about ±13.

## Real packages

_200 package battles per keyword (same package teams as SynergyReport). Uptime = share of damage landing while the keyword is on the target._

| Keyword | Uplift vs random team | From payoffs (payers' passives on vs off) | Base (passives off vs random team) | Team damage with keyword on target % | Payers' damage with keyword on % | Payers' share of team damage % | First death is a package hero % | Package heroes: % of team damage per appearance |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Airborne | -13 | +5 | -18 | 22 | 21 | 38 | 91 | Astraea 45, Rhiannon 41, Ophira 29, Kaida 26, Hartwen 9 |
| Asleep | -6 | +7 | -13 | 11 | 12 | 55 | 92 | Nyx 31, Isaura 29, Fenna 15, Zephyra 13, Lunaith 12 |
| Bleed | +5 | +24 | -19 | 48 | 53 | 75 | 90 | Ravenna 42, Sable 25, Sangrael 19, Vesper 16, Briar 7 |
| Blind | -5 | +3 | -8 | 20 | 24 | 32 | 95 | Lucienne 34, Maelis 33, Solenne 26, Liora 21, Ondine 11 |
| Buffed | -7 | +6 | -13 | n/a | n/a | 16 | 27 | Elowen 18, Seren 16, Siora 16, Rosalind 14, Seravelle 13, Elara 13, Zephyra 12, Valeria 7 |
| Burn | +11 | +18 | -7 | 61 | 62 | 64 | 94 | Ilyra 35, Zaria 33, Maelis 27, Draxa 17, Liora 13 |
| Charmed | -1 | +20 | -21 | 16 | 22 | 44 | 95 | Calypso 34, Velisande 13, Lorelei 13 |
| Cooldown | -14 | +8 | -22 | n/a | n/a | 38 | 83 | Halcyra 36, Seravelle 12, Elowen 11, Tempra 10 |
| Curse | -9 | +5 | -14 | 9 | 16 | 42 | 92 | Maelis 29, Caelith 22, Vesper 19 |
| Dispel | +2 | +2 | -1 | n/a | n/a | 23 | 83 | Caelith 24, Cassia 23, Amarante 12, Rosalind 11 |
| Drained | -12 | +10 | -22 | 61 | 66 | 47 | 89 | Isaura 34, Liora 16, Lorelei 12 |
| Energy | -16 | +3 | -19 | n/a | n/a | 40 | 70 | Rhiannon 37, Pelagia 15, Seravelle 15, Lunaith 13, Selene 13, Seren 11, Elowen 11, Zephyra 11, Thessaly 11, Marisol 9 |
| Feared | -10 | +5 | -15 | 15 | 21 | 40 | 84 | Nyx 32, Noctelle 14, Velisande 12 |
| Grouped | +5 | +13 | -8 | 77 | 78 | 54 | 94 | Ilyra 44, Astraea 40, Nerissa 26, Fenna 16, Hartwen 10, Vaela 9 |
| Isolated | -2 | +14 | -16 | 18 | 25 | 38 | 69 | Calypso 32, Thalassa 8, Corvina 8 |
| Kill | -19 | -1 | -18 | n/a | n/a | 42 | 95 | Sylwen 34, Venna 30, Sable 28, Nyx 24 |
| Lifesteal | +3 | +7 | -4 | n/a | n/a | 34 | 44 | Cassia 20, Sangrael 18, Velisande 11, Mordessa 8 |
| Mark | -11 | +1 | -11 | 18 | 17 | 42 | 95 | Sylwen 43, Ysra 31, Caelith 26, Nyx 23, Solenne 21, Seren 14 |
| Rooted | -23 | +9 | -32 | 34 | 34 | 75 | 95 | Sylwen 38, Kaida 34, Nerissa 27, Wren 14, Hartwen 10, Eldrith 9 |
| Shielded | -3 | +1 | -4 | n/a | n/a | 11 | 19 | Aurelle 15, Mireille 13, Odette 12, Nimue 11, Selene 10, Runa 10, Coralie 9, Eldrith 7, Valeria 7, Isolde 7 |
| Slow | -9 | +6 | -15 | 40 | 34 | 49 | 99 | Venna 36, Sylwen 35, Nerissa 32, Fenna 20, Pelagia 12, Siora 12, Elowen 10, Thalassa 9 |
| Soaked | +10 | +16 | -6 | 53 | 57 | 69 | 96 | Halcyra 40, Ysra 35, Pelagia 15, Lorelei 15, Ondine 13, Marisol 11, Thalassa 8, Coralie 7 |
| Stealth | -3 | +15 | -18 | n/a | n/a | 59 | 94 | Calypso 38, Nyx 27, Noctelle 14, Thessaly 13 |
| Stunned | -4 | +3 | -7 | 27 | 25 | 47 | 99 | Halcyra 43, Lucienne 33, Ophira 30, Ondine 14, Amarante 13, Thalassa 10, Valeria 7 |
| Summon | -14 | +6 | -20 | n/a | n/a | 58 | 85 | Rhiannon 31, Kaida 27 |
| Taunted | -15 | +7 | -22 | 19 | 25 | 12 | 25 | Sangrael 27, Hartwen 11, Briar 11, Eldrith 8, Vaela 8, Corvina 8, Coralie 7, Valeria 7 |
| Weakened | -11 | +2 | -13 | 26 | 23 | 45 | 95 | Caelith 36, Solenne 33, Liora 16, Amarante 14, Wren 12, Ondine 11 |

## Control: fake packages of random heroes (3 appliers + 3 payers)

_200 package battles per keyword (same package teams as SynergyReport). Uptime = share of damage landing while the keyword is on the target._

| Keyword | Uplift vs random team | From payoffs (payers' passives on vs off) | Base (passives off vs random team) | Team damage with keyword on target % | Payers' damage with keyword on % | Payers' share of team damage % | First death is a package hero % | Package heroes: % of team damage per appearance |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| _null0 | +9 | +14 | -6 | n/a | n/a | 45 | 96 | Calypso 36, Nyx 36, Nimue 12, Isolde 11, Coralie 7, Valeria 7 |
| _null1 | +1 | +7 | -7 | n/a | n/a | 46 | 93 | Rhiannon 38, Nyx 26, Cassia 20, Lorelei 12, Selene 10, Nimue 10 |
| _null2 | -11 | +3 | -14 | n/a | n/a | 33 | 88 | Kaida 37, Maelis 32, Mireille 21, Nimue 11, Thessaly 11, Mordessa 10 |
| _null3 | +7 | +10 | -3 | n/a | n/a | 39 | 97 | Maelis 36, Calypso 34, Rosalind 14, Aurelle 12, Thessaly 12, Valeria 6 |
| _null4 | +9 | +11 | -1 | n/a | n/a | 49 | 95 | Calypso 37, Astraea 36, Sangrael 21, Amarante 11, Nimue 10, Coralie 6 |
| _null5 | +9 | +13 | -4 | n/a | n/a | 55 | 98 | Ilyra 41, Astraea 37, Zaria 33, Liora 14, Thessaly 10, Zephyra 9 |
| _null6 | -9 | +4 | -13 | n/a | n/a | 13 | 22 | Draxa 26, Mireille 14, Nimue 11, Rosalind 10, Runa 9, Valeria 7 |
| _null7 | -13 | +3 | -15 | n/a | n/a | 36 | 94 | Rhiannon 41, Sylwen 35, Caelith 23, Nyx 23, Tempra 13, Zephyra 11 |
| _null8 | -5 | +1 | -6 | n/a | n/a | 22 | 81 | Calypso 31, Siora 14, Velisande 14, Elara 12, Hartwen 11, Valeria 7 |
| _null9 | -3 | +2 | -5 | n/a | n/a | 31 | 95 | Sable 32, Solenne 30, Noctelle 16, Pelagia 13, Rosalind 12, Corvina 8 |
| _null10 | -11 | +3 | -13 | n/a | n/a | 31 | 97 | Ilyra 45, Zaria 38, Isaura 29, Siora 10, Lorelei 10, Isolde 7 |
| _null11 | -1 | +6 | -7 | n/a | n/a | 44 | 98 | Isaura 33, Nyx 30, Solenne 30, Fenna 17, Aurelle 14, Mordessa 11 |
| _null12 | -21 | -1 | -20 | n/a | n/a | 43 | 95 | Lucienne 32, Kaida 31, Sable 29, Nyx 25, Vesper 20, Lunaith 12 |
| _null13 | -23 | +4 | -27 | n/a | n/a | 31 | 96 | Rhiannon 43, Astraea 38, Sable 28, Nyx 28, Velisande 11, Isolde 8 |
| _null14 | -11 | +7 | -19 | n/a | n/a | 12 | 77 | Isaura 33, Noctelle 10, Isolde 8, Corvina 8, Hartwen 8, Coralie 6 |
| _null15 | -1 | +2 | -3 | n/a | n/a | 19 | 83 | Maelis 33, Cassia 22, Pelagia 13, Noctelle 11, Corvina 8, Eldrith 7 |
