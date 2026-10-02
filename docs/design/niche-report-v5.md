# Niche report v5 (after roster pass 8, 2026-10-03)

Balance target (decisions row 29): every hero is the best pick somewhere; no hero is the best pick everywhere. Uplift = points a hero adds to her team's result versus a random same-role replacement, same opponent and seed. DOMINANT = overall ≥ +10 and above peers in ≥ 11 of 13 situations. NO NICHE = never more than +2 above peers in any situation. Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.NicheReport 150 1`.

Pass 8 effect (v4 → v5): Solenne +20 → +8, Caelith −17 → +3, Maelis −7 → +9, Venna −15 → −8, Isaura +5. Globals: auto attacks 75% ATK, cast pause 0.1 s, HP x1.2 (fights 30.6 s, 5.7% timeouts). Cassia rose to +14.

_234000 battles in 261s; 150 paired samples per hero per situation. Noise (one standard error): about ±4 points per cell, ±1.0 for a hero's overall uplift._

## Flags

- **Cassia** (high tank): DOMINANT. Overall +14 points; above her peers in 13 of 13 situations; best vs ocean, worst own race.
- **Calypso** (ocean dps): DOMINANT. Overall +12 points; above her peers in 12 of 13 situations; best vs ocean, worst vs sustain.
- **Aurelle** (high healer): DOMINANT. Overall +11 points; above her peers in 13 of 13 situations; best own race, worst full package.
- **Ilyra** (high dps): DOMINANT. Overall +11 points; above her peers in 11 of 13 situations; best vs nature, worst vs arcane.
- **Ysra** (ocean dps): DOMINANT. Overall +11 points; above her peers in 13 of 13 situations; best vs arcane, worst vs nature.
- **Odette** (ocean healer): NO NICHE. Overall -4 points; above her peers in 0 of 13 situations; best full package, worst vs sustain.
- **Mordessa** (dark healer): NO NICHE. Overall -5 points; above her peers in 0 of 13 situations; best vs dark, worst vs arcane.
- **Amarante** (high support): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best vs nature, worst vs ocean.
- **Runa** (arcane tank): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best full package, worst vs nature.
- **Rosalind** (high support): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs ocean, worst vs dark.
- **Thalassa** (ocean tank): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs nature, worst vs burst.
- **Seren** (arcane support): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs high, worst full package.
- **Isolde** (high tank): NO NICHE. Overall -8 points; above her peers in 0 of 13 situations; best vs control, worst own race.
- **Briar** (nature tank): NO NICHE. Overall -8 points; above her peers in 0 of 13 situations; best random, worst vs burst.
- **Venna** (nature dps): NO NICHE. Overall -8 points; above her peers in 0 of 13 situations; best own race, worst full package.
- **Nerissa** (ocean dps): NO NICHE. Overall -10 points; above her peers in 0 of 13 situations; best vs arcane, worst with partner.
- **Elara** (high healer): NO NICHE. Overall -10 points; above her peers in 0 of 13 situations; best vs sustain, worst own race.
- **Wren** (nature support): NO NICHE. Overall -10 points; above her peers in 0 of 13 situations; best vs high, worst vs tanky.
- **Kaida** (nature dps): NO NICHE. Overall -11 points; above her peers in 0 of 13 situations; best vs nature, worst vs dark.
- **Lucienne** (high dps): NO NICHE. Overall -12 points; above her peers in 0 of 13 situations; best vs control, worst vs tanky.
- **Vesper** (dark healer): NO NICHE. Overall -13 points; above her peers in 0 of 13 situations; best vs control, worst with partner.
- **Nyx** (dark dps): NO NICHE. Overall -17 points; above her peers in 0 of 13 situations; best with partner, worst vs sustain.

## Tanks

| Hero | Race | Uplift overall | Best situation | Worst situation | Damage soaked share / survives | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Cassia | high | +14 | vs ocean (+24) | own race (+7) | 35% / 57% | DOMINANT |
| Valeria | high | +7 | vs high (+14) | vs control (+1) | 31% / 56% | niche |
| Coralie | ocean | +7 | vs nature (+14) | vs dark (+1) | 31% / 59% | niche |
| Sangrael | dark | +6 | vs burst (+15) | vs sustain (-2) | 32% / 54% | niche |
| Vaela | arcane | +2 | own race (+15) | vs dark (-10) | 32% / 55% | niche |
| Draxa | arcane | 0 | vs burst (+6) | vs ocean (-9) | 35% / 52% | niche |
| Eldrith | nature | -1 | vs nature (+5) | random (-8) | 43% / 53% | niche |
| Corvina | dark | -2 | vs nature (+4) | vs burst (-10) | 42% / 53% | niche |
| Hartwen | nature | -5 | full package (+6) | vs arcane (-12) | 34% / 46% | niche |
| Runa | arcane | -7 | full package (-3) | vs nature (-10) | 28% / 44% | NO NICHE |
| Thalassa | ocean | -7 | vs nature (-2) | vs burst (-11) | 27% / 42% | NO NICHE |
| Isolde | high | -8 | vs control (0) | own race (-15) | 27% / 44% | NO NICHE |
| Briar | nature | -8 | random (-3) | vs burst (-16) | 25% / 40% | NO NICHE |

## Dpss

| Hero | Race | Uplift overall | Best situation | Worst situation | Team damage share | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Calypso | ocean | +12 | vs ocean (+26) | vs sustain (-1) | 30% | DOMINANT |
| Ilyra | high | +11 | vs nature (+27) | vs arcane (-3) | 44% | DOMINANT |
| Ysra | ocean | +11 | vs arcane (+21) | vs nature (+3) | 33% | DOMINANT |
| Maelis | dark | +9 | vs burst (+19) | vs ocean (+3) | 33% | niche |
| Solenne | high | +8 | vs dark (+16) | vs burst (+1) | 27% | niche |
| Isaura | arcane | +5 | with partner (+17) | vs control (-1) | 33% | niche |
| Astraea | arcane | +4 | vs nature (+14) | vs tanky (-7) | 36% | niche |
| Ravenna | dark | +4 | vs sustain (+13) | vs burst (-6) | 41% | niche |
| Caelith | arcane | +3 | vs high (+10) | own race (-18) | 27% | niche |
| Zaria | arcane | +3 | vs high (+12) | vs tanky (-4) | 39% | niche |
| Ophira | high | +2 | vs ocean (+14) | full package (-6) | 25% | niche |
| Sylwen | nature | -2 | own race (+3) | vs dark (-8) | 31% | niche |
| Halcyra | ocean | -2 | vs arcane (+5) | vs control (-8) | 36% | niche |
| Sable | dark | -4 | with partner (+4) | vs nature (-10) | 29% | niche |
| Rhiannon | nature | -4 | vs control (+7) | vs dark (-16) | 36% | niche |
| Venna | nature | -8 | own race (+1) | full package (-28) | 30% | NO NICHE |
| Nerissa | ocean | -10 | vs arcane (-2) | with partner (-18) | 21% | NO NICHE |
| Kaida | nature | -11 | vs nature (-2) | vs dark (-25) | 28% | NO NICHE |
| Lucienne | high | -12 | vs control (+2) | vs tanky (-19) | 28% | NO NICHE |
| Nyx | dark | -17 | with partner (-8) | vs sustain (-28) | 18% | NO NICHE |

## Healers

| Hero | Race | Uplift overall | Best situation | Worst situation | Heal / clutch heal per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Aurelle | high | +11 | own race (+18) | full package (+6) | 5701 / 984 | DOMINANT |
| Siora | ocean | +8 | vs sustain (+12) | own race (+1) | 4577 / 709 | niche |
| Mireille | nature | +6 | vs nature (+14) | vs control (+2) | 5927 / 1381 | niche |
| Selene | arcane | +3 | vs dark (+10) | vs tanky (-2) | 3591 / 1486 | niche |
| Lunaith | arcane | +3 | with partner (+7) | vs nature (-1) | 4131 / 773 | niche |
| Nimue | nature | +1 | vs high (+8) | own race (-8) | 1954 / 669 | niche |
| Thessaly | dark | +1 | with partner (+7) | vs high (-3) | 3713 / 774 | niche |
| Marisol | ocean | -3 | vs sustain (+4) | vs ocean (-10) | 3873 / 715 | niche |
| Odette | ocean | -4 | full package (0) | vs sustain (-9) | 946 / 263 | NO NICHE |
| Mordessa | dark | -5 | vs dark (-1) | vs arcane (-13) | 3103 / 639 | NO NICHE |
| Elara | high | -10 | vs sustain (-5) | own race (-21) | 2929 / 577 | NO NICHE |
| Vesper | dark | -13 | vs control (-6) | with partner (-18) | 1880 / 401 | NO NICHE |

## Supports

| Hero | Race | Uplift overall | Best situation | Worst situation | Effects applied per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Elowen | arcane | +9 | vs nature (+18) | full package (-1) | 15.1 | niche |
| Fenna | nature | +9 | full package (+19) | vs tanky (+4) | 19.3 | niche |
| Lorelei | ocean | +7 | vs dark (+14) | vs nature (-4) | 3.8 | niche |
| Seravelle | high | +7 | vs ocean (+13) | vs arcane (-2) | 8.9 | niche |
| Liora | dark | +6 | vs tanky (+14) | vs ocean (0) | 11.6 | niche |
| Tempra | arcane | +6 | vs burst (+14) | vs tanky (0) | 2.4 | niche |
| Ondine | ocean | +3 | full package (+12) | own race (-7) | 9.9 | niche |
| Velisande | dark | +2 | vs tanky (+9) | vs ocean (-5) | 17.2 | niche |
| Zephyra | nature | -3 | vs control (+8) | vs dark (-10) | 8.2 | niche |
| Pelagia | ocean | -4 | vs arcane (+8) | own race (-16) | 11.6 | niche |
| Amarante | high | -6 | vs nature (+1) | vs ocean (-12) | 33.5 | NO NICHE |
| Noctelle | dark | -7 | vs ocean (+3) | with partner (-16) | 17.3 | niche |
| Rosalind | high | -7 | vs ocean (-3) | vs dark (-10) | 12.2 | NO NICHE |
| Seren | arcane | -7 | vs high (-2) | full package (-13) | 13.4 | NO NICHE |
| Wren | nature | -10 | vs high (-2) | vs tanky (-18) | 6.0 | NO NICHE |

## Uplift by situation (points versus a random same-role replacement)

| Hero | random | vs high | vs dark | vs nature | vs ocean | vs arcane | vs tanky | vs sustain | vs burst | vs control | with partner | full package | own race |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Astraea | +8 | +11 | +4 | +14 | -5 | +3 | -7 | +12 | -1 | +3 | +5 | +4 | 0 |
| Caelith | 0 | +10 | +3 | +3 | 0 | +5 | +9 | +4 | +7 | -1 | +10 | +6 | -18 |
| Isaura | +6 | +13 | +9 | 0 | +1 | +3 | +2 | +1 | +10 | -1 | +17 | +1 | +4 |
| Zaria | +7 | +12 | +8 | +8 | -2 | -1 | -4 | +9 | +3 | +2 | -3 | +2 | -4 |
| Lunaith | +7 | -1 | +6 | -1 | +1 | +5 | +6 | 0 | -1 | -1 | +7 | +4 | +1 |
| Selene | +2 | +7 | +10 | +1 | 0 | +8 | -2 | 0 | -2 | +8 | +5 | -1 | -1 |
| Elowen | +5 | +8 | +8 | +18 | +13 | +15 | +14 | +10 | +7 | +4 | +2 | -1 | +12 |
| Seren | -7 | -2 | -6 | -13 | -6 | -5 | -5 | -7 | -5 | -5 | -13 | -13 | -11 |
| Tempra | +9 | +3 | +9 | +2 | +2 | +3 | 0 | +8 | +14 | +6 | +5 | +8 | +4 |
| Draxa | -3 | -2 | -1 | +5 | -9 | +1 | +3 | 0 | +6 | +6 | 0 | +1 | -1 |
| Runa | -8 | -3 | -8 | -10 | -5 | -8 | -7 | -3 | -7 | -5 | -9 | -3 | -9 |
| Vaela | +4 | -3 | -10 | +8 | 0 | +10 | -2 | +5 | -2 | -1 | +1 | +1 | +15 |
| Maelis | +10 | +8 | +9 | +10 | +3 | +7 | +6 | +5 | +19 | +11 | +4 | +4 | +16 |
| Nyx | -16 | -25 | -18 | -15 | -10 | -12 | -12 | -28 | -19 | -20 | -8 | -14 | -20 |
| Ravenna | +5 | -1 | +2 | +7 | -2 | +4 | -2 | +13 | -6 | +6 | 0 | +11 | +11 |
| Sable | -2 | -8 | -5 | -10 | 0 | -5 | -10 | 0 | -3 | +3 | +4 | -2 | -8 |
| Mordessa | -2 | -5 | -1 | -4 | -1 | -13 | -4 | -6 | -8 | -4 | -3 | -4 | -5 |
| Thessaly | -1 | -3 | -3 | -1 | +4 | +2 | +2 | -1 | -3 | +1 | +7 | +2 | +3 |
| Vesper | -8 | -11 | -16 | -10 | -16 | -15 | -16 | -16 | -11 | -6 | -18 | -18 | -6 |
| Liora | +6 | +2 | +6 | +5 | 0 | +5 | +14 | +3 | +13 | +4 | +5 | +13 | +4 |
| Noctelle | -10 | -5 | -7 | -8 | +3 | -6 | -2 | -8 | 0 | -4 | -16 | -7 | -15 |
| Velisande | +3 | +3 | -3 | +5 | -5 | +5 | +9 | +5 | -5 | 0 | +6 | +5 | -2 |
| Corvina | 0 | 0 | -2 | +4 | -5 | -6 | -1 | -3 | -10 | -1 | +3 | -2 | -7 |
| Sangrael | +2 | +10 | +1 | +14 | +8 | +3 | +4 | -2 | +15 | +5 | +10 | +2 | +8 |
| Ilyra | +10 | +13 | +11 | +27 | +6 | -3 | +14 | +10 | +6 | +2 | +17 | +19 | +13 |
| Lucienne | -17 | -7 | -6 | -18 | -9 | -14 | -19 | -12 | -8 | +2 | -15 | -18 | -12 |
| Ophira | +3 | 0 | +4 | -4 | +14 | +1 | +7 | -2 | -1 | +2 | +3 | -6 | +5 |
| Solenne | +7 | +12 | +16 | +10 | +4 | +1 | +16 | +11 | +1 | +6 | +12 | +8 | +6 |
| Aurelle | +11 | +8 | +13 | +11 | +10 | +7 | +13 | +16 | +9 | +10 | +16 | +6 | +18 |
| Elara | -7 | -14 | -8 | -9 | -10 | -9 | -7 | -5 | -13 | -10 | -7 | -7 | -21 |
| Amarante | -4 | -7 | -6 | +1 | -12 | -3 | -11 | -6 | -9 | -3 | -3 | -7 | -3 |
| Rosalind | -6 | -8 | -10 | -5 | -3 | -8 | -7 | -10 | -7 | -6 | -8 | -5 | -5 |
| Seravelle | +9 | +1 | +7 | +11 | +13 | -2 | +11 | +7 | +8 | +6 | +2 | +4 | +11 |
| Cassia | +12 | +18 | +9 | +9 | +24 | +14 | +14 | +15 | +19 | +14 | +13 | +13 | +7 |
| Isolde | -2 | -10 | -6 | -13 | -6 | -6 | -9 | -10 | -12 | 0 | -8 | -2 | -15 |
| Valeria | +11 | +14 | +8 | +5 | +9 | +7 | +6 | +8 | +5 | +1 | +8 | +4 | +7 |
| Kaida | -14 | -14 | -25 | -2 | -9 | -7 | -12 | -10 | -9 | -10 | -14 | -4 | -7 |
| Rhiannon | -6 | -2 | -16 | -15 | +3 | +6 | -11 | -8 | -2 | +7 | -8 | +2 | +2 |
| Sylwen | +1 | +1 | -8 | -2 | 0 | -2 | -3 | -3 | -7 | -2 | -2 | -4 | +3 |
| Venna | -5 | -16 | -7 | -6 | -7 | -11 | -5 | -3 | -6 | -2 | -15 | -28 | +1 |
| Mireille | +5 | +4 | +4 | +14 | +3 | +7 | +9 | +5 | +3 | +2 | +4 | +8 | +8 |
| Nimue | -3 | +8 | -1 | +5 | +6 | -2 | +1 | +5 | +6 | +4 | +3 | -7 | -8 |
| Fenna | +8 | +7 | +5 | +12 | +5 | +12 | +4 | +5 | +10 | +7 | +8 | +19 | +10 |
| Wren | -14 | -2 | -11 | -10 | -11 | -8 | -18 | -11 | -13 | -6 | -7 | -9 | -14 |
| Zephyra | -5 | -5 | -10 | -2 | -5 | -8 | -3 | -6 | -2 | +8 | -1 | -3 | +2 |
| Briar | -3 | -5 | -9 | -6 | -15 | -9 | -3 | -8 | -16 | -3 | -11 | -5 | -7 |
| Eldrith | -8 | -1 | +2 | +5 | -2 | -5 | -8 | +3 | +5 | -5 | -6 | -4 | +2 |
| Hartwen | -9 | -8 | -5 | -9 | -6 | -12 | -7 | -8 | -4 | -1 | -9 | +6 | +6 |
| Calypso | +9 | +4 | +4 | +10 | +26 | +17 | +14 | -1 | +17 | +13 | +14 | +17 | +12 |
| Halcyra | -3 | -6 | +4 | +1 | 0 | +5 | 0 | -6 | -5 | -8 | -8 | +4 | -7 |
| Nerissa | -11 | -14 | -15 | -14 | -6 | -2 | -2 | -16 | -6 | -4 | -18 | -5 | -14 |
| Ysra | +13 | +15 | +10 | +3 | +11 | +21 | +10 | +7 | +17 | +10 | +5 | +6 | +12 |
| Marisol | -8 | +1 | -8 | -7 | -10 | -6 | 0 | +4 | -2 | +1 | +2 | -6 | -7 |
| Odette | -5 | -1 | -5 | -4 | -2 | -7 | -7 | -9 | -2 | -2 | -3 | 0 | 0 |
| Siora | +8 | +8 | +11 | +6 | +12 | +9 | +12 | +12 | +8 | +9 | +7 | +8 | +1 |
| Lorelei | +7 | +8 | +14 | -4 | +13 | 0 | +7 | 0 | +10 | +8 | +9 | +11 | +9 |
| Ondine | -3 | -5 | +8 | -3 | +3 | 0 | +7 | +1 | +10 | +6 | +5 | +12 | -7 |
| Pelagia | -6 | 0 | -7 | -6 | -11 | +8 | -10 | +5 | -6 | -1 | +1 | +2 | -16 |
| Coralie | +2 | +8 | +1 | +14 | +10 | +7 | +8 | +9 | +6 | +2 | +8 | +3 | +12 |
| Thalassa | -8 | -11 | -4 | -2 | -4 | -3 | -4 | -8 | -11 | -8 | -11 | -8 | -10 |

## Races

| Race | Win % of its heroes in single-race teams | Uplift of its heroes vs each race (high / dark / nature / ocean / arcane) |
| --- | --- | --- |
| high | +1 uplift | +2 / +3 / +2 / +3 / -1 |
| dark | -2 uplift | -3 / -3 / 0 / -2 / -3 |
| nature | 0 uplift | -3 / -7 / -1 / -3 / -3 |
| ocean | -1 uplift | +1 / +1 / -1 / +3 / +4 |
| arcane | -1 uplift | +4 / +3 / +3 / -1 / +3 |
