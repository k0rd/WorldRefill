using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Hooks;
using TShockAPI;
using Terraria;
namespace WorldRefill
{
    [APIVersion(1, 11)]
    public class WorldRefill : TerrariaPlugin
    {
        public WorldRefill(Main game)
            : base(game)
        {
        }
        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("causeevents", DoCrystals, "gencrystals")); //Life Crystals
            Commands.ChatCommands.Add(new Command("causeevents", DoPots, "genpots"));         //Pots
            Commands.ChatCommands.Add(new Command("causeevents", DoOrbs, "genorbs"));         //Orbs
            Commands.ChatCommands.Add(new Command("causeevents", DoAltars, "genaltars"));     //Demon Altars
            Commands.ChatCommands.Add(new Command("causeevents", DoTraps, "gentraps"));       //Traps
            Commands.ChatCommands.Add(new Command("causeevents", DoStatues, "genstatues"));   //Statues
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
            base.Dispose(disposing);
        }

        public override Version Version
        {
            get { return new Version("1.0"); }
        }
        public override string Name
        {
            get { return "World Refill Plugin"; }
        }
        public override string Author
        {
            get { return "by k0rd"; }
        }
        public override string Description
        {
            get { return "Refill your world!"; }
        }
        private void DoCrystals(CommandArgs args)
        {

            if (args.Parameters.Count == 1)
            {
                var mCry = Int32.Parse(args.Parameters[0]);
                var surface = Main.worldSurface;
                var trycount = 0;
                const int maxtries = 1000000;
                var realcount = 0;

                while (trycount < maxtries)
                {
                    if (WorldGen.AddLifeCrystal(WorldGen.genRand.Next(1, Main.maxTilesX), WorldGen.genRand.Next((int)(surface + 20.0), Main.maxTilesY)))
                    {
                        realcount++;
                        if (realcount == mCry) break;
                    }
                    trycount++;
                }
                args.Player.SendMessage(string.Format("Generated and hid {0} Life Crystals.",realcount));

            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /gencrystals (number of crystals to generate)"));
            }
            
        }

        private void DoPots(CommandArgs args)
        {
            if (args.Parameters.Count == 1)
            {

                var mPot = Int32.Parse(args.Parameters[0]);
                var surface = Main.worldSurface;
                var trycount = 0;
                const int maxtries = 1000000;
                var realcount = 0;
                while (trycount < maxtries)
                {
                    var tryX = WorldGen.genRand.Next(1, Main.maxTilesX);
                    var tryY = WorldGen.genRand.Next((int) surface - 10, Main.maxTilesY);
                        if (WorldGen.PlacePot(tryX,tryY, 28))
                        {
                            realcount++;
                            if (realcount == mPot)
                                break;
                        }
                        trycount++;

                }
                args.Player.SendMessage(string.Format("Generated and hid {0} Pots.", realcount));
            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /genpots (number of pots to generate)"));
            }
        }

        private void DoOrbs(CommandArgs args)
        {
            if (args.Parameters.Count == 1)
            {

                var mOrb = Int32.Parse(args.Parameters[0]);
                var surface = Main.worldSurface;
                var trycount = 0;
                const int maxtries = 1000000;
                var realcount = 0;
                while (trycount < maxtries)
                {
                    var tryX = WorldGen.genRand.Next(1, Main.maxTilesX);
                    var tryY = WorldGen.genRand.Next((int)surface + 20, Main.maxTilesY);

                    if ((!Main.tile[tryX, tryY].active) && (Main.tile[tryX, tryY].wall == (byte)3))
                    {
                        WorldGen.AddShadowOrb(tryX, tryY);
                        if (Main.tile[tryX, tryY].type == 31)
                        {
                            realcount++;
                            if (realcount == mOrb)
                                break;
                        }
                    }
                    trycount++;
                }
                args.Player.SendMessage(string.Format("Generated and hid {0} Orbs.", realcount));
            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /genorbs (number of orbs to generate)"));
            }
        }
        private void DoAltars(CommandArgs args)
        {
            if (args.Parameters.Count == 1)
            {

                var mAltar = Int32.Parse(args.Parameters[0]);
                var surface = Main.worldSurface;
                var trycount = 0;
                const int maxtries = 1000000;
                var realcount = 0;
                while (trycount < maxtries)
                {
                    var tryX = WorldGen.genRand.Next(1, Main.maxTilesX);
                    var tryY = WorldGen.genRand.Next((int)surface + 10, Main.maxTilesY);

                        WorldGen.Place3x2(tryX, tryY, 26);
                        if (Main.tile[tryX, tryY].type == 26)
                        {
                            realcount++;
                            if (realcount == mAltar)
                                break;
                        }
                    
                    trycount++;
                }
                args.Player.SendMessage(string.Format("Generated and hid {0} Demon Altars.", realcount));
            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /genaltars (number of Demon Altars to generate)"));
            }
        }
        private void DoTraps(CommandArgs args)
        {
            if (args.Parameters.Count == 1)
            {
                args.Player.SendMessage("Generating traps.. this may take a while..");
                var mTrap = Int32.Parse(args.Parameters[0]);
                var surface = Main.worldSurface;
                var trycount = 0;
                const int maxtries = 4000;
                var realcount = 0;
                while (trycount < maxtries)
                {
                    var tryX = WorldGen.genRand.Next(200, Main.maxTilesX -200);
                    var tryY = WorldGen.genRand.Next((int)surface, Main.maxTilesY -300);

                  
                    if (Main.tile[tryX, tryY].wall == 0 && WorldGen.placeTrap(tryX, tryY, -1))
                    {
                        realcount++;
                        if (realcount == mTrap)
                            break;
                    }

                    trycount++;
                }
                args.Player.SendMessage(string.Format("Generated and hid {0} traps.", realcount));
            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /gentraps (number of Traps to generate)"));
            }
        }
        private void DoStatues(CommandArgs args)
        {
            if (args.Parameters.Count == 1)
            {
                args.Player.SendMessage("Generating statues.. this may take a while..");
                var mStatue = Int32.Parse(args.Parameters[0]);
                var surface = Main.worldSurface;
                var trycount = 0;
                const int maxtries = 100000;
                var realcount = 0;
                while (trycount < maxtries)
                {
                    var tryX = WorldGen.genRand.Next(20, Main.maxTilesX -20);
                    var tryY = WorldGen.genRand.Next((int)surface + 20, Main.maxTilesY -300);
                    var tryType = WorldGen.genRand.Next((int) 2, 44);
                   
                    while (!Main.tile[tryX, tryY].active)
                    {
                        tryY++;
                    }
                    tryY--;

                    if (tryY < Main.maxTilesY - 300)
                    {

                        WorldGen.PlaceTile(tryX, tryY, 105, true, true, -1, tryType);

                        if (Main.tile[tryX, tryY].type == 105)
                        {
                            realcount++;
                            if (realcount == mStatue)
                                break;
                        }
                    }
                    trycount++;
                }
                args.Player.SendMessage(string.Format("Generated and hid {0} Statues.", realcount));
            }
            else if (args.Parameters.Count == 2)
            {
                List<string> types = new List<string>();

                types.Add("Armor");
                types.Add("Angel");
                types.Add("Star");
                types.Add("Sword");
                types.Add("Slime");
                types.Add("Goblin");
                types.Add("Shield");
                types.Add("Bat");
                types.Add("Fish");
                types.Add("Bunny");
                types.Add("Skeleton");
                types.Add("Reaper");
                types.Add("Woman");
                types.Add("Imp");
                types.Add("Gargoyle");
                types.Add("Gloom");
                types.Add("Hornet");
                types.Add("Bomb");
                types.Add("Crab");
                types.Add("Hammer");
                types.Add("Potion");
                types.Add("Spear");
                types.Add("Cross");
                types.Add("Jellyfish");
                types.Add("Bow");
                types.Add("Boomerang");
                types.Add("Boot");
                types.Add("Chest");
                types.Add("Bird");
                types.Add("Axe");
                types.Add("Corrupt");
                types.Add("Tree");
                types.Add("Anvil");
                types.Add("Pickaxe");
                types.Add("Mushroom");
                types.Add("Eyeball");
                types.Add("Pillar");
                types.Add("Heart");
                types.Add("Pot");
                types.Add("Sunflower");
                types.Add("King");
                types.Add("Queen");
                types.Add("Piranha");
                types.Add("Ukown");

                string mReqs = args.Parameters[1].ToLower();
                var mStatue = Int32.Parse(args.Parameters[0]);
                var surface = Main.worldSurface;
                var trycount = 0;
                const int maxtries = 100000;
                var realcount = 0;
                int stid = 0;
                string found="unknown type!";
                foreach (string ment in types)
                {
                    found = ment.ToLower();
                    if (found.StartsWith(mReqs))
                    {
                        break;
                    }
                    stid++;
                }
                if (stid < 44)
                {


                    while (trycount < maxtries)
                    {
                        var tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                        var tryY = WorldGen.genRand.Next((int) surface + 20, Main.maxTilesY - 300);

                        while (!Main.tile[tryX, tryY].active)
                        {
                            tryY++;
                        }
                        tryY--;

                        if (tryY < Main.maxTilesY - 300)
                        {

                            WorldGen.PlaceTile(tryX, tryY, 105, true, true, -1, stid);

                            if (Main.tile[tryX, tryY].type == 105)
                            {
                                realcount++;
                                if (realcount == mStatue)
                                    break;
                            }
                        }
                        trycount++;
                    }
                    args.Player.SendMessage(string.Format("Generated and hid {0} {1} ({2})Statues.", realcount, found, stid));
                }
                else
                {
                    args.Player.SendMessage(string.Format("Couldn't find a match for {0}.", mReqs));
                }
            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /genstatues (number of statues to generate) [(optional)name of statue]"));
            }
        }

    }
}