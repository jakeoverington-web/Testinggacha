# Ablation report v1 (after roster pass 7, 2026-10-03)

What each part of a kit is worth: paired battles with the full hero vs the same hero with one part switched off. Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.Ablation 300 1` (about 2 min). What-if checks: `tools/csharp/test.sh -Main Gacha.Tests.Tools.Variant <hero> ai=<key>`.

Findings: (1) auto attacks carry most of every kit's value (damage dealers: autos 25 points, ultimate 11, skills 2-5), so kits converge; (2) every cast costs a 0.3 s attack pause, so weak or badly aimed skills are worth less than nothing (Lunaith's Drowse -4.5, Rosalind's Knighting -4.2, Tempra's Stasis -4.2, Liora's Cinder Veil -3.2, Briar's Root Hold -2.5); (3) 'Highest-HP enemy' targeting sends Caelith, Venna and Maelis into tanks: nearest targeting alone is worth +23, +12.5 and +16 points; (4) Solenne's strength is her targeting (highest ATK: worth +7) and her Sunshot mark, which makes her whole team focus the enemy carry, not her ultimate.

_108000 battles; 300 paired samples per hero; noise about ±1.9 points per cell (one standard error)._

Value = points of team result lost when that part is switched off. **Bold** = more than 2.5x noise away from the role average.

## Tanks

| Hero | Ultimate | Skill 1 | Skill 2 | Passive | Auto attacks |
| --- | --- | --- | --- | --- | --- |
| *role average* | *5.9* | *2.3* | *1.5* | *1.4* | *10.7* |
| Valeria | **17.7** | 0.8 | 2.3 | 2.5 | **20.2** |
| Cassia | 3.3 | **9.5** | 2.5 | 1.5 | 16.7 |
| Coralie | 6.5 | 6.2 | 5.0 | 0.7 | 9.7 |
| Sangrael | 3.2 | 7.0 | 7.2 | 2.3 | 8.0 |
| Draxa | **10.5** | **-2.7** | 1.5 | **0.0** | **17.7** |
| Corvina | 4.7 | 3.5 | 1.5 | 2.8 | 13.3 |
| Eldrith | **12.3** | **-2.3** | -2.2 | 3.0 | 13.8 |
| Vaela | 10.2 | 0.8 | 1.2 | 0.7 | 10.8 |
| Hartwen | **1.8** | 2.5 | 0.3 | 3.2 | 10.3 |
| Runa | 3.8 | 1.3 | -0.7 | 0.8 | 9.2 |
| Thalassa | **-0.5** | -0.7 | 3.5 | 1.5 | **2.3** |
| Briar | **2.5** | 2.7 | **-2.5** | -0.3 | **3.7** |
| Isolde | **0.5** | 1.7 | -0.3 | -0.3 | **2.8** |

## Dpss

| Hero | Ultimate | Skill 1 | Skill 2 | Passive | Auto attacks |
| --- | --- | --- | --- | --- | --- |
| *role average* | *11.3* | *4.9* | *2.2* | *3.1* | *25.0* |
| Ravenna | **21.7** | 9.0 | 1.0 | **13.7** | **39.2** |
| Calypso | 15.7 | 6.5 | 5.3 | **12.7** | **33.7** |
| Isaura | **23.0** | 8.5 | 1.0 | 4.2 | **33.7** |
| Ysra | 12.3 | 8.3 | 2.2 | 2.3 | **37.0** |
| Ilyra | 12.2 | **17.5** | 1.2 | 3.5 | 20.3 |
| Ophira | 8.7 | 2.7 | **9.0** | **0.0** | 31.0 |
| Astraea | 10.3 | 6.7 | -1.7 | 4.7 | 31.0 |
| Sable | 11.0 | 7.7 | 4.8 | 1.3 | 24.8 |
| Rhiannon | 9.3 | 5.2 | 3.5 | 7.5 | 22.5 |
| Solenne | **4.8** | 7.3 | 4.7 | **0.3** | 27.8 |
| Maelis | 16.3 | 3.8 | 1.0 | 4.2 | **18.5** |
| Halcyra | 13.7 | 1.2 | -1.0 | 5.5 | 22.2 |
| Zaria | 10.5 | 2.3 | 1.8 | **-1.2** | 27.3 |
| Kaida | 11.0 | 4.8 | -0.7 | 1.5 | 19.8 |
| Sylwen | 7.0 | **-2.3** | 5.2 | **-0.2** | 24.0 |
| Venna | 9.5 | 2.3 | 4.7 | **0.8** | **16.2** |
| Lucienne | **5.8** | 2.7 | 1.7 | **0.3** | 21.0 |
| Caelith | 10.8 | 3.3 | 1.7 | 0.8 | **12.3** |
| Nyx | 7.3 | **-0.8** | -1.2 | **-0.7** | 21.0 |
| Nerissa | **4.3** | 1.3 | 0.2 | 0.7 | **17.2** |

## Healers

| Hero | Ultimate | Skill 1 | Skill 2 | Passive | Auto attacks |
| --- | --- | --- | --- | --- | --- |
| *role average* | *13.0* | *9.8* | *2.8* | *2.2* | *18.8* |
| Siora | **20.2** | 14.8 | 3.2 | 2.0 | **27.0** |
| Mireille | 15.5 | 6.3 | **11.8** | 3.7 | 24.2 |
| Aurelle | **19.3** | 13.7 | 2.5 | 2.0 | 22.3 |
| Selene | 13.0 | 13.5 | 8.5 | 1.5 | 20.0 |
| Odette | 12.5 | 11.8 | 2.0 | **-1.8** | 22.3 |
| Lunaith | 13.0 | 11.0 | **-4.5** | 5.7 | 17.5 |
| Mordessa | 10.3 | 9.5 | 3.2 | 2.8 | 16.3 |
| Thessaly | 13.0 | 9.5 | -1.0 | 2.0 | 17.7 |
| Marisol | 12.0 | 7.5 | 3.5 | **-0.3** | 18.0 |
| Elara | 10.7 | 6.3 | 1.5 | 5.2 | 15.0 |
| Nimue | **7.7** | 10.8 | 1.3 | 3.5 | **10.3** |
| Vesper | **8.8** | **3.2** | 1.8 | **-0.3** | 14.7 |

## Supports

| Hero | Ultimate | Skill 1 | Skill 2 | Passive | Auto attacks |
| --- | --- | --- | --- | --- | --- |
| *role average* | *6.1* | *2.5* | *1.2* | *3.4* | *14.9* |
| Seravelle | 8.3 | **8.0** | **10.8** | **13.2** | 16.2 |
| Elowen | **16.5** | 3.5 | 1.7 | 3.8 | **24.5** |
| Lorelei | 6.8 | 5.2 | 3.2 | **7.7** | **22.2** |
| Fenna | 3.2 | 3.5 | 5.3 | **10.0** | 18.7 |
| Velisande | 11.3 | 2.2 | 2.5 | 2.2 | 16.5 |
| Amarante | 7.0 | 4.3 | 0.2 | 4.7 | 12.3 |
| Ondine | 5.3 | 1.5 | -1.2 | 0.7 | 20.0 |
| Wren | 6.0 | 1.2 | 5.0 | **1.0** | 11.8 |
| Seren | 4.0 | 3.7 | 1.0 | 1.5 | 14.8 |
| Tempra | 4.5 | 5.7 | -4.2 | 4.0 | 13.2 |
| Pelagia | 7.7 | -0.7 | -1.7 | **0.0** | 15.3 |
| Noctelle | 2.8 | 2.8 | -0.3 | 0.5 | 11.0 |
| Liora | 3.8 | 1.2 | -3.2 | **0.7** | 12.2 |
| Zephyra | **2.3** | -0.7 | -1.0 | **-0.2** | **7.5** |
| Rosalind | **1.8** | **-4.2** | -0.8 | 0.8 | **8.0** |
