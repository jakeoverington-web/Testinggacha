"""Revision passes applied on top of heroes_v0. Each change records why, so the compendium can show a revision log."""
import copy
from heroes_v0 import HEROES

# Each pass: list of (hero_id, path, new_value, reason). path like "skill2.name" or "applies" or "style".
PASSES = {}

def _set(h, path, val):
    obj = h
    parts = path.split(".")
    for p in parts[:-1]:
        obj = obj[p]
    last = parts[-1]
    if last.startswith("+"):          # append to list
        obj[last[1:]] = obj[last[1:]] + list(val)
    elif last.startswith("-"):        # remove from list
        obj[last[1:]] = [x for x in obj[last[1:]] if x not in val]
    else:
        obj[last] = val

def roster_at(n):
    R = copy.deepcopy(HEROES)
    idx = {h["id"]: h for h in R}
    for p in range(1, n + 1):
        for hid, path, val, why in PASSES.get(p, []):
            _set(idx[hid], path, val)
    return R

def log():
    return {p: [(hid, path, why) for hid, path, val, why in ch] for p, ch in PASSES.items()}

# ---------------- PASS 1: synergy holes, power bands, distinctness, theme, counters ----------------
PASSES[1] = [
 # Ocean identity: the Soaked keyword (applied by Ocean, paid off by storm and siren heroes)
 ("thalassa","ultimate.text","Knocks the enemy front row back, slows them and leaves them Soaked for 3s.","Ocean needs a shared race mechanic; Soaked links the race"),
 ("thalassa","ultimate.+tags",["soak"],"Soaked"),("thalassa","+applies",["Soaked"],"Soaked"),
 ("coralie","ultimate.text","A wall of water blocks enemy projectiles for 3s; enemies who touch it are Soaked.","Coralie's kit was 31% under the tank band and had no Ocean mechanic"),
 ("coralie","ultimate.+tags",["soak"],"Soaked"),("coralie","+applies",["Soaked"],"Soaked"),
 ("pelagia","skill1.text","Soaks and slows one enemy for 3s.","Spray now applies Soaked"),("pelagia","skill1.+tags",["soak"],"Soaked"),
 ("pelagia","ultimate.text","Soaks and pushes back all enemies; allies gain 20 energy.","High Tide now applies Soaked"),("pelagia","+applies",["Soaked"],"Soaked"),
 ("marisol","ultimate.text","Rain for 6s: allies heal 4% max HP/s and enemies are Soaked.","Monsoon soaks enemies too"),("marisol","ultimate.+tags",["soak"],"Soaked"),("marisol","+applies",["Soaked"],"Soaked"),
 ("halcyra","skill1.text","Soaks the target; her next hit on it bounces.","Kael lacked an Ocean mechanic"),("halcyra","skill1.+tags",["soak"],"Soaked"),("halcyra","+applies",["Soaked"],"Soaked"),
 ("halcyra","passive.text","Crits bounce once. Each skill she casts adds a bounce to her next Tempest; bounces between Soaked enemies hit twice.","Kael had one synergy partner; now pays off Soaked and Cooldown"),
 ("halcyra","+payoffs",["Soaked","Cooldown"],"payoffs"),
 ("ondine","skill2.text","Soaks the target and lowers its DEF by 30% for 5s.","Ondine lacked an Ocean mechanic"),("ondine","skill2.+tags",["soak"],"Soaked"),("ondine","+applies",["Soaked"],"Soaked"),
 ("ondine","style","Inkweaver","Two supports shared the Debuffer style"),
 ("ysra","skill2.text","Soaks and slows enemies in an area for 4s.","Ysra and Kael shared 67% of kit tags"),("ysra","skill2.tags",["soak","debuff_slow"],"distinct from Kael"),
 ("ysra","passive.text","Her hits on Soaked enemies deal +25% and chain to one more Soaked enemy.","Ysra paid off Stun, which no longer fits; she is the Soaked finisher"),
 ("ysra","payoffs",["Soaked"],"payoff"),("ysra","+applies",["Soaked"],"Soaked"),
 ("lorelei","passive.text","Enemies she hits lose 5 energy; Soaked enemies lose 10.","Lorelei had no synergy partners"),("lorelei","+payoffs",["Soaked"],"payoff"),("lorelei","+applies",["Drained"],"Drained"),
 # Drained: energy theft as a keyword
 ("liora","+applies",["Drained"],"Ember Sigil drains energy"),
 ("isaura","passive.text","+25% damage to enemies with less than 50 energy and to sleeping enemies.","Isaura had no synergy partners; now pays off Drained and Asleep"),
 ("isaura","payoffs",["Drained","Asleep"],"payoffs"),("isaura","+applies",["Drained"],"Drained"),
 # LowHP removed; Kill gets real appliers
 ("nyx","payoffs",["Stealth","Mark","Asleep"],"LowHP had no appliers; Nyx now pays off marks and sleep too (Epic Seven: sleep lowers crit resistance)"),
 ("nyx","passive.text","+40% crit damage from behind, from stealth or against sleeping enemies; marked enemies are her first targets.","matches payoffs"),
 ("nyx","+applies",["Kill"],"executes"),("sylwen","+applies",["Kill"],"executes"),
 ("sylwen","payoffs",["Rooted","Mark"],"LowHP removed; adds a cross-race partner"),
 ("sylwen","passive.text","Sees through stealth; +15% crit chance against rooted or marked enemies.","matches payoffs"),
 ("sable","payoffs",["Kill","Bleed"],"LowHP removed"),
 ("sable","passive.text","Gains 10 energy whenever any enemy falls; +15% damage to bleeding enemies; enemies she kills can't be revived.","adds the game's only revive block"),
 # Fear gets a second applier; Velisande gets a cross-race partner
 ("noctelle","skill2.name","Haunt","Feared had one applier"),("noctelle","skill2.text","Fears her target for 1.5s.","Feared"),
 ("noctelle","skill2.tags",["cc_fear"],"fear"),("noctelle","+applies",["Feared"],"Feared"),
 ("velisande","+payoffs",["Burn"],"Velisande had no cross-race partner"),
 ("velisande","passive.text","Feared or burning enemies take 10% more damage.","matches payoffs"),
 ("calypso","+payoffs",["Charmed","Stealth"],"Nocturne had no cross-race partner; Calypso strikes from hiding"),
 ("calypso","passive.text","+25% damage to isolated or charmed enemies, and her first hit from stealth crits.","matches payoffs"),
 ("cassia","+payoffs",["Lifesteal"],"Mordessa had no cross-race partner"),
 ("cassia","passive.text","Lifesteal heals her 50% more; each buff she removes shields her for 5% max HP.","matches payoffs"),
 ("lucienne","+payoffs",["Stunned"],"Thalassa had no cross-race partner"),
 ("lucienne","passive.text","+40% crit damage against blinded or stunned enemies.","matches payoffs"),
 ("elara","passive.text","Allies with her halo take 5% less damage; her heals are 20% stronger on buffed allies.","Elara had one partner"),("elara","payoffs",["Buffed"],"payoff"),
 ("vesper","payoffs",["Bleed"],"Vesper had one partner"),("vesper","passive.text","Her heals are 20% stronger while any enemy is bleeding.","matches payoffs"),
 ("kaida","+payoffs",["Rooted"],"Kaida had one partner"),("kaida","passive.text","+15% damage while a beast is out; her wolves deal double damage to rooted enemies.","matches payoffs"),
 ("caelith","payoffs",["Mark","Weakened"],"Caelith had one partner; Wren had no cross-race partner"),
 ("caelith","passive.text","+30% damage to shields, and +15% to marked or weakened enemies.","matches payoffs"),
 ("seravelle","payoffs",["Cooldown"],"Tempra had no partners: faster skills mean more energy from Hymn"),
 # Kit power: trims and lifts toward the role band
 ("cassia","ultimate.text","Holy zone for 5s: enemies inside take 40% ATK per second.","Cassia's kit was 27% above the tank band"),("cassia","ultimate.tags",["dmg_aoe"],"trim"),
 ("cassia","style","Crusader","Two tanks shared the Bruiser style"),
 ("sangrael","ultimate.tags",["dmg_aoe"],"Morwen was 27% above the band; drain counts as her lifesteal, not a heal"),
 ("solenne","skill2.text","Blinds enemies in a cone for 2s; allies in its light gain ATK up (25%) for 4s.","Solenne was 27% under the band and had no High mechanic"),("solenne","skill2.+tags",["buff_atk"],"buff"),
 ("solenne","passive.name","Sunlit","'Glare' was used twice"),("solenne","passive.text","Her crits against marked enemies blind them for 2s.","rename"),
 ("lucienne","skill1.text","Counter stance for 2s and shields herself for 10% max HP.","Lucienne was 21% under the band and had no High mechanic"),("lucienne","skill1.+tags",["shield_single"],"shield"),
 ("ophira","skill2.text","Becomes untargetable for 1s and gains a shield of 15% max HP.","Ophira had no High mechanic"),("ophira","skill2.+tags",["shield_single"],"shield"),
 ("maelis","ultimate.text","Smouldering ash burns all enemies for 6s.","Maelis was 20% above the band"),("maelis","ultimate.tags",["dot_burn"],"trim"),
 ("sable","ultimate.text","A scythe sweep that executes every enemy below 25% HP; each kill refunds 50 energy.","Sable was 20% above the band"),("sable","ultimate.tags",["execute"],"trim"),
 ("nimue","skill1.text","Heals one ally and gives them Regen (3% HP/s) for 4s.","Nimue was 22% under the band and had no Nature mechanic"),("nimue","skill1.+tags",["regen"],"regen"),
 ("selene","skill1.text","Heals one ally and gives them 10 energy.","Selene was 22% under the band and had no Arcane mechanic"),("selene","skill1.+tags",["buff_energy"],"energy"),("selene","+applies",["Energy"],"Energy"),
 ("liora","skill1.tags",["energy_drain"],"Liora was 26% above the band; the mark moved off her"),("liora","skill1.text","Drains 30 energy from the highest-ATK enemy.","trim"),
 ("fenna","skill2.text","Slows and poisons (2% HP/s) enemies in an area for 3s.","Fenna was 20% under the band"),("fenna","skill2.+tags",["dot_poison"],"poison"),("fenna","+applies",["Poison"],"Poison"),
 ("seren","skill1.text","Marks a target (+15% damage taken) for 5s; allies who hit it gain 5 energy.","Seren was 20% under the band"),("seren","skill1.+tags",["buff_energy"],"energy"),("seren","+applies",["Energy"],"Energy"),
 # Distinctness
 ("astraea","skill2.name","Comet Trail","keep"),("astraea","skill2.text","A comet knocks up enemies in a line.","Astraea and Zaria shared 100% of kit tags"),
 ("astraea","skill2.tags",["cc_knock"],"distinct"),("astraea","applies",["Airborne"],"Airborne"),
 ("astraea","passive.text","+20% damage to enemies standing close together; summons take double damage from her.","adds a counter to summon teams"),
 ("ilyra","style","Pyromancer","Two DPS shared the Nuker style"),
 ("vaela","skill1.text","Shields herself and silences the nearest enemy for 1s.","Vaela shared 75% of kit tags with Coralie"),("vaela","skill1.+tags",["cc_silence"],"distinct"),
 ("mireille","ultimate.text","Heals all over 8s and leaves Regen (2% HP/s) for 4s after.","Mireille and Selene shared 80% of kit tags"),("mireille","ultimate.tags",["heal_aoe","regen"],"distinct"),
 ("sylwen","style","Sniper","Two DPS shared the Marksman style"),
 ("marisol","style","Rainmaker","Two healers shared the Regen style"),
 ("coralie","style","Tidewall","Two tanks shared the Guardian style"),
 ("siora","skill2.text","Repeats her last heal at 50% on another ally and cleanses one debuff.","Siora had no Ocean mechanic"),("siora","skill2.+tags",["cleanse"],"cleanse"),
 # Theme: looks
 ("draxa","look","Dragon horns and indigo scales along her spine, slit-pupil gold eyes, smoke at her lips","look lacked an Arcane motif"),
 ("runa","look","Rune-carved stone plates fused to silk, glowing rune-light glyphs, cracked-porcelain skin","look lacked an Arcane motif"),
]

# ---------------- PASS 2: remaining outliers ----------------
PASSES[2] = [
 ("vaela","ultimate.text","A zone absorbs all enemy projectiles for 3s.","Vaela was 23% above the tank band after gaining a silence; the pull now lives only in Gravity Well"),
 ("vaela","ultimate.tags",["dr"],"trim"),
 ("elowen","skill2.text","One ally gains Haste up (+30) for 4s, so their skills recharge faster.","'Cooldown' had one applier; Elowen bends time for skills as well as energy"),
 ("elowen","skill2.tags",["buff_cdr"],"cooldown"),("elowen","+applies",["Cooldown"],"Cooldown"),
 ("kaida","+payoffs",["Airborne"],"Kaida had no cross-race partner"),
 ("kaida","passive.text","+15% damage while a beast is out; her wolves deal double damage to rooted or knocked-up enemies.","matches payoffs"),
 ("ysra","ultimate.text","A bolt strikes the Soaked or marked enemy with the lowest HP for 450% ATK.","Ysra still shared 60% of kit tags with Kael and Nerissa; she becomes the single-target finisher"),
 ("ysra","ultimate.tags",["dmg_single","execute"],"distinct"),
 ("ysra","+payoffs",["Mark"],"Ysra had no cross-race partner"),
 ("ysra","passive.text","Her hits on Soaked or marked enemies deal +25% and chain to one more Soaked enemy.","matches payoffs"),
 ("siora","ultimate.text","Sings for 4s: allies heal 5% HP/s and enemies who hear it are slowed.","Siora and Elara shared 67% of kit tags"),
 ("siora","ultimate.tags",["heal_aoe","debuff_slow"],"distinct"),("siora","+applies",["Slow"],"Slow"),
]

# ---------------- PASS 3: manual read-through (numbers, duplicates, rule caps, wording) ----------------
PASSES[3] = [
 # Number bands: area ults ~240-280%, row ults ~280-320%, single-target ults ~340-450%
 ("astraea","ultimate.text","Huge area blast for 280% ATK.","Area ultimates sit at 240-280%; 360% out-damaged every other area ult"),
 ("ilyra","ultimate.text","Fire blast on all enemies for 240% ATK; if she falls within 10s she revives at 30% (once).","Lower multiplier pays for her self-revive"),
 ("zaria","ultimate.text","Dragon breath in a cone for 260% ATK, applying Burn.","Area band"),
 ("calypso","ultimate.text","Pulls the enemy carry to her and devours them for 340% ATK, healing for half.","The pull plus heal earns a lower multiplier than a pure single-target nuke"),
 ("sylwen","ultimate.text","Three shots at the weakest enemies for 160% ATK each, ignoring 30% DEF.","Numbers were missing"),
 ("mireille","ultimate.text","Heals all for 40% over 8s, then leaves Regen (2% HP/s) for 4s.","Numbers were missing"),
 # Duplicate team buff
 ("rosalind","ultimate.text","Team DEF up (30%) for 6s and removes one debuff from each ally.","Rosalind and Seraphine both had 'team ATK up 25% for 6s'"),
 ("rosalind","ultimate.tags",["buff_def","cleanse"],"distinct"),
 # Control caps (max 2.5s)
 ("lorelei","ultimate.text","Charms two enemies for 2.5s.","3s broke the 2.5s control cap"),
 ("lunaith","ultimate.text","Heals all for 25% and makes them immune to sleep and stun for 4s.","Too close to Elara's full control immunity; Lunara now guards against sleep and stun only"),
 # Wording that didn't match the synergy links
 ("sangrael","skill1.text","Taunts nearby enemies; she and allies near her gain Lifesteal up (15%) for 4s.","Her lifesteal was self-only, so it couldn't feed Cassia or Mordessa"),
 ("corvina","payoffs",[],"'When an ally falls' is not an enemy kill; the Kill link was wrong"),
]

# ---------------- PASS 4: synergy fairness (no must-pick hub) ----------------
PASSES[4] = [
 ("astraea","payoffs",["Grouped"],"Astraea had 15 synergy partners (every energy support fed her): a must-pick risk. She keeps the gather-and-nuke role"),
 ("rhiannon","+payoffs",["Energy"],"Energy payoff moves to Rhiannon, who had only 2 partners"),
 ("rhiannon","passive.text","A spirit stag fights beside her all battle; whenever an ally gives her energy, the stag's next attack deals double damage.","matches payoffs"),
 ("ilyra","passive.text","Her burns stack up to three times; each stack on a target raises her damage to it by 5%.","Ilyra's passive gave her nothing herself; a small self-payoff for her burn stacks"),
]

# ---------------- PASS 5: final read of the published tables ----------------
PASSES[5] = [
 ("tempra","skill1.text","Moves one ally's skill cooldowns forward by 3s.","Accelerate duplicated Elowen's Hasten (Haste +30 for 4s)"),
 ("vaela","skill1.name","Void Barrier","Three 'Rune' skill names across different heroes; Vaela is Void"),
 ("runa","skill1.name","Glyph Ward","Same naming clash; matches her Glyph Mirror passive"),
]

# ---------------- PASS 6: tag audit by the executable kits (C# KitTests) ----------------
# Tags must describe what the ability really does: the battle tests now check every tag against the sim.
PASSES[6] = [
 ("isolde","ultimate.tags",["dr"],"Martyrdom takes allies' damage but never taunts; the taunt tag overstated it"),
 ("nyx","skill2.tags",["buff_crit"],"Shadowstep deals no damage itself: it sets up a guaranteed crit"),
]

# ---------------- PASS 7: first balance pass from the niche report v3 (battle sim, 234,000 paired battles) ----------------
# Dominant heroes toned down, clearly no-niche heroes strengthened. Borderline heroes left until the meta settles.
PASSES[7] = [
 ("isaura","ultimate.text","Drains 25 energy from all enemies and deals 5% ATK per energy drained.","Dominant (+23): energy denial stopped enemy ultimates; 40 -> 25"),
 ("isaura","skill1.text","Steals 10 energy from her target.","Dominant: Siphon 15 -> 10"),
 ("solenne","ultimate.text","A beam that pierces the whole row for 260% ATK.","Dominant (+20): Noon Lance 320% -> 260%"),
 ("ilyra","ultimate.text","Fire blast on all enemies for 190% ATK; if she falls within 10s she revives at 30% (once).","Dominant (+16): Rebirth Flame 240% -> 190%"),
 ("cassia","ultimate.text","Holy zone for 5s: enemies inside take 30% ATK per second.","Dominant (+11): Sanctified Ground 40% -> 30%"),
 ("coralie","ultimate.text","A wall of water blocks enemy projectiles for 2s; enemies who touch it are Soaked.","Dominant (+11): Tidal Bulwark 3s -> 2s"),
 ("caelith","ultimate.text","Three hits that each deal 25% of the target's max HP.","No niche (-23): heroes have low HP, so 15% max HP was weaker than a basic skill; 15% -> 25%"),
 ("venna","skill1.text","Poisons the target (3%/s) for 4s.","No niche (-19): poison is % max HP; 2% -> 3%"),
 ("venna","ultimate.text","Poisons all enemies (6% max HP/s) for 4s.","No niche: Nightshade Bloom 4% -> 6%"),
 ("nyx","ultimate.text","Blinks to the weakest enemy, strikes for 300% ATK and executes her below 25% HP.","No niche (-17): Night's Edge 200% -> 300%, execute 20% -> 25%; the strike is now in the text"),
 ("vesper","skill1.text","Curses an enemy: whoever hits it heals for 30% of the damage.","No niche (-12): Feeding Curse 15% -> 30%; the amount is now in the text"),
 ("maelis","ultimate.text","Smouldering ash burns all enemies for 50% ATK per second for 6s.","No niche (-11): Ashfall 35% -> 50% ATK/s; the amount is now in the text"),
]

# ---------------- PASS 8: from the ablation report (what each kit part is worth) ----------------
PASSES[8] = [
 ("caelith","targeting","Enemy carry (highest ATK)","Highest-HP targeting sent her into tanks; carry targeting alone was worth +32 points"),
 ("caelith","ultimate.text","Three hits that each deal 15% of the target's max HP.","Pass 7 buff pulled back now that she hits carries instead of tanks"),
 ("maelis","targeting","Enemy carry (highest ATK)","Highest-HP targeting sent her into tanks; carry targeting alone was worth +22 points"),
 ("maelis","ultimate.text","Smouldering ash burns all enemies for 35% ATK per second for 6s.","Pass 7 buff pulled back with the targeting fix"),
 ("venna","targeting","Lowest-HP enemy","Highest-HP targeting sent her into tanks; finishing the weak also feeds Contagion (+16 points)"),
 ("venna","skill1.text","Poisons the target (2%/s) for 4s.","Pass 7 buff pulled back with the targeting fix"),
 ("venna","ultimate.text","Poisons all enemies (4% max HP/s) for 4s.","Pass 7 buff pulled back with the targeting fix"),
 ("solenne","skill1.cd",8,"Sunshot's mark makes her whole team focus the enemy carry every 6s; now every 8s"),
 ("thalassa","ultimate.text","Drags the enemy front row toward her, slows them and leaves them Soaked for 3s.","Knocking enemies away from her own melee team made Undertow worth nothing; an undertow pulls in"),
 ("thalassa","ultimate.tags",["pull","debuff_slow","soak"],"Undertow now pulls"),
 ("isolde","ultimate.text","For 4s she takes 50% of the damage dealt to allies, and takes 30% less damage herself.","Martyrdom was worth almost nothing: she absorbed damage she could not survive"),
]
