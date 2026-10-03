# Package probe v2 (after roster pass 10, 2026-10-03)

Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.PackageProbe 200` (about 3 min; add `null` for the fake-package control, which averages -5 with a spread of about ±13).

**Findings.** Payoffs are worth +6 to +25 points in every package (switching the payers' passives off; that column overstates by about 4). What still holds Disrupted, Exposed, Guarded and Tempo back is their heroes' base strength (-13 to -24 with payoffs off): their members are modest supports and tanks. Hindered and Gathered are now on the target for 54-82% of the team's damage; Dread 17% (sleep, fear and charm are short even with the 3 s window).

_200 package battles per keyword (same package teams as SynergyReport). Uptime = share of damage landing while the keyword is on the target._

| Keyword | Uplift vs random team | From payoffs (payers' passives on vs off) | Base (passives off vs random team) | Team damage with keyword on target % | Payers' damage with keyword on % | Payers' share of team damage % | First death is a package hero % | Package heroes: % of team damage per appearance |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Burn | +7 | +13 | -7 | 61 | 61 | 62 | 97 | Ilyra 38, Zaria 35, Maelis 26, Draxa 18, Liora 12 |
| Disrupted | -9 | +7 | -16 | 41 | 52 | 43 | 98 | Maelis 45, Lucienne 36, Isaura 32, Caelith 29, Solenne 27, Liora 18, Lorelei 13, Ondine 13, Runa 12, Amarante 10, Vaela 10 |
| Dread | 0 | +14 | -14 | 17 | 20 | 62 | 97 | Calypso 33, Nyx 31, Isaura 30, Fenna 22, Velisande 15, Lunaith 14, Noctelle 13, Zephyra 11, Lorelei 11 |
| Exposed | -8 | +6 | -13 | 30 | 28 | 59 | 98 | Sylwen 41, Ysra 34, Maelis 29, Caelith 28, Solenne 22, Vesper 19, Seren 13, Liora 13, Amarante 11, Wren 10, Ondine 9 |
| Gathered | -2 | +10 | -11 | 82 | 87 | 40 | 97 | Astraea 41, Sangrael 28, Nerissa 26, Fenna 17, Vaela 10, Valeria 9, Hartwen 8, Thalassa 8, Briar 8, Eldrith 7, Coralie 7, Corvina 6 |
| Guarded | -12 | +6 | -18 | n/a | n/a | 14 | 24 | Mireille 18, Seren 16, Elowen 14, Aurelle 14, Zephyra 13, Odette 13, Siora 13, Runa 13, Rosalind 12, Nimue 12, Elara 11, Selene 11, Seravelle 10, Coralie 9, Isolde 8, Valeria 8 |
| Hindered | -4 | +6 | -10 | 54 | 53 | 49 | 100 | Halcyra 46, Sylwen 42, Rhiannon 37, Ophira 35, Venna 35, Ysra 35, Astraea 32, Nerissa 31, Kaida 31, Lucienne 26, Fenna 24, Pelagia 16, Wren 13, Siora 13, Elowen 13, Ondine 13, Amarante 11, Hartwen 10, Eldrith 9, Thalassa 8, Valeria 7 |
| Lifesteal | +6 | +7 | -2 | n/a | n/a | 23 | 41 | Cassia 19, Sangrael 19, Velisande 9, Mordessa 9 |
| Soaked | +15 | +14 | +1 | 53 | 56 | 68 | 93 | Halcyra 38, Ysra 36, Ondine 16, Lorelei 16, Pelagia 14, Marisol 12, Thalassa 10, Coralie 7 |
| Stealth | +1 | +19 | -19 | n/a | n/a | 56 | 94 | Calypso 31, Nyx 28, Noctelle 13, Thessaly 13 |
| Tempo | -7 | +17 | -24 | n/a | n/a | 72 | 94 | Ilyra 46, Halcyra 33, Rhiannon 33, Seren 13, Pelagia 12, Thessaly 12, Marisol 12, Zephyra 11, Tempra 11, Elowen 10, Seravelle 10, Selene 10, Lunaith 7 |
| Wounds | +7 | +25 | -18 | 60 | 64 | 78 | 99 | Ravenna 39, Sable 28, Venna 25, Sangrael 18, Vesper 16, Fenna 13, Briar 8 |
