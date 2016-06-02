using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;

namespace Championship_Riven
{
    class Riven
    {
        
        
        public static int LastCastW;
        public static int LastCastQ;
        public static int CountQ;
        public static int LastQ;
        public static int LastW;
        public static int LastE;

        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Active R;
        public static Spell.Skillshot R2;
        public static Spell.Targeted Flash;

        public static Item Hydra;
        public static Item Tiamat;
        public static Item Youmu;
        public static Item Qss;
        public static Item Mercurial;

        public static AIHeroClient FocusTarget;

        public static void Load()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 275, SkillShotType.Circular, 250, 2200, 100);
            W = new Spell.Active(SpellSlot.W, 250);
            E = new Spell.Skillshot(SpellSlot.E, 310, SkillShotType.Linear);
            R = new Spell.Active(SpellSlot.R);
            R2 = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Cone, 250, 1600, 125);

            if (Player.Instance.Spellbook.GetSpell(SpellSlot.Summoner1).Name == "SummonerFlash")
            {
                Flash = new Spell.Targeted(SpellSlot.Summoner1, 425);
            }
            else if (Player.Instance.Spellbook.GetSpell(SpellSlot.Summoner2).Name == "SummonerFlash")
            {
                Flash = new Spell.Targeted(SpellSlot.Summoner2, 425);
            }

            Hydra = new Item((int)ItemId.Ravenous_Hydra, 350);
            Tiamat = new Item((int)ItemId.Tiamat, 350);
            Youmu = new Item((int)ItemId.Youmuus_Ghostblade, 0);
            Qss = new Item((int)ItemId.Quicksilver_Sash, 0);
            Mercurial = new Item((int)ItemId.Mercurial_Scimitar, 0);

            
            DamageIndicator.Initialize(DamageTotal);
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Orbwalker.OnPreAttack += BeforeAttack;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (RivenMenu.CheckBox(RivenMenu.Draw, "DrawOFF"))
                return;

            if (RivenMenu.CheckBox(RivenMenu.Draw, "DrawQ"))
            {
                if (CountQ == 0 && Riven.Q.IsReady())
                {
                    Circle.Draw(Color.Aqua, Riven.Q.Range, Player.Instance.Position);
                }
                if (CountQ == 1 && Riven.Q.IsReady())
                {
                    Circle.Draw(Color.Yellow, Riven.Q.Range, Player.Instance.Position);
                }
                if (CountQ == 2 && Riven.Q.IsReady())
                {
                    Circle.Draw(Color.Red, Riven.Q.Range, Player.Instance.Position);
                }
            }

            if (RivenMenu.CheckBox(RivenMenu.Draw, "DrawW"))
            {
                if (Riven.W.IsReady())
                {
                    Circle.Draw(Color.White, Riven.W.Range, Player.Instance.Position);
                }
            }

            if (RivenMenu.CheckBox(RivenMenu.Draw, "DrawE"))
            {
                if (Riven.E.IsReady())
                {
                    Circle.Draw(Color.Black, Riven.E.Range, Player.Instance.Position);
                }
            }

            if (RivenMenu.CheckBox(RivenMenu.Draw, "DrawR"))
            {
                if (Riven.R.IsReady())
                {
                    Circle.Draw(Color.DarkBlue, Riven.R2.Range, Player.Instance.Position);
                }
            }

            if(RivenMenu.Keybind(RivenMenu.Burst, "BurstAllowed"))
            {
                if (Riven.FocusTarget != null)
                {
                    Circle.Draw(Color.DarkBlue, 150, Riven.FocusTarget.Position);
                }
            }

            if(RivenMenu.Keybind(RivenMenu.Burst, "BurstAllowed"))
            {
                Circle.Draw(Color.Red, 800, Player.Instance.Position);
            }
        }

        private static void BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (ObjectManager.Player.Level <= 1)
            {
                
                var lasthitmode = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);

                if (!lasthitmode || RivenMenu.CheckBox(RivenMenu.Jungleclear, "UseWJG") )
                {
                    return;
                }


                else
                {
                    {
                        args.Process = false;
                    }
                }
            }

        }
        

       
  
          

        private static bool HasHydra()
        {
            if (!Hydra.IsOwned() && !RivenMenu.CheckBox(RivenMenu.Items, "Hydra"))
                return false;

            if (Hydra.IsReady())
            {
                return true;
            }

            return false;
        }

        private static bool HasTiamat()
        {
            if (!Tiamat.IsOwned() && !RivenMenu.CheckBox(RivenMenu.Items, "Tiamat"))
                return false;

            if (Tiamat.IsReady())
            {
                return true;
            }

            return false;
        }

        private static bool HasYoumu()
        {
            if (!Youmu.IsOwned() && !RivenMenu.CheckBox(RivenMenu.Items, "Youmu"))
                return false;

            if (Youmu.IsReady())
            {
                return true;
            }

            return false;
        }

        private static bool HasQss()
        {
            if (!Qss.IsOwned() && !RivenMenu.CheckBox(RivenMenu.Items, "Qss"))
                return false;

            if (Qss.IsReady())
            {
                return true;
            }

            return false;
        }

        private static bool HasMercurial()
        {
            if (!Mercurial.IsOwned() && !RivenMenu.CheckBox(RivenMenu.Items, "Qss"))
                return false;

            if (Mercurial.IsReady())
            {
                return true;
            }

            return false;
        }

        private static bool CheckUlt()
        {
            if (Player.Instance.HasBuff("RivenFengShuiEngine"))
            {
                return true;
            }
            return false;
        }

        private static void Burst()
        {
            if (FocusTarget.Health == 0)
                return;

            if(RivenMenu.ComboBox(RivenMenu.Burst, "BurstType") == 0)
            {
                if (DamageTotal(FocusTarget) >= FocusTarget.Health)
                {
                    if (FocusTarget.IsValidTarget(800))
                    {
                        switch(RivenMenu.Slider(RivenMenu.Burst, "BurstStyle"))
                        {
                            case 1:

                                if(E.IsReady())
                                {
                                    Player.CastSpell(SpellSlot.E, FocusTarget.Position);
                                }

                                if (Flash.IsReady())
                                {
                                    Flash.Cast(FocusTarget.Position);
                                }

                                if(R.IsReady() && !CheckUlt())
                                {
                                    R.Cast();
                                }

                                if (FocusTarget.IsValidTarget(Hydra.Range))
                                {
                                    if (HasTiamat())
                                    {
                                        Tiamat.Cast();
                                    }

                                    if (HasHydra())
                                    {
                                        Hydra.Cast();
                                    }
                                }

                                if(W.IsReady())
                                {
                                    if(FocusTarget.IsValidTarget(W.Range))
                                    {
                                        W.Cast();
                                    }
                                }

                                break;

                            case 2:

                                if (E.IsReady())
                                {
                                    Player.CastSpell(SpellSlot.E, FocusTarget.Position);
                                }

                                if (R.IsReady() && !CheckUlt())
                                {
                                    R.Cast();
                                }

                                if (Flash.IsReady())
                                {
                                    Flash.Cast(FocusTarget.Position);
                                }

                                if (FocusTarget.IsValidTarget(Hydra.Range))
                                {
                                    if (HasTiamat())
                                    {
                                        Tiamat.Cast();
                                    }

                                    if (HasHydra())
                                    {
                                        Hydra.Cast();
                                    }
                                }

                                if (W.IsReady())
                                {
                                    if (FocusTarget.IsValidTarget(W.Range))
                                    {
                                        W.Cast();
                                    }
                                }

                                break;
                        }
                    }
                }
            }
        }
        
        private static void Flee()
        {
            if(RivenMenu.CheckBox(RivenMenu.Flee, "UseQFlee"))
            {
                Q.Cast((Game.CursorPos.Distance(Player.Instance) > Q.Range ? Player.Instance.Position.Extend(Game.CursorPos, Q.Range - 1).To3D() : Game.CursorPos));
            }

            if(RivenMenu.CheckBox(RivenMenu.Flee, "UseEFlee"))
            {
                E.Cast((Game.CursorPos.Distance(Player.Instance) > E.Range ? Player.Instance.Position.Extend(Game.CursorPos, E.Range - 1).To3D() : Game.CursorPos));
            }
        }

        private static void ChooseR(AIHeroClient Target)
        {
            switch(RivenMenu.ComboBox(RivenMenu.Combo, "UseRType"))
            {
                case 0:

                    if (Target.HealthPercent <= 40)
                    {
                        if (RivenMenu.CheckBox(RivenMenu.Misc, "BrokenAnimations"))
                        {
                            if (W.IsReady() && RivenMenu.CheckBox(RivenMenu.Combo, "UseWCombo"))
                            {
                                if (Target.IsValidTarget(W.Range))
                                {
                                    R.Cast();
                                    W.Cast();
                                }
                            }
                            else if (E.IsReady() && RivenMenu.CheckBox(RivenMenu.Combo, "UseECombo"))
                            {
                                if (Target.IsValidTarget(E.Range))
                                {
                                    R.Cast();
                                    Player.CastSpell(SpellSlot.E, Target.Position);
                                }
                            }
                        }
                        else
                        {
                            R.Cast();
                        }
                    }

                    break;

                case 1:

                    if (DamageTotal(Target) >= Target.Health)
                    {
                        if (RivenMenu.CheckBox(RivenMenu.Misc, "BrokenAnimations"))
                        {
                            if (W.IsReady() && RivenMenu.CheckBox(RivenMenu.Combo, "UseWCombo"))
                            {
                                if (Target.IsValidTarget(W.Range))
                                {
                                    R.Cast();
                                    W.Cast();
                                }
                            }
                            else if (E.IsReady() && RivenMenu.CheckBox(RivenMenu.Combo, "UseECombo"))
                            {
                                if (Target.IsValidTarget(E.Range))
                                {
                                    R.Cast();
                                    Player.CastSpell(SpellSlot.E, Target.Position);
                                }
                            }
                        }
                        else
                        {
                            R.Cast();
                        }
                    }

                    break;

                case 2:

                    if (RivenMenu.CheckBox(RivenMenu.Misc, "BrokenAnimations"))
                    {
                        if (W.IsReady() && RivenMenu.CheckBox(RivenMenu.Combo, "UseWCombo"))
                        {
                            if (Target.IsValidTarget(W.Range))
                            {
                                R.Cast();
                                W.Cast();
                            }
                        }
                        else if (E.IsReady() && RivenMenu.CheckBox(RivenMenu.Combo, "UseECombo"))
                        {
                            if (Target.IsValidTarget(E.Range))
                            {
                                R.Cast();
                                Player.CastSpell(SpellSlot.E, Target.Position);
                            }
                        }
                    }
                    else
                    {
                        R.Cast();
                    }

                    break;

                case 3:

                    if (RivenMenu.Keybind(RivenMenu.Combo, "ForceR"))
                    {
                        R.Cast();
                    }

                    break;
            }
        }
        private static void ChooseR2(AIHeroClient Target)
        {
            switch(RivenMenu.ComboBox(RivenMenu.Combo, "UseR2Type"))
            {
                case 0:


                    if (Target.IsValidTarget(R2.Range))
                    {
                        if (RDamage(Target,Target.Health) * 0.80 >= Target.Health)
                        {
                            var RPred = R2.GetPrediction(FocusTarget);

                            if (RPred.HitChance >= HitChance.High)
                            {
                                R2.Cast(RPred.UnitPosition);
                            }
                        }
                    }

                    break;

                case 1:

                    if (FocusTarget.IsValidTarget(R2.Range))
                    {
                        var RPred = R2.GetPrediction(FocusTarget);

                        if(RPred.HitChance >= HitChance.High)
                        {
                            R2.Cast(RPred.UnitPosition);
                        }
                    }

                    break;
            }
        }

        private static void Combo()
        {
            var Target = TargetSelector.GetTarget(R2.Range, DamageType.Physical);

            if (Target != null)
            {
                if (R.IsReady())
                {
                    if (CheckUlt() == false)
                    {
                        if(Target.HealthPercent >= RivenMenu.Slider(RivenMenu.Combo, "DontR1"))
                        {
                            ChooseR(Target);
                        }
                    }
                }

                if (RivenMenu.CheckBox(RivenMenu.Combo, "UseR2Combo"))
                {
                    if (CheckUlt() == true)
                    {
                        ChooseR2(Target);
                    }
                }

                if (Player.Instance.CountEnemiesInRange(Hydra.Range) > 0)
                {
                    if (HasHydra())
                    {
                        Hydra.Cast();
                    }

                    if (HasTiamat())
                    {
                        Tiamat.Cast();
                    }
                }

                if (HasYoumu())
                {
                    if (Target.Health <= RivenMenu.Slider(RivenMenu.Items, "YoumuHealth"))
                    {
                        Youmu.Cast();
                    }
                }
                
                if (CountQ == 2 && Q.IsReady())
                {
                    if (Player.Instance.IsFacing(Target) && Target.IsValidTarget(450) && Target.CanMove && !Player.HasBuff("Valor"))
                    {
                        Player.CastSpell(SpellSlot.Q, Target.Position);
                    }
                }

                if (CountQ != 2 && E.IsReady() && RivenMenu.CheckBox(RivenMenu.Combo, "UseECombo"))
                {
                    if (Target.IsValidTarget(E.Range) && Target.CanMove)
                    {
                        Player.CastSpell(SpellSlot.E, Target.Position);
                    }
                }

                if (W.IsReady() && RivenMenu.CheckBox(RivenMenu.Combo, "UseWCombo"))
                {
                    if (Target.IsValidTarget(W.Range) && Target.CanMove && !Player.HasBuff("Valor"))
                    {
                        Player.CastSpell(SpellSlot.W, Target.Position);
                    }
                }
                
                if (Player.Instance.IsFacing(Target) && ObjectManager.Player.Position.Distance(Target.ServerPosition) > Player.Instance.GetAutoAttackRange(Target) && ObjectManager.Player.Position.Distance(Target.ServerPosition) < 400)
                {
                    {
                        Q.Cast(Player.Instance.Position.Extend(Target.ServerPosition, 250).To3D());
                    }
                }

            }
        }
        
        private static void Harass()
        {
             
            var Target = TargetSelector.GetTarget(R2.Range, DamageType.Physical);
            
            if (Target != null)
            {
                if (Player.Instance.CountEnemiesInRange(Hydra.Range) > 0)
                {
                    if (HasHydra())
                    {
                        Hydra.Cast();
                    }

                    if (HasTiamat())
                    {
                        Tiamat.Cast();
                    }
                }

                if (HasYoumu())
                {
                    if (Target.Health <= RivenMenu.Slider(RivenMenu.Items, "YoumuHealth"))
                    {
                        Youmu.Cast();
                    }
                }
                if(Q.IsReady() && CountQ < 2)
                {
                    if(Target.IsValidTarget(Q.Range + 300) && !Target.IsDead )
                    {

                        if(Player.Instance.IsFacing(Target) && ObjectManager.Player.Position.Distance(Target.ServerPosition) > 250 )
                        {
                           
                            Q.Cast(Player.Instance.Position.Extend(Target.ServerPosition, 200).To3D());
                        }
                
                var EPos = Player.Instance.ServerPosition + (Player.Instance.ServerPosition - Target.ServerPosition);
                if (Player.Instance.IsFacing(Target) && CountQ == 2 && Q.IsReady()  && Target.IsValidTarget(400))
                {
                
                    {
                        Player.CastSpell(SpellSlot.Q, Target.Position);
                        
                        {
                           Core.DelayAction( () => Player.CastSpell(SpellSlot.E, EPos),1100);
                           Core.DelayAction( () => Player.CastSpell(SpellSlot.W), 1150);
                        }

                    }
                }

            }
        }

        private static void Laneclear()
        {
            var Minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.IsValid && !x.IsDead && x.IsValidTarget(W.Range)).OrderByDescending(x => x.MaxHealth);
            var Minions = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(Minion, Q.Range, (int)Q.Range);

            if (Minion == null)
                return;

            if(RivenMenu.CheckBox(RivenMenu.Laneclear, "UseWLane"))
            {
                if (Minions.HitNumber >= RivenMenu.Slider(RivenMenu.Laneclear, "UseWLaneMin"))
                {
                    W.Cast();
                }
            }
        }
        private static void LastHit()
        {
            var Minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, Q.Range + 300);
            
            foreach (var Minion in Minions)
            {

                if(Q.IsReady())
                {
                    if(Minion.IsValidTarget(Q.Range + 300) && !Minion.IsDead )
                    {

                        if(Player.Instance.IsFacing(Minion) && ObjectManager.Player.Position.Distance(Minion.ServerPosition) > 250 && Minion.Health - SpellQDamage(Minion, Minion.Health) * 2.4 <= 0)
                        {
                           
                            Q.Cast(Player.Instance.Position.Extend(Minion.ServerPosition, 200).To3D());
                        }
                        
                    }
                }
            }
        }  
        
        private static void Jungleclear()
        {
            {
                var Monsters = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(x => x.MaxHealth).FirstOrDefault(x => x.IsValidTarget(E.Range));
    
                if (Monsters == null)
                    return;
    
                if (RivenMenu.CheckBox(RivenMenu.Jungleclear, "UseWJG"))
                {
                    if(Monsters.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }
                }
    
                if (RivenMenu.CheckBox(RivenMenu.Jungleclear, "UseEJG"))
                {
                    if(Monsters.IsValidTarget(E.Range))
                    {
                        Player.CastSpell(SpellSlot.E, Monsters.Position);
                    }
    
                }
            }
            {
                if (ObjectManager.Player.Level <= 1)
                {
                    
                    var jminions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition, 1000, true);
                    foreach (var jungleMobs in jminions.Where(x => x.IsValidTarget(Player.Instance.AttackRange)))              
                            {
                                if (jungleMobs == null)
                                {
                                    
                                    return;
                                }
                                if (jungleMobs != null)
                                {
                                        if (jungleMobs.Name =="SRU_RedMini10.1.3" || jungleMobs.Name == "SRU_BlueMini1.1.2" || jungleMobs.Name == "SRU_BlueMini21.1.3")
                                        {
                                                
                                                Player.IssueOrder(GameObjectOrder.AttackUnit, jungleMobs);
                                        }
                                        else
                                        {
                                               
                                        }
                                }
                           }
                        
                }   
            }

        }
        

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != 0x202)
                return;

            FocusTarget = EntityManager.Heroes.Enemies.FindAll(x => x.IsValid || x.Distance(Game.CursorPos) < 3000 || x.IsVisible || x.Health > 0).OrderBy(x => x.Distance(Game.CursorPos)).FirstOrDefault();
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (RivenMenu.CheckBox(RivenMenu.Combo, "UseQCombo") && Q.IsReady())
                {
                    if (CountQ == 0 || !Orbwalker.IsAutoAttacking)
                    {
                        Q.Cast(target.Position);
                    }

                    if (CountQ == 1 || !Orbwalker.IsAutoAttacking)
                    {
                        Q.Cast(target.Position);
                    }

                    if(CountQ == 2 || !Orbwalker.IsAutoAttacking)
                    {
                        Q.Cast(target.Position);
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                if (RivenMenu.CheckBox(RivenMenu.Laneclear, "UseQLane") && Q.IsReady())
                {
                    if (CountQ == 0 || !Orbwalker.IsAutoAttacking)
                    {
                        Q.Cast(target.Position);
                    }

                    if (CountQ == 1 || !Orbwalker.IsAutoAttacking)
                    {
                        Q.Cast(target.Position);
                    }

                   // if (CountQ == 2 || !Orbwalker.IsAutoAttacking)
                    //{
                     //   Q.Cast(target.Position);
                    //}
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (RivenMenu.CheckBox(RivenMenu.Jungleclear, "UseQJG") && Q.IsReady())
                {
                    if (CountQ == 0 || !Orbwalker.IsAutoAttacking)
                    {
                        Q.Cast(target.Position);
                    }

                    if (CountQ == 1 || !Orbwalker.IsAutoAttacking)
                    {
                        Q.Cast(target.Position);
                    }

                    if (CountQ == 2 || !Orbwalker.IsAutoAttacking)
                    {
                        Q.Cast(target.Position);
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.Instance.IsDead)
                return;

            if(!Flash.IsReady())
            {
                RivenMenu.Burst["BurstAllowed"].Cast<KeyBind>().CurrentValue = false;
            }

            if(Player.Instance.HasBuffOfType(BuffType.Charm) && RivenMenu.CheckBox(RivenMenu.Items, "QssCharm"))
            {
                if(HasQss())
                {
                    Qss.Cast();
                }

                if(HasMercurial())
                {
                    Mercurial.Cast();
                }
            }else if (Player.Instance.HasBuffOfType(BuffType.Charm) && RivenMenu.CheckBox(RivenMenu.Items, "QssFear"))
            {
                if (HasQss())
                {
                    Qss.Cast();
                }

                if (HasMercurial())
                {
                    Mercurial.Cast();
                }
            }
            else if (Player.Instance.HasBuffOfType(BuffType.Charm) && RivenMenu.CheckBox(RivenMenu.Items, "QssTaunt"))
            {
                if (HasQss())
                {
                    Qss.Cast();
                }

                if (HasMercurial())
                {
                    Mercurial.Cast();
                }
            }
            else if (Player.Instance.HasBuffOfType(BuffType.Charm) && RivenMenu.CheckBox(RivenMenu.Items, "QssSuppression"))
            {
                if (HasQss())
                {
                    Qss.Cast();
                }

                if (HasMercurial())
                {
                    Mercurial.Cast();
                }
            }
            else if(Player.Instance.HasBuffOfType(BuffType.Snare) && RivenMenu.CheckBox(RivenMenu.Items, "QssSnare"))
            {
                if (HasQss())
                {
                    Qss.Cast();
                }

                if (HasMercurial())
                {
                    Mercurial.Cast();
                }
            }

            if(RivenMenu.CheckBox(RivenMenu.Misc, "Skin"))
            {
                Player.Instance.SetSkinId(RivenMenu.Slider(RivenMenu.Misc, "SkinID"));
            }

            if(Player.Instance.CountEnemiesInRange(W.Range) >=  RivenMenu.Slider(RivenMenu.Combo, "W/Auto"))
            {
                W.Cast();
            }

            if(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (RivenMenu.Keybind(RivenMenu.Burst, "BurstAllowed"))
                {
                    Burst();
                }
                else
                {
                    Combo();
                }

                if (RivenMenu.CheckBox(RivenMenu.Combo, "UseR2Combo"))
                {
                    if(RivenMenu.Keybind(RivenMenu.Burst, "BurstAllowed"))
                    {
                        if (CheckUlt() == true)
                        {
                            ChooseR2(FocusTarget);
                        }
                    }
                }
            }

            if(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }

            if(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Laneclear();
            }
            if(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }

            if(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Jungleclear();
            }
            if(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
        {
            if (!sender.IsMe) return;

            if (args.SData.Name.ToLower().Contains(Riven.W.Name.ToLower()))
            {
                LastCastW = Environment.TickCount;
                return;
            }
            if (args.SData.Name.ToLower().Contains(Riven.Q.Name.ToLower()))
            {
                LastCastQ = Environment.TickCount;
                
                Core.DelayAction(() =>
                {
                    if (!Player.Instance.IsRecalling() && CountQ <= 2)
                    {
                        Player.CastSpell(SpellSlot.Q,
                            Orbwalker.LastTarget != null && Orbwalker.LastAutoAttack - Environment.TickCount < 3000
                                ? Orbwalker.LastTarget.Position
                                : Game.CursorPos);
                    }
                }, 3480);
                return;
            }
        }    
        
        {
            if (sender.IsMe || sender.IsAlly || sender == null)
                return;

            var EPos = Player.Instance.ServerPosition + (Player.Instance.ServerPosition - sender.ServerPosition);

            if(Player.Instance.IsValidTarget(args.SData.CastRange))
            {
                if(args.Slot == SpellSlot.Q)
                {
                    if(RivenMenu.CheckBox(RivenMenu.Shield, "E/" + sender.BaseSkinName + "/Q"))
                    {
                        if(args.SData.TargettingType == SpellDataTargetType.Unit)
                        {
                            if(Player.Instance.NetworkId == args.Target.NetworkId)
                            {
                                E.Cast(EPos);
                            }
                        }
                        else if(args.SData.TargettingType == SpellDataTargetType.SelfAoe)
                        {
                            E.Cast(EPos);
                        }
                        else
                        {
                            E.Cast(EPos);
                        }
                    }
                }

                if(args.Slot == SpellSlot.W)
                {
                    if (RivenMenu.CheckBox(RivenMenu.Shield, "E/" + sender.BaseSkinName + "/W"))
                    {
                        if (args.SData.TargettingType == SpellDataTargetType.Unit)
                        {
                            if (Player.Instance.NetworkId == args.Target.NetworkId)
                            {
                                E.Cast(EPos);
                            }
                        }
                        else if (args.SData.TargettingType == SpellDataTargetType.SelfAoe)
                        {
                            E.Cast(EPos);
                        }
                        else
                        {
                            E.Cast(EPos);
                        }
                    }
                }

        

                if(args.Slot == SpellSlot.E)
                {
                    if (RivenMenu.CheckBox(RivenMenu.Shield, "E/" + sender.BaseSkinName + "/E"))
                    {
                        if (args.SData.TargettingType == SpellDataTargetType.Unit)
                        {
                            if (Player.Instance.NetworkId == args.Target.NetworkId)
                            {
                                E.Cast(EPos);
                            }
                        }
                        else if (args.SData.TargettingType == SpellDataTargetType.SelfAoe)
                        {
                            E.Cast(EPos);
                        }
                        else
                        {
                            E.Cast(EPos);
                        }
                    }
                }

                if(args.Slot == SpellSlot.R)
                {
                    if (RivenMenu.CheckBox(RivenMenu.Shield, "E/" + sender.BaseSkinName + "/R"))
                    {
                        if (args.SData.TargettingType == SpellDataTargetType.Unit)
                        {
                            if (Player.Instance.NetworkId == args.Target.NetworkId)
                            {
                                E.Cast(EPos);
                            }
                        }
                        else if (args.SData.TargettingType == SpellDataTargetType.SelfAoe)
                        {
                            E.Cast(EPos);
                        }
                        else
                        {
                            E.Cast(EPos);
                        }
                    }
                }
            }
        }
        }


        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsMe || sender.IsAlly || sender == null)
                return;

            if (!RivenMenu.CheckBox(RivenMenu.Misc, "Gapcloser"))
                return;

            if(RivenMenu.CheckBox(RivenMenu.Misc, "GapcloserW"))
            {
                if (sender.IsValidTarget(W.Range))
                {
                    if(W.IsReady())
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsMe || sender.IsAlly || sender == null)
                return;

            if (!RivenMenu.CheckBox(RivenMenu.Misc, "Interrupter"))
                return;

            if(RivenMenu.CheckBox(RivenMenu.Misc, "InterrupterW"))
            {
                if(sender.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
            }
        }

        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe)
                return;

            var T = 0;

            switch(args.Animation)
            {
                case "Spell1a":

                    LastQ = Core.GameTickCount;
                    CountQ = 1;
                    T = 291;

                    break;

                case "Spell1b":

                    LastQ = Core.GameTickCount;
                    CountQ = 2;
                    T = 291;

                    break;

                case "Spell1c":

                    LastQ = 0;
                    CountQ = 0;
                    T = 393;

                    break;

                case "Spell2":
                    T = 170;

                    break;

                case "Spell3":

                    break;
                case "Spell4a":
                    T = 0;

                    break;
                case "Spell4b":
                    T = 150;

                    break;
            }

            if(T != 0)
            {
                if(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Orbwalker.ResetAutoAttack();
                    Core.DelayAction(CancelAnimation, T - Game.Ping);
                }
                else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    Orbwalker.ResetAutoAttack();
                    Core.DelayAction(CancelAnimation, T - Game.Ping);
                }
            }
        }

        private static void CancelAnimation()
        {
            Player.DoEmote(Emote.Dance);
            Orbwalker.ResetAutoAttack();
        }

        public static float DamageTotal(AIHeroClient target)
        {
            double dmg = 0;
            var passiveStacks = 0;

            dmg += Q.IsReady()
                ? QDamage(!CheckUlt()) * (3 - CountQ)
                : 0;
            passiveStacks += Q.IsReady()
                ? (3 - CountQ)
                : 0;

            dmg += W.IsReady()
                ? WDamage()
                : 0;
            passiveStacks += W.IsReady()
                ? 1
                : 0;
            passiveStacks += E.IsReady()
                ? 1
                : 0;

            dmg += PassiveDamage() * passiveStacks;
            dmg += (R.IsReady() && !CheckUlt() && !Player.Instance.HasBuff("RivenFengShuiEngine")
                ? Player.Instance.TotalAttackDamage * 1.2
                : Player.Instance.TotalAttackDamage) * passiveStacks;

            if (dmg < 10)
            {
                return 0 * Player.Instance.TotalAttackDamage;
            }

            dmg += R.IsReady() && !CheckUlt()
                ? RDamage(target, Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float)dmg))
                : 0;
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float)dmg);
        }

        public static float QDamage(bool useR = false)
        {
            return (float)(new double[] { 10, 30, 50, 70, 90 }[Q.Level - 1] +
                            ((Riven.R.IsReady() && useR && !Player.Instance.HasBuff("RivenFengShuiEngine")
                                ? Player.Instance.TotalAttackDamage * 1.2
                                : Player.Instance.TotalAttackDamage) / 100) *
                            new double[] { 40, 45, 50, 55, 60 }[Q.Level - 1]);
        }

        public static float WDamage()
        {
            return (float)(new double[] { 50, 80, 110, 140, 170 }[W.Level - 1] +
                            1 * ObjectManager.Player.FlatPhysicalDamageMod);
        }

        public static double PassiveDamage()
        {
            return ((20 + ((Math.Floor((double)ObjectManager.Player.Level / 3)) * 5)) / 100) *
                   (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod);
        }

        public static float RDamage(Obj_AI_Base target, float healthMod = 0f)
        {
            if (target != null)
            {
                float missinghealth = (target.MaxHealth - healthMod) / target.MaxHealth > 0.75f ? 0.75f : (target.MaxHealth - healthMod) / target.MaxHealth;
                float pluspercent = missinghealth * (8f / 3f);
                var rawdmg = new float[] { 80, 120, 160 }[R.Level - 1] + 0.6f * Player.Instance.FlatPhysicalDamageMod;
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, rawdmg * (1 + pluspercent));
            }
            return 0f;
        }
        public static float SpellQDamage(Obj_AI_Base target, float healthMod = 0f)
        {
            if (target != null)
            {
                float missinghealth = (target.MaxHealth - healthMod) / target.MaxHealth > 0.75f ? 0.75f : (target.MaxHealth - healthMod) / target.MaxHealth;
                float pluspercent = missinghealth * (8f / 3f);
                var rawdmg = new float[]  { 10, 30, 50, 70, 90 }[Q.Level - 1] + 0.4f * Player.Instance.FlatPhysicalDamageMod;
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, rawdmg);
            }
            return 0f;
        }
    }
}
