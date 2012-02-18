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


    }
}