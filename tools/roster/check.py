"""Automated review of a roster: counts, stat budget, kit power, control limits, synergy graph,
distinctness, theme and counter coverage. Usage: python3 check.py [pass_number]"""
import sys, itertools, collections, json
from combat_rules import TEMPLATES, COST, STYLE_MODS
from revisions import roster_at

PASS = int(sys.argv[1]) if len(sys.argv) > 1 else 0
R = roster_at(PASS)
issues = collections.defaultdict(list)

ROLE_TARGET = {"tank": 13, "dps": 20, "healer": 12, "support": 15}
HARD_CC = {"cc_stun", "cc_sleep", "cc_fear", "cc_charm", "cc_knock"}
WEIGHT = dict(dmg_single=2, dmg_aoe=3, dmg_row=2.5, execute=2.5, heal_single=2, heal_aoe=3, shield_single=1.5, shield_aoe=3,
              revive=3, regen=1.5, cc_stun=2, cc_sleep=2, cc_root=1.5, cc_fear=2.5, cc_charm=3, cc_silence=1.5, cc_knock=1.5,
              taunt=1.5, pull=1.5, buff_atk=2, buff_def=1.5, buff_speed=1.5, buff_crit=1.5, buff_energy=2, buff_cdr=2,
              buff_lifesteal=1.5, debuff_def=2, debuff_atk=1.5, debuff_weaken=2, debuff_slow=1, debuff_blind=1.5,
              debuff_antiheal=1.5, dot_burn=1.5, dot_bleed=1.5, dot_poison=2, curse=1.5, mark=1.5, soak=1, dispel=1.5,
              cleanse=1.5, untargetable=1.5, unkillable=2, reflect=1.5, summon=2.5, energy_drain=1.5, dr=1.5, immunity=2,
              counter=1.5)
SIGNATURE = {
    "high": {"shield_aoe", "shield_single", "buff_atk", "buff_def", "cleanse", "dispel", "revive", "dr", "immunity"},
    "dark": {"dot_burn", "dot_bleed", "curse", "buff_lifesteal", "cc_fear", "execute", "untargetable", "debuff_antiheal", "unkillable"},
    "nature": {"cc_root", "regen", "summon", "dot_poison", "dot_bleed", "reflect", "cc_sleep", "debuff_weaken"},
    "ocean": {"debuff_slow", "cc_knock", "pull", "cc_charm", "soak", "shield_aoe", "cleanse", "energy_drain"},
    "arcane": {"buff_energy", "buff_cdr", "dmg_aoe", "cc_silence", "energy_drain", "dot_burn", "pull", "mark", "buff_crit"},
}
MOTIF = {
    "high": ["gold", "white", "halo", "marble", "dawn", "sun", "wing", "silver", "candle", "plate"],
    "dark": ["black", "crimson", "blood", "raven", "skull", "bone", "shadow", "smoke", "ember", "ash", "night", "velvet", "rose", "veil"],
    "nature": ["moss", "antler", "leaf", "flower", "bloom", "wolf", "fae", "thorn", "briar", "willow", "moth", "owl", "oak", "bark", "petal", "nightshade", "mushroom", "huntress"],
    "ocean": ["pearl", "sea", "tide", "wet", "storm", "coral", "scale", "shell", "rain", "ink", "tentacle", "abyss", "teal", "mermaid", "siren", "water", "wave", "lightning"],
    "arcane": ["star", "rune", "void", "moon", "dream", "hourglass", "clock", "dragon", "constellation", "indigo", "orb", "astrolabe"],
}

def tags(h):
    return [t for k in ("ultimate", "skill1", "skill2", "passive") for t in h[k]["tags"]]

def stats(h):
    t = dict(TEMPLATES[(h["role"], h["range"]) if (h["role"], h["range"]) in TEMPLATES else (h["role"], "ranged")])
    for k, v in STYLE_MODS.get(h["style"], {}).items():
        t[k] = round(t.get(k, 0) + v, 4)
    return t

def power(st):
    return sum(COST.get(k, 0) * v for k, v in st.items())

# 1 counts
by_core = collections.Counter(h["core"] for h in R)
by_role = collections.Counter(h["role"] for h in R)
for c, n in by_core.items():
    if n != 12: issues["counts"].append(f"{c} has {n} heroes (want 12)")
for r, n in ROLE_TARGET.items():
    if by_role[r] != n: issues["counts"].append(f"{r}: {by_role[r]} (want {n})")
for c in by_core:
    rc = collections.Counter(h["role"] for h in R if h["core"] == c)
    for r in ROLE_TARGET:
        if rc[r] < 2: issues["counts"].append(f"{c} has only {rc[r]} {r}")

# 2 unique names
names = collections.Counter()
for h in R:
    names[h["name"]] += 1
    for k in ("ultimate", "skill1", "skill2", "passive"):
        names[h[k]["name"]] += 1
for n, c in names.items():
    if c > 1: issues["names"].append(f"'{n}' used {c} times")
ids = collections.Counter(h["id"] for h in R)
for i, c in ids.items():
    if c > 1: issues["names"].append(f"id {i} duplicated")

# 3 stat budget per role
pw = {h["id"]: power(stats(h)) for h in R}
for role in ROLE_TARGET:
    vals = [pw[h["id"]] for h in R if h["role"] == role]
    mean = sum(vals) / len(vals)
    for h in R:
        if h["role"] == role:
            d = (pw[h["id"]] - mean) / mean
            if abs(d) > 0.03: issues["stat budget"].append(f"{h['name']} ({role}/{h['style']}) {d:+.1%} vs role mean")
    if h["style"] not in STYLE_MODS: issues["stat budget"].append(f"{h['name']}: style '{h['style']}' has no stat mods")

# 4 kit power
kp = {h["id"]: sum(WEIGHT.get(t, 1) for t in tags(h)) for h in R}
for role in ROLE_TARGET:
    vals = [kp[h["id"]] for h in R if h["role"] == role]
    mean = sum(vals) / len(vals)
    for h in R:
        if h["role"] == role:
            d = (kp[h["id"]] - mean) / mean
            if abs(d) > 0.20: issues["kit power"].append(f"{h['name']} ({role}) kit {kp[h['id']]:.1f} vs mean {mean:.1f} ({d:+.0%})")

# 5 control limits
for h in R:
    hc = [t for t in tags(h) if t in HARD_CC]
    if len(hc) > 2: issues["control"].append(f"{h['name']} has {len(hc)} hard-control effects {hc}")

# 6 synergy graph
appliers = collections.defaultdict(set); payers = collections.defaultdict(set)
for h in R:
    for k in h["applies"]: appliers[k].add(h["id"])
    for k in h["payoffs"]: payers[k].add(h["id"])
for k in payers:
    if len(appliers[k] - set()) < 2: issues["synergy"].append(f"payoff '{k}' has only {len(appliers[k])} appliers")
partners = collections.defaultdict(set)
links = collections.defaultdict(list)
for k in payers:
    for a in appliers[k]:
        for b in payers[k]:
            if a != b:
                partners[a].add(b); partners[b].add(a); links[b].append((a, k))
core = {h["id"]: h["core"] for h in R}
for h in R:
    p = partners[h["id"]]
    cross = [x for x in p if core[x] != h["core"]]
    if len(p) < 2: issues["synergy"].append(f"{h['name']} has {len(p)} synergy partners (want 2+)")
    elif not cross: issues["synergy"].append(f"{h['name']} has no cross-race partner")
    if not h["payoffs"]: issues["synergy (info)"].append(f"{h['name']} has no payoff (enabler only)")

# 7 distinctness within role
for role in ROLE_TARGET:
    hs = [h for h in R if h["role"] == role]
    for a, b in itertools.combinations(hs, 2):
        A, B = set(tags(a)), set(tags(b))
        j = len(A & B) / len(A | B)
        if j >= 0.6: issues["distinct"].append(f"{a['name']} / {b['name']} share {j:.0%} of kit tags")
styles = collections.Counter((h["role"], h["style"]) for h in R)
for (r, s), c in styles.items():
    if c > 1:
        who = [h["name"] for h in R if h["role"] == r and h["style"] == s]
        issues["distinct"].append(f"style '{s}' used by {', '.join(who)}")

# 8 theme
for h in R:
    sig = SIGNATURE[h["core"]] & set(tags(h))
    if not sig: issues["theme"].append(f"{h['name']} uses none of {h['core']}'s signature mechanics")
    if not any(m in h["look"].lower() for m in MOTIF[h["core"]]): issues["theme"].append(f"{h['name']} look has no {h['core']} motif")

# 9 counters
COUNTERS = {"untargetable": {"aoe or stealth reveal": lambda h: "Night Sight" in h["passive"]["name"]},
            "heal": {"anti-heal": lambda h: "debuff_antiheal" in tags(h)},
            "shield": {"shield breaker": lambda h: "+30% damage to shields" in h["passive"]["text"]},
            "buff": {"dispel": lambda h: "dispel" in tags(h)},
            "hard cc": {"cleanse or immunity": lambda h: {"cleanse", "immunity"} & set(tags(h))},
            "revive": {"revive block": lambda h: "revive" in h["passive"]["text"].lower() and "can't" in h["passive"]["text"].lower()},
            "summon": {"summon clear": lambda h: "summon" in (h["skill1"]["text"] + h["skill2"]["text"] + h["passive"]["text"]).lower() and "damage" in (h["skill1"]["text"] + h["skill2"]["text"] + h["passive"]["text"]).lower() and h["role"] != "dps" or "summons take" in h["passive"]["text"].lower()},
            "energy": {"energy drain": lambda h: "energy_drain" in tags(h)}}
for mech, d in COUNTERS.items():
    for name, fn in d.items():
        n = sum(1 for h in R if fn(h))
        if n == 0: issues["counters"].append(f"nothing counters {mech} ({name})")

report = {"pass": PASS, "heroes": len(R), "issues": {k: v for k, v in issues.items()}, "issue_count": sum(len(v) for k, v in issues.items() if not k.endswith("(info)")),
          "partners": {h["name"]: len(partners[h["id"]]) for h in R}}
print(f"PASS {PASS}: {len(R)} heroes, {report['issue_count']} issues")
for k, v in issues.items():
    print(f"\n[{k}] {len(v)}")
    for x in v: print("  -", x)
json.dump(report, open(f"report_pass{PASS}.json", "w"), indent=1)
