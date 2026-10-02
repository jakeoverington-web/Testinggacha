# Roster tools

Builds `Assets/_Game/Data/heroes.json` and `combat.json` and reviews the roster.

- `combat_rules.py`: stat sheet, energy, effect library, control rules, keywords, role templates, style stat trades.
- `heroes_v0.py`: the first draft of every hero.
- `revisions.py`: numbered revision passes; every edit records why.
- `check.py N`: reviews the roster as of pass N (counts, stat budget, kit power, control limits, synergy links, look-alike kits, race identity, WLOP look motifs, counters).
- `build.py`: writes the JSON data and markdown tables for the Hero Compendium doc.

To change a hero: add an entry to a new pass in `revisions.py`, run `python3 check.py <pass>` until it reports 0 issues, then `python3 build.py`.
