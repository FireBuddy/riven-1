using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Championship_Riven
{
    class Program
    {
        public static int CountQ;
        public static Text Status;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Riven")
                return;

            RivenMenu.Load();
            Riven.Load();
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (RivenMenu.CheckBox(RivenMenu.Draw, "DrawOFF"))
                return;

            if (RivenMenu.CheckBox(RivenMenu.Draw, "DrawQ"))
            {
                if (Riven.Q.IsReady())
                {
                    Circle.Draw(Color.DarkBlue, Riven.Q.Range, Player.Instance.Position);
                }
            }

            if (RivenMenu.CheckBox(RivenMenu.Draw, "DrawW"))
            {
                if (Riven.W.IsReady())
                {
                    Circle.Draw(Color.DarkBlue, Riven.W.Range, Player.Instance.Position);
                }
            }

            if (RivenMenu.CheckBox(RivenMenu.Draw, "DrawE"))
            {
                if (Riven.E.IsReady())
                {
                    Circle.Draw(Color.DarkBlue, Riven.E.Range, Player.Instance.Position);
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
    }
}
