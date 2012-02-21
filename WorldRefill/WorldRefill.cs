using System;
using System.Collections.Generic;
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
            Commands.ChatCommands.Add(new Command("causeevents", DoOres, "genores"));         //ores
            Commands.ChatCommands.Add(new Command("causeevents", DoWebs, "genwebs"));         //webs
            Commands.ChatCommands.Add(new Command("causeevents", DoMineHouse, "genhouse"));   //mine house
            Commands.ChatCommands.Add(new Command("causeevents", DoTrees, "gentrees"));       //trees
            Commands.ChatCommands.Add(new Command("causeevents", DoIsland, "genisland"));     //floating island
            Commands.ChatCommands.Add(new Command("causeevents", DoShrooms, "genpatch"));     //mushroom patch

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
        public static void InformPlayers()
        {
            foreach (TSPlayer person in TShock.Players)
            {
                if ((person != null) && (person.Active))
                {
                    person.SendMessage("The server is sending you new map data due to world restock...");
                    person.SendTileSquare(person.TileX, person.TileY, 150);
                }
            }

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
                InformPlayers();
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
                InformPlayers();
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
                InformPlayers();
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
                InformPlayers();
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
                const int maxtries = 100000;
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
                InformPlayers();
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
                InformPlayers();
            }
            else if (args.Parameters.Count == 2)
            {
                List<string> types = new List<string>
                                         {
                                             "Armor",
                                             "Angel",
                                             "Star",
                                             "Sword",
                                             "Slime",
                                             "Goblin",
                                             "Shield",
                                             "Bat",
                                             "Fish",
                                             "Bunny",
                                             "Skeleton",
                                             "Reaper",
                                             "Woman",
                                             "Imp",
                                             "Gargoyle",
                                             "Gloom",
                                             "Hornet",
                                             "Bomb",
                                             "Crab",
                                             "Hammer",
                                             "Potion",
                                             "Spear",
                                             "Cross",
                                             "Jellyfish",
                                             "Bow",
                                             "Boomerang",
                                             "Boot",
                                             "Chest",
                                             "Bird",
                                             "Axe",
                                             "Corrupt",
                                             "Tree",
                                             "Anvil",
                                             "Pickaxe",
                                             "Mushroom",
                                             "Eyeball",
                                             "Pillar",
                                             "Heart",
                                             "Pot",
                                             "Sunflower",
                                             "King",
                                             "Queen",
                                             "Piranha",
                                             "Ukown"
                                         };

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

                    args.Player.SendMessage(string.Format("Generating {0} statues.. this may take a while..", found));
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
                    InformPlayers();
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

        private static void DoOres(CommandArgs args)
        {
            if (WorldGen.genRand == null)
                WorldGen.genRand = new Random();

            TSPlayer ply = args.Player;



            int num = WorldGen.altarCount % 3;
            int num2 = WorldGen.altarCount / 3 + 1;
            float num3 = (float)(Main.maxTilesX / 4200);
            int num4 = 1 - num;
            num3 = num3 * 310f - (float)(85 * num);
            num3 *= 0.85f;
            num3 /= (float)num2;

            if (args.Parameters.Count < 1)
            {
                ply.SendMessage("Usage: /genores (type) (amount)", Color.Red);    //should this be a help message instead?
                return;
            }
            else if (args.Parameters[0].ToLower() == "cobalt")
            {
                num = 0;
            }
            else if (args.Parameters[0].ToLower() == "mythril")
            {
                num = 1;
            }
            else if (args.Parameters[0].ToLower() == "copper")
            {
                num = 3;
            }
            else if (args.Parameters[0].ToLower() == "iron")
            {
                num = 4;
            }
            else if (args.Parameters[0].ToLower() == "silver")
            {
                num = 6;
            }
            else if (args.Parameters[0].ToLower() == "gold")
            {
                num = 5;
            }
            else if (args.Parameters[0].ToLower() == "demonite")
            {
                num = 7;
            }
            else if (args.Parameters[0].ToLower() == "sapphire")
            {
                num = 8;
            }
            else if (args.Parameters[0].ToLower() == "ruby")
            {
                num = 9;
            }
            else if (args.Parameters[0].ToLower() == "emerald")
            {
                num = 10;
            }
            else if (args.Parameters[0].ToLower() == "topaz")
            {
                num = 11;
            }
            else if (args.Parameters[0].ToLower() == "amethyst")
            {
                num = 12;
            }
            else if (args.Parameters[0].ToLower() == "diamond")
            {
                num = 13;
            }
            else
            {
                num = 2;
            }

            if (num == 0)
            {
                num = 107;
                num3 *= 1.05f;
            }
            else if (num == 1)
            {
                num = 108;
            }
            else if (num == 3)
            {
                num = 7;
                num3 *= 1.1f;
            }
            else if (num == 4)
            {
                num = 6;
                num3 *= 1.1f;
            }
            else if (num == 5)
            {
                num = 8;
                num3 *= 1.1f;
            }
            else if (num == 6)
            {
                num = 9;
                num3 *= 1.1f;
            }
            else if (num == 7)
            {
                num = 22;
                num3 *= 1;
            }
            else if (num == 8)
            {
                num = 63;
                num3 *= .80f;
            }
            else if (num == 9)
            {
                num = 64;
                num3 *= 1;
            }
            else if (num == 10)
            {
                num = 65;
                num3 *= 1;
            }
            else if (num == 11)
            {
                num = 66;
                num3 *= 1;
            }
            else if (num == 12)
            {
                num = 67;
                num3 *= 1;
            }
            else if (num == 13)
            {
                num = 68;
                num3 *= 1;
            }
            else
            {
                num = 111;
            }


            if (args.Parameters.Count > 1)
            {
                float.TryParse(args.Parameters[1], out num3);
                num3 = Math.Min(num3, 10000f);
            }

            int num5 = 0;
            while ((float)num5 < num3)
            {
                int i2 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                double num6 = Main.worldSurface;
                if ((num == 108) || (num == 6) || (num == 7) || (num == 8) || (num == 9) || ((num > 62) && (num < 69)))
                {
                    num6 = Main.rockLayer;
                }
                if ((num == 111) || (num == 22) || (num == 68))
                {
                    num6 = (Main.rockLayer + Main.rockLayer + (double)Main.maxTilesY) / 3.0;
                }
                int j2 = WorldGen.genRand.Next((int)num6, Main.maxTilesY - 150);
                WorldGen.OreRunner(i2, j2, (double)WorldGen.genRand.Next(5, 9 + num4), WorldGen.genRand.Next(5, 9 + num4), num);
                num5++;
            }
            ply.SendMessage(String.Format("Spawned {0} tiles of {1}", Math.Floor(num3), num), Color.Green);
            InformPlayers();
        }
        private void DoWebs(CommandArgs args)
        {
            if (args.Parameters.Count == 1)
            {

                var mWeb = Int32.Parse(args.Parameters[0]);
                var surface = Main.worldSurface;
                var trycount = 0;
                const int maxtries = 1000000;
                var realcount = 0;
                while (trycount < maxtries)
                {
                    var tryX = WorldGen.genRand.Next(20, Main.maxTilesX-20);
                    var tryY = WorldGen.genRand.Next(150, Main.maxTilesY-300);
                    int direction = WorldGen.genRand.Next(2);
                    if (direction == 0)
                    {
                        direction = -1;
                    }
                    else
                    {
                        direction = 1;
                    }
                     while (!Main.tile[tryX, tryY].active && tryY>149)
                     {
                         tryY--;
                     }
                    tryY++;
                     while (!Main.tile[tryX, tryY].active && tryX > 10 && tryX < Main.maxTilesX - 10)
                     {
                         tryX += direction;
                     }
                    tryX -= direction;
                    
                    if ((tryY< Main.maxTilesY - 300 )&& (tryX <Main.maxTilesX - 20) && (tryX>20) && (tryY>150))
                    {

                         WorldGen.TileRunner(tryX, tryY, (double)WorldGen.genRand.Next(4, 11), WorldGen.genRand.Next(2, 4), 51, true, (float)direction, -1f, false, false);
                        realcount++;
                        if (realcount == mWeb)
                            break;
                    }
                    trycount++;

                }
                args.Player.SendMessage(string.Format("Generated and hid {0} Webs.", realcount));
                InformPlayers();
            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /genwebs (number of webs to generate)"));
            }
        }
        private void DoMineHouse(CommandArgs args)
        {
            int tryX = args.Player.TileX;
            int tryY = args.Player.TileY;
            WorldGen.MineHouse(tryX, tryY +1);
            InformPlayers();
        }
        private void DoTrees(CommandArgs args)
        {
            var counter = 0;
            while ((double)counter < (double)Main.maxTilesX * 0.003)
            {
                int tryX = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                int tryY = WorldGen.genRand.Next(25, 50);
                for (var tick = tryX - tryY; tick < tryX + tryY; tick++)
                {
                    var offset = 20;
                    while ((double)offset < Main.worldSurface)
                    {
                        WorldGen.GrowEpicTree(tick, offset);
                        offset++;
                    }
                }
                counter++;
            }
            WorldGen.AddTrees();
            args.Player.SendMessage("Enjoy your trees.",Color.Green);
            InformPlayers();
        }
        private void DoShrooms(CommandArgs args)
        {
            int tryX = args.Player.TileX;
            int tryY = args.Player.TileY;
            const int offset = 25;
            WorldGen.ShroomPatch(tryX, tryY + 1);
            for (int z = args.Player.TileX - offset; z < args.Player.TileX +offset; z++  )
            {
                for (int y = args.Player.TileY - offset; y < args.Player.TileY +offset; y++  )
                {
                    if (Main.tile[z, y].active)
                    {
                        WorldGen.SpreadGrass(z, y ,59, 70, false );

                    }
                }
            }

                InformPlayers();
            args.Player.SendMessage("Mushroom Farm generated.");
        }
        private void DoIsland(CommandArgs args)
        {
            int tryX = args.Player.TileX;
            int tryY = args.Player.TileY;
            WorldGen.FloatingIsland(tryX, tryY -50);
            const int offset = 100;
            for (int z = args.Player.TileX - offset; z < args.Player.TileX + offset; z++)
            {
                for (int y = args.Player.TileY - offset ; y < args.Player.TileY; y++)
                {
                    if (Main.tile[z, y].active)
                    {
                        WorldGen.SpreadGrass(z, y, 0, 2, false);

                    }
                }
            }
            InformPlayers();
            args.Player.SendMessage("I made you a nice little island",Color.Green);
        }


    }
}