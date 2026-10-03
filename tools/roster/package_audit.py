"""Package audit (decision row 30): every hero's synergy package claims must match her executable kit.

Checks, for the 8 enemy-side packages: each package a hero "applies" comes from one of her abilities; each package she
"pays off" appears in her passive's conditions; no passive condition rewards a package she is not listed for; no status
sits in two packages; no retired keyword remains. Team-side packages (Lifesteal, Stealth, Tempo, Guarded) are checked by hand.
Run from anywhere: python tools/roster/package_audit.py   (exit code 1 when there are findings)
"""
import json, re, collections, pathlib, sys
REPO = pathlib.Path(__file__).resolve().parents[2]
H = json.load(open(REPO / "Assets/_Game/Data/heroes.json", encoding="utf-8"))["heroes"]
ENEMY = {"Burn": {"burn"}, "Soaked": {"soaked"}, "Wounds": {"bleed", "poison"}, "Hindered": {"slow", "root", "stun", "airborne"},
         "Dread": {"sleep", "fear", "charm"}, "Exposed": {"mark", "weaken", "def_down", "curse", "feeding_curse"},
         "Disrupted": {"blind", "silence", "drained"}, "Gathered": {"grouped", "taunt"}}
BUFFS = {"def_up", "atk_up", "aspd_up", "crit_up", "haste_up", "regen", "dr", "halo", "thorns", "counter", "reflect", "cc_immunity",
         "lifesteal_up", "skill_shield", "projectile_block", "untargetable", "sure_crit", "revive_guard", "hit_shield", "share", "thread",
         "bond_heal", "seed", "redirect", "unkillable", "dragonform", "revive_ready", "planted", "stealth"}
seen = collections.defaultdict(set)
for pk, ss in ENEMY.items():
    for s_ in ss: seen[s_].add(pk)
problems = []
for s_, pks in seen.items():
    if len(pks) > 1: problems.append(f"status {s_} is in two packages: {pks}")

def walk(ops):
    for o in ops:
        yield o
        yield from walk(o.get("ops", []))
        yield from walk(o.get("then", []))

for h in H:
    hid = h["id"]; sk = h["skills"]
    ops = [o for k in ("ultimate", "skill1", "skill2") for o in walk(sk[k].get("ops", []))]
    p = sk["passive"]; pops = [o for t in p.get("triggers", []) for o in walk(t["ops"])]
    allops = ops + pops
    enemy_to = lambda o: not str(o.get("to", "")).startswith(("self", "ally", "allies", "weakest_all", "nearest_ally", "most_debuffed", "most_targeted", "dead_ally", "zone_allies", "evt_nearest_ally"))
    applied = set()
    for o in allops:
        if o["op"] == "status" and enemy_to(o): applied.add(o["s"])
        if o["op"] == "energy" and o.get("v", 0) < 0 and enemy_to(o): applied.add("drained")
        if o["op"] == "move" and o.get("kind", "").startswith("pull") and o.get("to") not in ("highest_atk_enemy", "backrow_enemy"): applied.add("grouped")
        if o["op"] == "summon" and o.get("tauntR"): applied.add("taunt")
        if o["op"] == "status" and o["s"] == "dragonform": applied.add("burn")
        if o["op"] == "zone": pass
    # evidence for each declared enemy package applied
    for pk in h["applies"]:
        if pk in ENEMY and not (applied & ENEMY[pk]):
            problems.append(f"{hid}: applies {pk} but no ability applies {sorted(ENEMY[pk])} (applied: {sorted(applied)})")
    # undeclared applies
    for pk, ss in ENEMY.items():
        if applied & ss and pk not in h["applies"]:
            problems.append(f"{hid}: applies {sorted(applied & ss)} but is not listed as applying {pk}")
    # payoffs vs passive conditions
    conds = [m.get("if", "") for m in p.get("mods", [])] + [t.get("if", "") for t in p.get("triggers", [])]
    condstat = set()
    for c in conds:
        for tok in re.split(r"[|]", c or ""):
            tok = tok.split(":")[-1].split(".")[-1]; tok = re.sub(r"\+\d+(\.\d+)?$", "", tok).lstrip("!")
            condstat.add(tok)
    if any(o.get("soakedDouble") or o.get("onlySoaked") for o in allops): condstat.add("soaked")
    for pk in h["payoffs"]:
        if pk in ENEMY and not (condstat & ENEMY[pk]):
            problems.append(f"{hid}: pays off {pk} but her passive never checks {sorted(ENEMY[pk])} (checks {sorted(condstat - {''})})")
    for pk, ss in ENEMY.items():
        if condstat & ss and pk not in h["payoffs"]:
            problems.append(f"{hid}: passive checks {sorted(condstat & ss)} but she is not listed as paying off {pk}")
    for kw in h["applies"] + h["payoffs"]:
        if kw in {"Ambush", "Kill", "Dispel", "Isolated", "Airborne", "Rooted", "Stunned", "Slow", "Asleep", "Feared", "Charmed", "Mark", "Weakened", "Curse", "Bleed", "Poison", "Blind", "Drained", "Grouped", "Taunted", "Energy", "Cooldown", "Shielded", "Buffed"}:
            problems.append(f"{hid}: still uses retired keyword {kw}")
print(len(problems), "findings"); print("\n".join(sorted(problems)))
sys.stdout.flush()
pk_count = collections.Counter(k for h in H for k in h["payoffs"]); ap_count = collections.Counter(k for h in H for k in h["applies"])
print({k: (ap_count[k], pk_count[k]) for k in sorted(set(pk_count) | set(ap_count))})
sys.exit(1 if problems else 0)
