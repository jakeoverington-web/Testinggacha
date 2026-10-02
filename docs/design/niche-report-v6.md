# Niche report v6 (auto-attack tiers, 2026-10-03)

Balance target (decisions row 29). Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.NicheReport 150 1` (and seed 2 to separate real situational swings from noise).

Change since v5: per-hero auto attack strength. Specialists 100% ATK (Ravenna, Lucienne, Kaida, Sable, Sylwen, Ophira, Nyx, Nerissa, Rhiannon); casters 60% (Ilyra, Ysra, Astraea, Zaria, Halcyra, Isaura, Maelis, Caelith); everyone else 75%. Dominant 5 → 2.

**Distinction check (two seeds):** about 85% of the difference between heroes is overall strength (agrees between runs, r = 0.97) and about 15% is real situational swing (shape agrees only r = 0.40). Best pick per situation: tanks Cassia 11/13, healers Aurelle 11/13; damage dealers and supports have 5 different best picks each. Most situational: Ilyra (30-point swing), Ravenna (24), Lucienne (22), Caelith (20). Flattest: Odette, Thessaly, Mireille, Corvina, Lunaith, Noctelle.

_234000 battles in 255s; 150 paired samples per hero per situation. Noise (one standard error): about ±4 points per cell, ±1.0 for a hero's overall uplift._

## Flags

- **Cassia** (high tank): DOMINANT. Overall +13 points; above her peers in 13 of 13 situations; best random, worst full package.
- **Calypso** (ocean dps): DOMINANT. Overall +10 points; above her peers in 11 of 13 situations; best vs ocean, worst vs dark.
- **Odette** (ocean healer): NO NICHE. Overall -3 points; above her peers in 0 of 13 situations; best vs tanky, worst vs dark.
- **Pelagia** (ocean support): NO NICHE. Overall -4 points; above her peers in 0 of 13 situations; best vs arcane, worst own race.
- **Runa** (arcane tank): NO NICHE. Overall -4 points; above her peers in 0 of 13 situations; best own race, worst vs nature.
- **Halcyra** (ocean dps): NO NICHE. Overall -5 points; above her peers in 0 of 13 situations; best vs nature, worst vs control.
- **Amarante** (high support): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best own race, worst vs burst.
- **Thalassa** (ocean tank): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs tanky, worst own race.
- **Rosalind** (high support): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs ocean, worst vs sustain.
- **Seren** (arcane support): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs ocean, worst vs arcane.
- **Noctelle** (dark support): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs ocean, worst with partner.
- **Isolde** (high tank): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best full package, worst vs ocean.
- **Briar** (nature tank): NO NICHE. Overall -8 points; above her peers in 0 of 13 situations; best full package, worst vs burst.
- **Elara** (high healer): NO NICHE. Overall -9 points; above her peers in 0 of 13 situations; best vs sustain, worst vs arcane.
- **Wren** (nature support): NO NICHE. Overall -11 points; above her peers in 0 of 13 situations; best vs high, worst vs tanky.
- **Venna** (nature dps): NO NICHE. Overall -12 points; above her peers in 0 of 13 situations; best vs dark, worst full package.
- **Vesper** (dark healer): NO NICHE. Overall -12 points; above her peers in 0 of 13 situations; best vs control, worst vs arcane.
- **Nyx** (dark dps): NO NICHE. Overall -13 points; above her peers in 0 of 13 situations; best vs arcane, worst own race.

## Tanks

| Hero | Race | Uplift overall | Best situation | Worst situation | Damage soaked share / survives | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Cassia | high | +13 | random (+20) | full package (+7) | 34% / 56% | DOMINANT |
| Valeria | high | +7 | vs high (+13) | with partner (+2) | 31% / 55% | niche |
| Sangrael | dark | +6 | vs burst (+15) | vs sustain (-2) | 32% / 54% | niche |
| Coralie | ocean | +6 | vs nature (+10) | vs control (+2) | 31% / 58% | niche |
| Vaela | arcane | +3 | vs arcane (+10) | vs dark (-4) | 32% / 53% | niche |
| Eldrith | nature | 0 | vs nature (+6) | random (-8) | 42% / 53% | niche |
| Draxa | arcane | -1 | vs control (+5) | full package (-6) | 35% / 52% | niche |
| Corvina | dark | -3 | vs nature (+4) | vs ocean (-14) | 42% / 51% | niche |
| Hartwen | nature | -4 | full package (+5) | random (-11) | 33% / 45% | niche |
| Runa | arcane | -4 | own race (-1) | vs nature (-9) | 28% / 45% | NO NICHE |
| Thalassa | ocean | -7 | vs tanky (+1) | own race (-13) | 27% / 41% | NO NICHE |
| Isolde | high | -7 | full package (-2) | vs ocean (-13) | 27% / 43% | NO NICHE |
| Briar | nature | -8 | full package (0) | vs burst (-21) | 25% / 41% | NO NICHE |

## Dpss

| Hero | Race | Uplift overall | Best situation | Worst situation | Team damage share | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Calypso | ocean | +10 | vs ocean (+23) | vs dark (+1) | 30% | DOMINANT |
| Solenne | high | +10 | vs dark (+20) | random (+2) | 27% | niche |
| Ilyra | high | +10 | vs nature (+26) | vs arcane (-7) | 43% | niche |
| Ravenna | dark | +9 | vs sustain (+22) | vs burst (-2) | 43% | niche |
| Ophira | high | +8 | vs ocean (+20) | full package (-1) | 28% | niche |
| Ysra | ocean | +3 | vs arcane (+9) | vs nature (-7) | 31% | niche |
| Maelis | dark | +3 | vs burst (+11) | vs arcane (-4) | 31% | niche |
| Astraea | arcane | +2 | vs high (+9) | vs ocean (-9) | 35% | niche |
| Zaria | arcane | +1 | vs high (+11) | vs tanky (-7) | 38% | niche |
| Sylwen | nature | +1 | own race (+9) | vs dark (-7) | 33% | niche |
| Rhiannon | nature | 0 | own race (+10) | vs nature (-16) | 38% | niche |
| Isaura | arcane | 0 | vs high (+7) | vs nature (-8) | 30% | niche |
| Sable | dark | -1 | vs control (+5) | vs nature (-9) | 31% | niche |
| Caelith | arcane | -2 | vs high (+8) | own race (-18) | 24% | niche |
| Halcyra | ocean | -5 | vs nature (+2) | vs control (-10) | 35% | NO NICHE |
| Kaida | nature | -7 | vs nature (+3) | vs dark (-19) | 30% | niche |
| Nerissa | ocean | -8 | vs arcane (+2) | with partner (-15) | 24% | niche |
| Lucienne | high | -9 | vs control (+3) | with partner (-17) | 31% | niche |
| Venna | nature | -12 | vs dark (-1) | full package (-26) | 29% | NO NICHE |
| Nyx | dark | -13 | vs arcane (+1) | own race (-23) | 21% | NO NICHE |

## Healers

| Hero | Race | Uplift overall | Best situation | Worst situation | Heal / clutch heal per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Aurelle | high | +9 | own race (+15) | vs ocean (0) | 5545 / 1001 | niche |
| Siora | ocean | +9 | vs high (+16) | vs ocean (+3) | 4463 / 716 | niche |
| Mireille | nature | +5 | vs sustain (+13) | vs dark (-3) | 5712 / 1349 | niche |
| Lunaith | arcane | +4 | vs tanky (+10) | vs nature (-4) | 3911 / 758 | niche |
| Selene | arcane | +4 | vs high (+8) | vs sustain (-2) | 3604 / 1520 | niche |
| Nimue | nature | +3 | vs ocean (+11) | full package (-6) | 1888 / 648 | niche |
| Thessaly | dark | +1 | own race (+4) | vs burst (-4) | 3542 / 789 | niche |
| Mordessa | dark | -3 | own race (+8) | vs arcane (-12) | 3041 / 613 | niche |
| Odette | ocean | -3 | vs tanky (0) | vs dark (-7) | 912 / 255 | NO NICHE |
| Marisol | ocean | -4 | full package (+3) | vs nature (-11) | 3707 / 712 | niche |
| Elara | high | -9 | vs sustain (-5) | vs arcane (-14) | 2821 / 588 | NO NICHE |
| Vesper | dark | -12 | vs control (-1) | vs arcane (-20) | 1780 / 401 | NO NICHE |

## Supports

| Hero | Race | Uplift overall | Best situation | Worst situation | Effects applied per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Elowen | arcane | +9 | vs nature (+16) | full package (-5) | 15.0 | niche |
| Fenna | nature | +9 | random (+14) | vs tanky (+3) | 18.4 | niche |
| Lorelei | ocean | +7 | vs tanky (+11) | vs arcane (+2) | 3.6 | niche |
| Seravelle | high | +6 | with partner (+11) | vs arcane (-3) | 8.4 | niche |
| Liora | dark | +6 | vs burst (+11) | vs arcane (-3) | 11.6 | niche |
| Tempra | arcane | +4 | vs nature (+13) | vs arcane (-3) | 2.3 | niche |
| Ondine | ocean | +2 | vs dark (+11) | vs nature (-7) | 9.4 | niche |
| Velisande | dark | +1 | with partner (+5) | own race (-4) | 16.5 | niche |
| Pelagia | ocean | -4 | vs arcane (+2) | own race (-15) | 11.3 | NO NICHE |
| Zephyra | nature | -5 | own race (+3) | vs dark (-15) | 8.0 | niche |
| Amarante | high | -7 | own race (+1) | vs burst (-15) | 32.6 | NO NICHE |
| Rosalind | high | -7 | vs ocean (-3) | vs sustain (-13) | 11.7 | NO NICHE |
| Seren | arcane | -7 | vs ocean (0) | vs arcane (-13) | 12.7 | NO NICHE |
| Noctelle | dark | -7 | vs ocean (-1) | with partner (-18) | 16.7 | NO NICHE |
| Wren | nature | -11 | vs high (-2) | vs tanky (-17) | 5.8 | NO NICHE |

## Uplift by situation (points versus a random same-role replacement)

| Hero | random | vs high | vs dark | vs nature | vs ocean | vs arcane | vs tanky | vs sustain | vs burst | vs control | with partner | full package | own race |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Astraea | +5 | +9 | +2 | +7 | -9 | +7 | -4 | +1 | -1 | 0 | +1 | +2 | +6 |
| Caelith | -2 | +8 | -2 | 0 | -8 | -7 | +1 | +5 | +2 | -1 | -1 | +3 | -18 |
| Isaura | -3 | +7 | +1 | -8 | -7 | 0 | +6 | -3 | +6 | -3 | +3 | +1 | -2 |
| Zaria | -1 | +11 | +10 | 0 | -2 | -5 | -7 | -1 | +3 | +3 | 0 | +5 | +2 |
| Lunaith | +5 | +2 | -1 | -4 | +7 | +5 | +10 | 0 | +6 | +4 | +5 | +8 | +5 |
| Selene | +4 | +8 | +4 | +6 | -1 | +7 | 0 | -2 | +7 | +8 | +1 | +6 | +1 |
| Elowen | +8 | +7 | +8 | +16 | +12 | +15 | +11 | +11 | +9 | +4 | +6 | -5 | +14 |
| Seren | -2 | -1 | -11 | -12 | 0 | -13 | -9 | -3 | -10 | -11 | -8 | -8 | -5 |
| Tempra | +4 | +5 | +2 | +13 | +2 | -3 | +2 | +1 | +10 | +1 | +5 | +8 | +1 |
| Draxa | 0 | -5 | +1 | -4 | -3 | +2 | -2 | 0 | +1 | +5 | +4 | -6 | -1 |
| Runa | -5 | -2 | -8 | -9 | -2 | -2 | -4 | -6 | -3 | -8 | -5 | -3 | -1 |
| Vaela | +1 | +4 | -4 | +7 | -3 | +10 | +1 | +5 | +2 | +3 | +2 | -1 | +9 |
| Maelis | -1 | -2 | +5 | +3 | +6 | -4 | +3 | +3 | +11 | +8 | +3 | +4 | +3 |
| Nyx | -13 | -22 | -18 | -13 | -14 | +1 | -8 | -14 | -5 | -18 | -9 | -8 | -23 |
| Ravenna | +13 | +8 | +14 | +7 | +5 | +14 | -1 | +22 | -2 | +5 | +8 | +13 | +12 |
| Sable | +3 | -2 | -1 | -9 | +4 | 0 | -8 | -4 | +1 | +5 | 0 | 0 | -2 |
| Mordessa | -5 | -8 | -3 | -1 | 0 | -12 | -9 | -5 | -3 | -7 | +3 | +5 | +8 |
| Thessaly | +3 | -1 | +3 | +2 | -1 | -1 | -1 | +3 | -4 | 0 | +1 | +3 | +4 |
| Vesper | -11 | -8 | -13 | -15 | -18 | -20 | -16 | -18 | -5 | -1 | -16 | -12 | -9 |
| Liora | +6 | +4 | +5 | +3 | +5 | -3 | +10 | +4 | +11 | +9 | +8 | +11 | 0 |
| Noctelle | -12 | -4 | -9 | -5 | -1 | -7 | -2 | -10 | -6 | -8 | -18 | -3 | -10 |
| Velisande | -1 | +4 | -3 | +5 | -2 | +3 | +5 | -1 | +3 | -3 | +5 | +5 | -4 |
| Corvina | -3 | -2 | +1 | +4 | -14 | -3 | -3 | -5 | -9 | -7 | +4 | +1 | -4 |
| Sangrael | +9 | +7 | +6 | +7 | +5 | +4 | +8 | -2 | +15 | +8 | +1 | +4 | +11 |
| Ilyra | +5 | +9 | +21 | +26 | +2 | -7 | +8 | +12 | +2 | +5 | +15 | +17 | +8 |
| Lucienne | -13 | -3 | -6 | -16 | -1 | -15 | -11 | -11 | -6 | +3 | -17 | -11 | -8 |
| Ophira | +12 | +11 | +8 | +2 | +20 | +14 | +9 | +3 | +11 | +2 | +4 | -1 | +8 |
| Solenne | +2 | +10 | +20 | +13 | +6 | +5 | +12 | +12 | +9 | +5 | +6 | +14 | +11 |
| Aurelle | +11 | +11 | +9 | +12 | 0 | +14 | +14 | +9 | +8 | +8 | +6 | +7 | +15 |
| Elara | -6 | -13 | -7 | -11 | -12 | -14 | -6 | -5 | -14 | -6 | -7 | -8 | -10 |
| Amarante | -1 | -3 | -5 | -3 | -15 | -4 | -12 | -4 | -15 | -8 | -4 | -12 | +1 |
| Rosalind | -5 | -6 | -6 | -9 | -3 | -11 | -9 | -13 | -6 | -3 | -7 | -4 | -8 |
| Seravelle | +11 | +3 | +5 | +6 | +11 | -3 | +8 | +3 | +7 | +10 | +11 | +3 | +10 |
| Cassia | +20 | +20 | +11 | +10 | +18 | +16 | +10 | +10 | +9 | +17 | +11 | +7 | +9 |
| Isolde | -3 | -3 | -9 | -10 | -13 | -3 | -9 | -13 | -9 | -2 | -7 | -2 | -12 |
| Valeria | +5 | +13 | +10 | +3 | +4 | +10 | +6 | +11 | +11 | +9 | +2 | +3 | +9 |
| Kaida | -10 | -4 | -19 | +3 | -12 | -4 | -10 | -4 | -7 | -10 | -12 | -3 | -5 |
| Rhiannon | -6 | +3 | -12 | -16 | 0 | +8 | 0 | -4 | +4 | +9 | -3 | +9 | +10 |
| Sylwen | +5 | +7 | -7 | +6 | 0 | -2 | +2 | +3 | -6 | -2 | 0 | +1 | +9 |
| Venna | -12 | -20 | -1 | -5 | -8 | -13 | -5 | -14 | -19 | -2 | -19 | -26 | -7 |
| Mireille | -1 | 0 | -3 | +11 | +3 | +12 | +5 | +13 | +3 | +2 | +6 | +5 | +2 |
| Nimue | +4 | +5 | 0 | +1 | +11 | +6 | -4 | +5 | +9 | +3 | +9 | -6 | -5 |
| Fenna | +14 | +10 | +5 | +11 | +10 | +11 | +3 | +7 | +9 | +7 | +6 | +10 | +11 |
| Wren | -11 | -2 | -15 | -9 | -17 | -14 | -17 | -11 | -8 | -5 | -15 | -5 | -12 |
| Zephyra | -4 | -4 | -15 | -9 | -8 | -11 | -3 | -6 | -6 | +2 | -2 | 0 | +3 |
| Briar | -7 | -4 | -10 | -8 | -9 | -6 | -6 | -9 | -21 | -8 | -14 | 0 | -5 |
| Eldrith | -8 | 0 | 0 | +6 | -5 | +2 | -2 | +1 | -1 | -1 | +2 | -3 | +5 |
| Hartwen | -11 | -6 | -1 | -11 | -5 | -5 | -2 | -6 | 0 | -2 | -4 | +5 | +3 |
| Calypso | +8 | +1 | +1 | +7 | +23 | +17 | +13 | +5 | +15 | +5 | +19 | +14 | +8 |
| Halcyra | -2 | -5 | -1 | +2 | +1 | -2 | -6 | -9 | -6 | -10 | -9 | -5 | -8 |
| Nerissa | -13 | -7 | -8 | -9 | -10 | +2 | -4 | -13 | -10 | -3 | -15 | -3 | -9 |
| Ysra | +6 | +5 | +7 | -7 | +6 | +9 | +4 | +3 | +7 | +4 | 0 | -2 | +3 |
| Marisol | -4 | -2 | -8 | -11 | -5 | +2 | -2 | -4 | -5 | -5 | -3 | +3 | -2 |
| Odette | -4 | -1 | -7 | -7 | -5 | -5 | 0 | -4 | -1 | -1 | -2 | 0 | -4 |
| Siora | +4 | +16 | +9 | +8 | +3 | +8 | +11 | +14 | +7 | +9 | +7 | +11 | +8 |
| Lorelei | +6 | +6 | +8 | +8 | +9 | +2 | +11 | +4 | +3 | +8 | +8 | +9 | +8 |
| Ondine | -3 | -2 | +11 | -7 | +2 | +1 | +10 | 0 | +4 | +2 | +4 | +10 | -3 |
| Pelagia | -5 | -6 | -4 | -11 | -3 | +2 | 0 | 0 | -2 | +1 | -3 | -1 | -15 |
| Coralie | +6 | +4 | +4 | +10 | +10 | +6 | +9 | +4 | +6 | +2 | +7 | +5 | +10 |
| Thalassa | -13 | -12 | -8 | -3 | -3 | -6 | +1 | -7 | -5 | -4 | -9 | -7 | -13 |

## Races

| Race | Win % of its heroes in single-race teams | Uplift of its heroes vs each race (high / dark / nature / ocean / arcane) |
| --- | --- | --- |
| high | +3 uplift | +4 / +4 / +2 / +1 / 0 |
| dark | -1 uplift | -2 / -1 / -1 / -2 / -2 |
| nature | +1 uplift | -1 / -6 / -2 / -3 / -1 |
| ocean | -1 uplift | 0 / 0 / -2 / +2 / +3 |
| arcane | +1 uplift | +4 / 0 / +1 / -1 / +1 |
