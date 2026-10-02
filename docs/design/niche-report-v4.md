# Niche report v4 (after roster pass 7, 2026-10-02)

Balance target (decisions row 29): every hero is the best pick somewhere; no hero is the best pick everywhere. Uplift = points a hero adds to her team's result versus a random same-role replacement, same opponent and seed. DOMINANT = overall ≥ +10 and above peers in ≥ 11 of 13 situations. NO NICHE = never more than +2 above peers in any situation. Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.NicheReport 150 1`.

Pass 7 effect (v3 → v4): Isaura +23 → +5, Cassia +11 → +9, Coralie +11 → +9, Ilyra +16 → +13, Solenne +20 → +20 (her ultimate is not her strength), Maelis −11 → −7, Nyx −17 → −14, Venna −19 → −15, Caelith −23 → −17, Vesper −12 → −12 (Feeding Curse is not her problem). Aurelle rose to +12 as the meta shifted.

_234000 battles in 286s; 150 paired samples per hero per situation. Noise (one standard error): about ±4 points per cell, ±1.0 for a hero's overall uplift._

## Flags

- **Solenne** (high dps): DOMINANT. Overall +20 points; above her peers in 13 of 13 situations; best vs dark, worst vs control.
- **Ilyra** (high dps): DOMINANT. Overall +13 points; above her peers in 11 of 13 situations; best vs dark, worst vs arcane.
- **Aurelle** (high healer): DOMINANT. Overall +12 points; above her peers in 13 of 13 situations; best vs tanky, worst vs ocean.
- **Fenna** (nature support): DOMINANT. Overall +11 points; above her peers in 13 of 13 situations; best vs burst, worst vs sustain.
- **Calypso** (ocean dps): DOMINANT. Overall +10 points; above her peers in 11 of 13 situations; best vs arcane, worst own race.
- **Odette** (ocean healer): NO NICHE. Overall -3 points; above her peers in 0 of 13 situations; best vs burst, worst random.
- **Marisol** (ocean healer): NO NICHE. Overall -4 points; above her peers in 0 of 13 situations; best with partner, worst vs ocean.
- **Pelagia** (ocean support): NO NICHE. Overall -5 points; above her peers in 0 of 13 situations; best vs sustain, worst own race.
- **Thalassa** (ocean tank): NO NICHE. Overall -5 points; above her peers in 0 of 13 situations; best random, worst vs burst.
- **Runa** (arcane tank): NO NICHE. Overall -5 points; above her peers in 0 of 13 situations; best full package, worst vs tanky.
- **Tempra** (arcane support): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best vs dark, worst own race.
- **Zephyra** (nature support): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best vs sustain, worst vs arcane.
- **Noctelle** (dark support): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs ocean, worst own race.
- **Rosalind** (high support): NO NICHE. Overall -8 points; above her peers in 0 of 13 situations; best vs control, worst vs tanky.
- **Nerissa** (ocean dps): NO NICHE. Overall -8 points; above her peers in 0 of 13 situations; best vs arcane, worst own race.
- **Lucienne** (high dps): NO NICHE. Overall -9 points; above her peers in 0 of 13 situations; best vs control, worst vs nature.
- **Wren** (nature support): NO NICHE. Overall -9 points; above her peers in 0 of 13 situations; best vs high, worst own race.
- **Briar** (nature tank): NO NICHE. Overall -9 points; above her peers in 0 of 13 situations; best vs high, worst random.
- **Elara** (high healer): NO NICHE. Overall -9 points; above her peers in 0 of 13 situations; best vs dark, worst own race.
- **Isolde** (high tank): NO NICHE. Overall -11 points; above her peers in 0 of 13 situations; best full package, worst vs sustain.
- **Vesper** (dark healer): NO NICHE. Overall -12 points; above her peers in 0 of 13 situations; best vs high, worst vs arcane.
- **Nyx** (dark dps): NO NICHE. Overall -14 points; above her peers in 0 of 13 situations; best vs tanky, worst vs dark.
- **Venna** (nature dps): NO NICHE. Overall -15 points; above her peers in 0 of 13 situations; best vs control, worst full package.
- **Caelith** (arcane dps): NO NICHE. Overall -17 points; above her peers in 0 of 13 situations; best full package, worst vs arcane.

## Tanks

| Hero | Race | Uplift overall | Best situation | Worst situation | Damage soaked share / survives | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Cassia | high | +9 | vs ocean (+23) | full package (-1) | 34% / 53% | niche |
| Coralie | ocean | +9 | own race (+20) | full package (+1) | 32% / 60% | niche |
| Valeria | high | +8 | vs high (+16) | vs sustain (0) | 32% / 54% | niche |
| Sangrael | dark | +5 | full package (+10) | vs sustain (-3) | 32% / 50% | niche |
| Vaela | arcane | +5 | vs arcane (+13) | vs burst (0) | 33% / 55% | niche |
| Eldrith | nature | +2 | vs nature (+12) | vs control (-5) | 44% / 56% | niche |
| Draxa | arcane | 0 | vs dark (+8) | vs ocean (-10) | 36% / 51% | niche |
| Corvina | dark | 0 | vs arcane (+5) | own race (-9) | 44% / 51% | niche |
| Thalassa | ocean | -5 | random (+1) | vs burst (-13) | 27% / 43% | NO NICHE |
| Runa | arcane | -5 | full package (-1) | vs tanky (-9) | 30% / 43% | NO NICHE |
| Hartwen | nature | -6 | vs ocean (+2) | with partner (-13) | 35% / 42% | niche |
| Briar | nature | -9 | vs high (-3) | random (-14) | 26% / 37% | NO NICHE |
| Isolde | high | -11 | full package (-2) | vs sustain (-19) | 27% / 39% | NO NICHE |

## Dpss

| Hero | Race | Uplift overall | Best situation | Worst situation | Team damage share | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Solenne | high | +20 | vs dark (+35) | vs control (+7) | 28% | DOMINANT |
| Ilyra | high | +13 | vs dark (+24) | vs arcane (-3) | 45% | DOMINANT |
| Calypso | ocean | +10 | vs arcane (+22) | own race (0) | 28% | DOMINANT |
| Ysra | ocean | +9 | vs burst (+19) | vs nature (-6) | 33% | niche |
| Ravenna | dark | +7 | own race (+18) | vs tanky (-2) | 40% | niche |
| Ophira | high | +7 | vs burst (+15) | vs nature (0) | 26% | niche |
| Isaura | arcane | +5 | vs high (+13) | vs control (-1) | 32% | niche |
| Zaria | arcane | +5 | vs high (+12) | vs ocean (-3) | 39% | niche |
| Astraea | arcane | +4 | vs high (+16) | vs burst (-7) | 37% | niche |
| Rhiannon | nature | +2 | own race (+10) | vs nature (-10) | 36% | niche |
| Sable | dark | +1 | vs ocean (+8) | vs arcane (-5) | 29% | niche |
| Halcyra | ocean | -1 | vs arcane (+10) | vs sustain (-11) | 37% | niche |
| Sylwen | nature | -1 | random (+7) | vs control (-11) | 31% | niche |
| Kaida | nature | -7 | full package (+4) | vs dark (-20) | 29% | niche |
| Maelis | dark | -7 | vs nature (+2) | vs arcane (-17) | 42% | niche |
| Nerissa | ocean | -8 | vs arcane (-1) | own race (-14) | 21% | NO NICHE |
| Lucienne | high | -9 | vs control (+1) | vs nature (-16) | 29% | NO NICHE |
| Nyx | dark | -14 | vs tanky (-6) | vs dark (-19) | 19% | NO NICHE |
| Venna | nature | -15 | vs control (-2) | full package (-27) | 37% | NO NICHE |
| Caelith | arcane | -17 | full package (-7) | vs arcane (-29) | 32% | NO NICHE |

## Healers

| Hero | Race | Uplift overall | Best situation | Worst situation | Heal / clutch heal per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Aurelle | high | +12 | vs tanky (+16) | vs ocean (+4) | 7397 / 1326 | DOMINANT |
| Siora | ocean | +10 | vs sustain (+15) | vs ocean (+6) | 5536 / 919 | niche |
| Mireille | nature | +10 | vs nature (+17) | vs control (+4) | 8529 / 1948 | niche |
| Selene | arcane | +2 | vs high (+11) | vs tanky (-4) | 4116 / 1871 | niche |
| Thessaly | dark | +1 | with partner (+7) | vs burst (-3) | 4479 / 1079 | niche |
| Lunaith | arcane | 0 | vs arcane (+9) | vs nature (-5) | 5137 / 1118 | niche |
| Nimue | nature | 0 | vs high (+7) | vs dark (-13) | 2393 / 854 | niche |
| Mordessa | dark | -1 | full package (+5) | vs dark (-9) | 4501 / 892 | niche |
| Odette | ocean | -3 | vs burst (+1) | random (-8) | 1003 / 317 | NO NICHE |
| Marisol | ocean | -4 | with partner (+2) | vs ocean (-9) | 4913 / 967 | NO NICHE |
| Elara | high | -9 | vs dark (-5) | own race (-16) | 3582 / 775 | NO NICHE |
| Vesper | dark | -12 | vs high (-5) | vs arcane (-21) | 2187 / 492 | NO NICHE |

## Supports

| Hero | Race | Uplift overall | Best situation | Worst situation | Effects applied per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Fenna | nature | +11 | vs burst (+17) | vs sustain (+5) | 20.3 | DOMINANT |
| Elowen | arcane | +8 | vs dark (+17) | with partner (-2) | 15.1 | niche |
| Seravelle | high | +7 | vs burst (+15) | vs dark (+1) | 9.1 | niche |
| Lorelei | ocean | +6 | vs dark (+11) | vs sustain (+1) | 3.8 | niche |
| Velisande | dark | +5 | vs nature (+13) | vs ocean (-4) | 18.4 | niche |
| Liora | dark | +5 | random (+9) | vs nature (-2) | 13.1 | niche |
| Ondine | ocean | +2 | with partner (+10) | vs arcane (-4) | 10.4 | niche |
| Seren | arcane | -4 | vs dark (+3) | vs arcane (-11) | 13.4 | niche |
| Pelagia | ocean | -5 | vs sustain (+1) | own race (-18) | 12.3 | NO NICHE |
| Tempra | arcane | -6 | vs dark (0) | own race (-19) | 2.4 | NO NICHE |
| Zephyra | nature | -6 | vs sustain (0) | vs arcane (-19) | 8.2 | NO NICHE |
| Amarante | high | -6 | own race (+3) | random (-12) | 35.8 | niche |
| Noctelle | dark | -7 | vs ocean (+2) | own race (-15) | 18.0 | NO NICHE |
| Rosalind | high | -8 | vs control (+1) | vs tanky (-16) | 12.6 | NO NICHE |
| Wren | nature | -9 | vs high (-6) | own race (-16) | 6.4 | NO NICHE |

## Uplift by situation (points versus a random same-role replacement)

| Hero | random | vs high | vs dark | vs nature | vs ocean | vs arcane | vs tanky | vs sustain | vs burst | vs control | with partner | full package | own race |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Astraea | +12 | +16 | +6 | +8 | -6 | -1 | -3 | +9 | -7 | +2 | +1 | +8 | +10 |
| Caelith | -18 | -12 | -18 | -20 | -22 | -29 | -19 | -19 | -14 | -10 | -12 | -7 | -25 |
| Isaura | +8 | +13 | +1 | +7 | 0 | +7 | +6 | +3 | +10 | -1 | +8 | +6 | +4 |
| Zaria | -1 | +12 | +3 | +9 | -3 | +3 | -1 | +6 | +11 | +4 | +7 | +3 | +11 |
| Lunaith | -1 | -1 | -3 | -5 | -1 | +9 | +1 | -4 | 0 | -2 | 0 | +4 | +1 |
| Selene | -2 | +11 | +10 | -2 | +2 | +7 | -4 | -2 | +7 | +4 | +1 | -1 | 0 |
| Elowen | +10 | +10 | +17 | +9 | +9 | +13 | +8 | +7 | +6 | +1 | -2 | +3 | +17 |
| Seren | -4 | -5 | +3 | -7 | +2 | -11 | -2 | -4 | -5 | +1 | -5 | -5 | -7 |
| Tempra | -1 | -4 | 0 | -7 | -5 | -7 | -8 | -6 | -4 | -6 | -3 | -7 | -19 |
| Draxa | -3 | -3 | +8 | -3 | -10 | +2 | +3 | +1 | 0 | 0 | 0 | +6 | 0 |
| Runa | -5 | -7 | -6 | -6 | -4 | -3 | -9 | -7 | -4 | -6 | -7 | -1 | -7 |
| Vaela | +1 | 0 | +4 | +7 | +2 | +13 | +3 | +9 | 0 | +5 | +4 | +3 | +6 |
| Maelis | -11 | -8 | -9 | +2 | -13 | -17 | -3 | -8 | -3 | -6 | -4 | -11 | -3 |
| Nyx | -9 | -15 | -19 | -13 | -10 | -9 | -6 | -18 | -18 | -19 | -14 | -11 | -19 |
| Ravenna | +8 | +3 | +12 | +8 | -2 | +8 | -2 | +16 | +7 | +3 | +5 | +4 | +18 |
| Sable | +6 | +3 | +5 | 0 | +8 | -5 | -4 | +1 | 0 | +2 | +4 | -3 | +1 |
| Mordessa | -4 | 0 | -9 | -2 | +2 | -3 | -4 | 0 | -2 | 0 | +3 | +5 | -1 |
| Thessaly | +4 | 0 | +1 | 0 | +2 | -2 | +1 | -1 | -3 | +1 | +7 | -3 | +4 |
| Vesper | -15 | -5 | -7 | -13 | -13 | -21 | -21 | -18 | -10 | -9 | -8 | -10 | -7 |
| Liora | +9 | +5 | +7 | -2 | +3 | -2 | +9 | +8 | +4 | +1 | +8 | +9 | +4 |
| Noctelle | -8 | +1 | -13 | -9 | +2 | -4 | -1 | -14 | -4 | -10 | -4 | -10 | -15 |
| Velisande | +1 | 0 | +7 | +13 | -4 | +4 | +12 | +2 | +3 | +6 | +9 | +7 | +5 |
| Corvina | +4 | +1 | -5 | -1 | -3 | +5 | +3 | -3 | +4 | 0 | -1 | +1 | -9 |
| Sangrael | +5 | +7 | +3 | +7 | +9 | -1 | +1 | -3 | +8 | +6 | +8 | +10 | +5 |
| Ilyra | +5 | +8 | +24 | +23 | +11 | -3 | +22 | +13 | +12 | +14 | +18 | +21 | +2 |
| Lucienne | -6 | -3 | -14 | -16 | -2 | -7 | -13 | -9 | 0 | +1 | -9 | -16 | -16 |
| Ophira | +11 | 0 | +1 | 0 | +14 | +11 | +11 | +4 | +15 | 0 | +7 | +8 | +4 |
| Solenne | +16 | +19 | +35 | +24 | +15 | +9 | +25 | +25 | +12 | +7 | +28 | +31 | +10 |
| Aurelle | +10 | +12 | +13 | +13 | +4 | +12 | +16 | +16 | +14 | +11 | +15 | +11 | +11 |
| Elara | -6 | -11 | -5 | -6 | -15 | -12 | -7 | -11 | -9 | -7 | -10 | -5 | -16 |
| Amarante | -12 | -3 | -12 | -2 | -8 | -6 | -5 | -8 | -6 | -11 | -5 | -3 | +3 |
| Rosalind | -8 | -9 | -8 | -5 | 0 | -7 | -16 | -12 | -15 | +1 | -6 | -7 | -6 |
| Seravelle | +10 | +6 | +1 | +12 | +6 | +1 | +7 | +6 | +15 | +4 | +11 | +2 | +13 |
| Cassia | +7 | +12 | +4 | +17 | +23 | +8 | +11 | +4 | +10 | +12 | +10 | -1 | +7 |
| Isolde | -8 | -11 | -7 | -17 | -12 | -12 | -9 | -19 | -14 | -14 | -11 | -2 | -13 |
| Valeria | +5 | +16 | +15 | +6 | +11 | +7 | +9 | 0 | +9 | +10 | +5 | +7 | +8 |
| Kaida | -11 | -13 | -20 | -2 | -4 | -6 | -2 | -5 | -9 | -9 | -6 | +4 | -6 |
| Rhiannon | +3 | +1 | -10 | -10 | +3 | +9 | 0 | +5 | +7 | +6 | -3 | +5 | +10 |
| Sylwen | +7 | -2 | -1 | 0 | +3 | -1 | -8 | -1 | -5 | -11 | -5 | 0 | +6 |
| Venna | -18 | -12 | -17 | -10 | -16 | -22 | -14 | -13 | -15 | -2 | -14 | -27 | -10 |
| Mireille | +6 | +5 | +10 | +17 | +15 | +9 | +11 | +8 | +5 | +4 | +7 | +15 | +13 |
| Nimue | 0 | +7 | -13 | +5 | +6 | +2 | -3 | +4 | 0 | 0 | +4 | -9 | -8 |
| Fenna | +7 | +15 | +8 | +14 | +7 | +11 | +14 | +5 | +17 | +5 | +8 | +16 | +12 |
| Wren | -9 | -6 | -7 | -11 | -9 | -8 | -10 | -12 | -8 | -6 | -6 | -6 | -16 |
| Zephyra | -10 | -3 | -5 | -7 | -1 | -19 | -9 | 0 | -5 | -7 | -1 | -5 | -7 |
| Briar | -14 | -3 | -10 | -10 | -6 | -12 | -10 | -7 | -12 | -9 | -10 | -4 | -7 |
| Eldrith | +4 | -1 | -3 | +12 | -4 | +3 | +3 | -2 | -2 | -5 | +1 | +5 | +11 |
| Hartwen | -7 | -9 | -11 | -5 | +2 | -4 | -5 | -1 | -2 | -8 | -13 | -7 | -1 |
| Calypso | +15 | +1 | +6 | +8 | +17 | +22 | +13 | +3 | +14 | +8 | +13 | +16 | 0 |
| Halcyra | -3 | -4 | -5 | +7 | -3 | +10 | -2 | -11 | -4 | -1 | +9 | +5 | -8 |
| Nerissa | -7 | -7 | -13 | -6 | -7 | -1 | -5 | -8 | -5 | -8 | -12 | -8 | -14 |
| Ysra | +5 | +11 | +17 | -6 | +8 | +16 | +18 | +2 | +19 | +10 | +7 | +6 | +10 |
| Marisol | -1 | -5 | -3 | -7 | -9 | -7 | -5 | -4 | -7 | -1 | +2 | -5 | -1 |
| Odette | -8 | -1 | -2 | -6 | -7 | 0 | -4 | -6 | +1 | -1 | -5 | +1 | -7 |
| Siora | +9 | +10 | +7 | +8 | +6 | +14 | +9 | +15 | +13 | +11 | +6 | +10 | +10 |
| Lorelei | +6 | +7 | +11 | +3 | +8 | +6 | +3 | +1 | +11 | +3 | +3 | +10 | +9 |
| Ondine | -3 | +2 | +3 | -4 | -1 | -4 | +8 | +7 | -4 | +3 | +10 | +9 | +4 |
| Pelagia | -10 | -4 | 0 | -1 | -8 | -4 | -7 | +1 | -9 | +1 | -2 | -2 | -18 |
| Coralie | +8 | +6 | +8 | +5 | +15 | +9 | +7 | +5 | +11 | +7 | +13 | +1 | +20 |
| Thalassa | +1 | -11 | -2 | -6 | -8 | -3 | -5 | -6 | -13 | -9 | -1 | 0 | -3 |

## Races

| Race | Win % of its heroes in single-race teams | Uplift of its heroes vs each race (high / dark / nature / ocean / arcane) |
| --- | --- | --- |
| high | 0 uplift | +3 / +4 / +4 / +4 / 0 |
| dark | -1 uplift | -1 / -2 / -1 / -2 / -4 |
| nature | 0 uplift | -2 / -7 / 0 / 0 / -3 |
| ocean | 0 uplift | 0 / +2 / 0 / +1 / +5 |
| arcane | -1 uplift | +3 / +2 / -1 / -3 / 0 |
