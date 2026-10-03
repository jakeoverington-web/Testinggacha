using System.Linq;
using Gacha.Core.Battle;
using Gacha.Core.Campaign;
using Gacha.Core.Gear;
using Gacha.Core.Progression;
using UnityEngine.UIElements;

namespace Gacha.UI
{
    /// <summary>
    /// Hero detail (phase 2 spec; Omniheroes reference): 1:2 art placeholder, gold/diamond stars, stats, 4 gear slots with
    /// a piece list (equip, upgrade, salvage), skills sliding out with ranks, and Level Up, Stars, Equip Best, Reset and
    /// Contract. Arg: the hero id.
    /// </summary>
    public sealed class HeroDetailController : IScreen
    {
        static readonly string[] SlotNames = { "weapon", "helm", "armor", "boots" };

        ScreenRouter _r;
        VisualElement _root;
        string _id, _slot = "", _message = "";

        GameData G => _r.Data;
        CampaignState S => _r.State;

        public void Bind(VisualElement root, ScreenRouter router, object arg)
        {
            _r = router; _root = root; _id = (string)arg;
            root.Q<Button>("back").clicked += () => _r.Show("heroes");
            root.Q<Button>("skills-toggle").clicked += () => root.Q("skills").ToggleInClassList("skills--open");
            root.Q<Button>("level").clicked += LevelUp;
            root.Q<Button>("star").clicked += StarUp;
            root.Q<Button>("best").clicked += () => { S.Inventory.EquipBest(G.Gear, _id); Done("Equipped the best free pieces."); };
            root.Q<Button>("reset").clicked += () => { Stars.Reset(G, S.Collection, _id, S.Wallet); Done("Reset to 1 star: copies, gold and Sigils refunded."); };
            Fill();
        }

        public void Tick(float dt) { }

        void Done(string message) { _message = message; _r.Game.Save(); Fill(); }

        void LevelUp()
        {
            int slot = S.Collection.SlotOf(_id);
            if (slot < 0) { _message = "Only Contract heroes level up; everyone else follows the lowest slot."; Fill(); return; }
            Done(S.Collection.LevelUp(G, slot, S.Wallet) ? "Level up." : "Not enough Hero XP, gold or Starlight, or at the star cap.");
        }

        void StarUp()
        {
            bool Locked(string h) => S.Inventory.IsGeared(h);
            var (fodder, sigils) = Stars.AutoFodder(G, S.Collection, _id, Locked);
            string why = Stars.Why(G, S.Collection, _id, fodder, sigils, S.Wallet, Locked);
            if (why != null) { _message = "Can't raise stars: " + why + "."; Fill(); return; }
            Stars.Raise(G, S.Collection, _id, fodder, sigils, S.Wallet, Locked);
            Done($"Now {S.Collection.Get(_id).Stars} stars.");
        }

        void Fill()
        {
            var h = G.Heroes[_id]; var o = S.Collection.Get(_id);
            if (o == null) { _r.Show("heroes"); return; }   // fed to another hero elsewhere
            int level = S.Collection.Level(G, _id), cap = G.Levels.Cap(o.Stars);
            var bonus = S.Inventory.BonusFor(G.Gear, _id);

            _root.Q<Label>("name").text = h.Name;
            var stars = _root.Q<Label>("stars");
            stars.text = Placeholder.Stars(o.Stars);
            stars.EnableInClassList("detail__stars--diamond", o.Stars > 5);
            int slot = S.Collection.SlotOf(_id);
            _root.Q<Label>("sub").text = $"{h.Role} · {h.Core} · Lv {level}" + (slot >= 0 ? $" (slot {slot + 1}, cap {cap})" : " (synced)") +
                                         $" · Power {Placeholder.Short(S.HeroPower(G, _id))} · +{o.Copies} copies";

            var art = _root.Q("art");
            art.style.backgroundColor = Placeholder.RaceColour(h.Core);
            art.EnableInClassList("art--diamond", o.Stars >= 10);
            _root.Q<Label>("art-label").text = $"hero_{_id}_fullbody\n(art in phase 5)";

            var st = h.Stats.Clone(); double m = G.Progression.Mult(level, o.Stars);
            st.Hp *= m * G.Tuning.HpScale; st.Atk *= m; st.Def *= m; bonus.ApplyTo(st);
            _root.Q<Label>("stats").text =
                $"HP {st.Hp:N0}\nATK {st.Atk:N0}\nDEF {st.Def:N0}\nAttack speed {st.AtkSpd:0.00}\nHaste {st.Haste:0}\nCrit {st.CritRate:P0} x{st.CritDmg:0.00}\n" +
                $"Effect hit {st.EffectHit:P0} · resist {st.EffectRes:P0}\nLifesteal {st.Lifesteal:P0}\nHealing {st.HealPower:P0} done · {st.HealRecv:P0} received";

            var ranks = CampaignState.RanksFor(o.Stars);
            _root.Q<Label>("skills").text = $"Ultimate: {h.Ult.Name} (rank {ranks[0]})\nSkill 1: {h.S1.Name} (rank {ranks[1]})\n" +
                                            $"Skill 2: {h.S2.Name} (rank {ranks[2]})\nPassive: {h.Passive.Name} (rank {ranks[3]})";

            FillGear();

            var row = G.StarCosts;
            _root.Q<Button>("star").text = o.Stars >= G.Levels.MaxStars ? "Max stars" :
                $"Stars → {o.Stars + 1}\n{row.For(o.Stars + 1).Copies} copies · {row.For(o.Stars + 1).Fodder} fodder · {Placeholder.Short(row.For(o.Stars + 1).Gold)} gold";
            _root.Q<Button>("level").text = slot < 0 ? "Level Up\n(Contract heroes only)" :
                level >= cap ? $"Level Up\nat cap {cap}" : $"Level Up\n{Placeholder.Short(G.Levels.Xp(level + 1))} XP · {Placeholder.Short(G.Levels.Gold(level + 1))} gold";

            var contract = _root.Q("contract");
            contract.Clear();
            for (int i = 0; i < Collection.SlotCount; i++)
            {
                int index = i;
                var b = new Button(() => { S.Collection.Contract(index, _id); Done($"Contract slot {index + 1}."); })
                    { text = slot == i ? $"In slot {i + 1}" : $"Slot {i + 1} (Lv {S.Collection.Slots[i].Level})" };
                b.AddToClassList("btn");
                b.SetEnabled(slot != i);
                contract.Add(b);
            }
            _root.Q<Label>("message").text = _message;
        }

        void FillGear()
        {
            var gear = _root.Q("gear");
            gear.Clear();
            foreach (var slotName in SlotNames)
            {
                string s = slotName;
                var piece = S.Inventory.On(_id).FirstOrDefault(i => G.Gear.Type(i.Type).Slot == s);
                var b = new Button(() => { _slot = _slot == s ? "" : s; Fill(); }) { text = $"{Cap(s)}\n" + (piece == null ? "Empty" : Describe(piece)) };
                b.AddToClassList("btn"); b.AddToClassList("gear-slot");
                if (_slot == s) b.AddToClassList("gear-slot--picked");
                gear.Add(b);
            }

            var list = _root.Q<ScrollView>("gear-list");
            list.Clear();
            if (_slot == "") return;
            var pieces = S.Inventory.Items.Where(i => G.Gear.Type(i.Type).Slot == _slot && (i.EquippedOn == "" || i.EquippedOn == _id))
                .OrderByDescending(i => i.Rarity).ThenByDescending(i => i.Upgrade).ToList();
            if (pieces.Count == 0) list.Add(Placeholder.Label("No free pieces for this slot yet.", "gear-row__text"));
            foreach (var p in pieces)
            {
                var item = p;
                var row = new VisualElement(); row.AddToClassList("gear-row");
                row.Add(Placeholder.Label(Describe(item) + (item.EquippedOn == _id ? "  (worn)" : ""), "gear-row__text"));
                row.Add(Btn(item.EquippedOn == _id ? "Remove" : "Equip", () =>
                {
                    if (item.EquippedOn == _id) S.Inventory.Unequip(item); else S.Inventory.Equip(G.Gear, item, _id);
                    Done("");
                }));
                row.Add(Btn(item.Upgrade >= G.Gear.MaxUpgrade ? "Max" : $"+1\n{Placeholder.Short(S.Inventory.UpgradeCost(G.Gear, item))}",
                    () => Done(S.Inventory.Upgrade(G.Gear, item, S.Wallet) ? "Upgraded." : "Not enough gold, or at +20.")));
                row.Add(Btn($"Salvage\n{Placeholder.Short(S.Inventory.SalvageValue(G.Gear, item))}", () => { S.Inventory.Salvage(G.Gear, item, S.Wallet); Done("Salvaged for gold."); }));
                list.Add(row);
            }
        }

        static Button Btn(string text, System.Action onClick) { var b = new Button(onClick) { text = text }; b.AddToClassList("btn"); return b; }

        string Describe(GearItem i) =>
            $"{Cap(G.Gear.Rarity(i.Rarity).Id)} {Cap(i.Type)} ({Cap(i.Set)}){(i.Upgrade > 0 ? $" +{i.Upgrade}" : "")}";

        static string Cap(string s) => string.IsNullOrEmpty(s) ? s : char.ToUpperInvariant(s[0]) + s.Substring(1);
    }
}
