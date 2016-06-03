using System.Reflection;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Championship_Riven
{
    class RivenMenu
    {
        public static Menu Principal, Combo, Burst, Shield, Items, Laneclear, Jungleclear, Flee, Misc, Draw, Killsteal;

        public static void Load()
        {
            Principal = MainMenu.AddMenu("Championship Riven", "Riven");
            Principal.AddLabel("Championship Riven v" + Assembly.GetExecutingAssembly().GetName().Version);
            Principal.AddSeparator(2);
            Principal.AddLabel("Good game !");

            Combo = Principal.AddSubMenu("Combo", "Combo");
            Combo.AddSeparator(3);
            Combo.AddLabel("• Spells Combo");
            Combo.Add("UseQCombo", new CheckBox("Use Q?"));
            Combo.Add("UseWCombo", new CheckBox("Use W?"));
            Combo.Add("UseECombo", new CheckBox("Use E"));
            Combo.Add("UseRCombo", new CheckBox("Use R?"));
            Combo.Add("UseR2Combo", new CheckBox("Use R2?"));
            Combo.AddSeparator(3);
            Combo.AddLabel("• Spell W");
            Combo.Add("W/Auto", new Slider("Auto W if {0} Enemies <=", 2, 1, 5));
            Combo.AddSeparator(3);
            Combo.AddLabel("• Spell R");
            Combo.Add("UseRType", new ComboBox("Use R when", 1, "Normal Kill", "Hard Kill", "Always", "ForceR"));
            Combo.Add("ForceR", new KeyBind("Force R", false, KeyBind.BindTypes.PressToggle, 'U'));
            Combo.Add("DontR1", new Slider("Dont R if Target HP {0}% <=", 25, 10, 50));
            Combo.AddSeparator(3);
            Combo.AddLabel("• Spell R2");
            Combo.Add("UseR2Type", new ComboBox("Use R2 when", 0, "Kill only", "Max damage"));

            Shield = Principal.AddSubMenu("Shield", "Shield");
            Shield.AddLabel("• Spell E");
            foreach(var Enemy in EntityManager.Heroes.Enemies)
            {
                Shield.AddLabel(Enemy.ChampionName);
                Shield.Add("E/" + Enemy.BaseSkinName + "/Q", new CheckBox(Enemy.ChampionName + " (Q)", false));
                Shield.Add("E/" + Enemy.BaseSkinName + "/W", new CheckBox(Enemy.ChampionName + " (W)", false));
                Shield.Add("E/" + Enemy.BaseSkinName + "/E", new CheckBox(Enemy.ChampionName + " (E)", false));
                Shield.Add("E/" + Enemy.BaseSkinName + "/R", new CheckBox(Enemy.ChampionName + " (R)", false));
                Shield.AddSeparator(1);
            }

            Burst = Principal.AddSubMenu("Burst", "Burst");
            Burst.AddLabel("• Burst");
            Burst.AddLabel("The combo burst key is the Combo !");
            Burst.AddLabel("This 'Burst allowed' option is just to confirm that you want to use the Burst");
            Burst.AddSeparator(2);
            Burst.Add("BurstAllowed", new KeyBind("Burst Allowed ?", false, KeyBind.BindTypes.PressToggle, 'T'));
            Burst.Add("BurstType", new ComboBox("Burst:", 0, "Damage Check", "Always"));
            Burst.AddSeparator(2);
            Burst.AddLabel("Select Burst style");
            Burst.AddLabel("Style Burst 1: E > Flash > R > W > Hydra > R2");
            Burst.AddLabel("Style Burst 2: E > R > Flash > W > Hydra > R2");
            Burst.AddSeparator(1);
            Burst.Add("BurstStyle", new Slider("Burst style", 1, 1, 2));

            Items = Principal.AddSubMenu("Items", "Items");
            Items.AddLabel("• Hydra Logic");
            Items.Add("Hydra", new CheckBox("Use Hydra?"));
            Items.Add("HydraReset", new CheckBox("Use hydra to reset your AA"));
            Items.AddSeparator(3);
            Items.AddLabel("• Tiamat Logic");
            Items.Add("Tiamat", new CheckBox("Use Tiamat?"));
            Items.Add("TiamatReset", new CheckBox("Use the Tiamat to reset your AA"));
            Items.AddSeparator(3);
            Items.AddLabel("• Qss / Mercurial Logic");
            Items.Add("Qss", new CheckBox("Use Qss?"));
            Items.Add("QssCharm", new CheckBox("Use Qss because of charm"));
            Items.Add("QssFear", new CheckBox("Use Qss because of fear"));
            Items.Add("QssTaunt", new CheckBox("Use Qss because of taunt"));
            Items.Add("QssSuppression", new CheckBox("Use Qss because of suppression"));
            Items.Add("QssSnare", new CheckBox("Use Qss because of snare"));
            Items.AddSeparator(3);
            Items.AddLabel("• Youmu Logic");
            Items.Add("Youmu", new CheckBox("Use Youmu?"));
            Items.Add("YoumuHealth", new Slider("Use Youmu if the enemy has less than {0} HP", 65, 25, 100));

            Laneclear = Principal.AddSubMenu("Laneclear", "Laneclear");
            Laneclear.Add("UseQLane", new CheckBox("Use Q"));
            Laneclear.Add("UseWLane", new CheckBox("Use W"));
            Laneclear.Add("UseWLaneMin", new Slider("Use W if you hit {0} minions", 3, 0, 10));

            Jungleclear = Principal.AddSubMenu("Jungleclear", "Jungleclear");
            Jungleclear.Add("UseQJG", new CheckBox("Use Q"));
            Jungleclear.Add("UseWJG", new CheckBox("Use W"));
            Jungleclear.Add("UseEJG", new CheckBox("Use E"));
            Jungleclear.Add("Level_1 JungleClearing", new CheckBox("only attack small Red/BLUE"));

            Flee = Principal.AddSubMenu("Flee", "Flee");
            Flee.Add("UseQFlee", new CheckBox("Use Q"));
            Flee.Add("UseEFlee", new CheckBox("Use E"));

            Misc = Principal.AddSubMenu("Misc", "Misc");
            Misc.Add("Skin", new CheckBox("Skinhack ?", false));
            Misc.Add("SkinID", new Slider("Skin ID: {0}", 4, 0, 6));
            Misc.Add("Interrupter", new CheckBox("Interrupter ?"));
            Misc.Add("InterrupterW", new CheckBox("Interrupter with W ?"));
            Misc.Add("Gapcloser", new CheckBox("Gapcloser ?"));
            Misc.Add("GapcloserW", new CheckBox("Use W on Gapcloser ?"));
            Misc.Add("BrokenAnimations", new CheckBox("Broken Animations ?"));

            Draw = Principal.AddSubMenu("Drawing", "Drawing");
            Draw.Add("DrawQ", new CheckBox("Draw Q"));
            Draw.Add("DrawW", new CheckBox("Draw W"));
            Draw.Add("DrawE", new CheckBox("Draw E"));
            Draw.Add("DrawR", new CheckBox("Draw R2"));
            Draw.Add("DrawDamage", new CheckBox("Draw Damage"));
            Draw.Add("DrawOFF", new CheckBox("Draw OFF", false));
        }

        public static bool CheckBox(Menu m, string s)
        {
            return m[s].Cast<CheckBox>().CurrentValue;
        }

        public static int Slider(Menu m, string s)
        {
            return m[s].Cast<Slider>().CurrentValue;
        }

        public static bool Keybind(Menu m, string s)
        {
            return m[s].Cast<KeyBind>().CurrentValue;
        }

        public static int ComboBox(Menu m, string s)
        {
            return m[s].Cast<ComboBox>().SelectedIndex;
        }
    }
}
