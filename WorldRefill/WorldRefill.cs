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
            Commands.ChatCommands.Add(new Command("causeevents", DoLake, "genlake"));         //lake
            Commands.ChatCommands.Add(new Command("causeevents", DoMountain, "genmountain")); //mountain
            Commands.ChatCommands.Add(new Command("causeevents", CountEmpties, "genchests"));    //chests
            Commands.ChatCommands.Add(new Command("causeevents", DoIslandHouse, "genihouse"));    //island house
            Commands.ChatCommands.Add(new Command("causeevents", DoHV, "hellavator")); //hellevator
            Commands.ChatCommands.Add(new Command("causeevents", DoChestsWithOptions,"genchestoption");
            //The following creates the chestoptions.txt file in the same directory as TerrariaServer.exe If chestoptions.txt already exists, it does nothing.
            if (!File.Exists(path + "\\" + optiontext))
            {
                Directory.CreateDirectory(path);
                using (FileStream fs = new FileStream(optiontext, FileMode.Create))
                {
                    using (StreamWriter bw = new StreamWriter((Stream)fs))
                    {
                        bw.WriteLine("Test,1 2,2 4,5 3");
                        bw.WriteLine("Test2,1 2,3 4,5 6");
                        bw.WriteLine("Do not remove this line. This text document works as follows: <Name>,<Item ID> <Item Quantity>,<Item ID> <Item Quantity>, etc.");
                    }
                }
            }
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
        public static void InformPlayers(bool hard=false)
        {
            foreach (TSPlayer person in TShock.Players)
            {
                if ((person != null) && (person.Active))
                {
                    person.SendMessage("The server is sending you map data due to world restock...");
                    if (hard)
                    {
                        var myX = person.TileX;
                        var myy = person.TileY;
                        person.SendTileSquare(person.TileX, person.TileY, 150);
                        int count;
                        for (count = person.TileY; count < Main.maxTilesY; count += 150)
                        {
                            person.Teleport(person.TileX + 1, count);
                        }
                        person.Teleport(myX, myy);
                    }

                else
                        person.SendTileSquare(person.TileX, person.TileY, 150);
                }
            }

        }
        private void DoChestsWithOptions(CommandArgs args){
         
            if (args.Parameters.Count == 2)
            {
                string[] rawdata = File.ReadAllLines(path+"\\chestoptions.txt");
                var mChest = Int32.Parse(args.Parameters[0]);
                string option = (args.Parameters[1]);
                string[] options = new string[rawdata.Length - 1];
                int[] IDsAndQuantities = new int[0];
                for (int i = 0; i < rawdata.Length - 1; i++)
                {
                    options[i] = rawdata[i];
                }
                foreach (string s in options)
                {
                    if (s.Substring(0,s.IndexOf(",")).Equals(option))
                    {
                        string noName = s;
                        string[]subOne=noName.Split(',');
                        IDsAndQuantities=new int[(subOne.Length-1)*2];
                        int derp=0;
                        for(int i=1;i<subOne.Length;i++)
                        {
                            string[] subtwo = subOne[i].Split(' ');
                            IDsAndQuantities[derp] = Int32.Parse(subtwo[0]);
                            derp++;
                            IDsAndQuantities[derp] = Int32.Parse(subtwo[1]);
                            derp++;
                        }
                    }
                }
                foreach(int i in IDsAndQuantities)
                {
                    Console.Write(i+" ");
                }
                if (mChest < Main.maxChests)
                {
                    args.Player.SendMessage("Generating Underground Chests.. this may take a while..");
                    var surface = Main.worldSurface;
                    var trycount = 0;
                    const int maxtries = 9001;
                    var realcount = 0;
                    int totalchests = 0;
                    while (trycount < maxtries)
                    {
                        var tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                        var tryY = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 200);
                        int style = 0;
                        int ind =WorldGen.PlaceChest(tryX, tryY, 21, false, style);
                        if (ind > -1)
                        {
                            Chest c=null;
                            c = Main.chest[ind];                   
                            int chestItemIndex = 0;
                            int[] maxquantities = new int[IDsAndQuantities.Length/2];
                            int[] indexes=new int[IDsAndQuantities.Length/2];
                            for (int i = 0; i < IDsAndQuantities.Length; i++)
                            {
                                if (i % 2 == 0) indexes[i/2] = IDsAndQuantities[i];
                                if (i % 2 == 1) maxquantities[i / 2] = IDsAndQuantities[i];
                            }
                            
                            for (int i = 0; i < indexes.Length; i++)
                            {

                                    c.item[chestItemIndex].SetDefaults(indexes[i]);
                                    Random rand = new Random(); ;
                                    int quantity = 1;
                                    if (maxquantities[i] > 1)
                                        quantity = rand.Next(1, maxquantities[i]);
                                    c.item[chestItemIndex].stack = quantity;
                                    chestItemIndex++;
                                
                            }
                            realcount++;
                            if (realcount == mChest)
                            {

                                break;
                            }
                        }
                        trycount++;

                    }
                    Console.WriteLine(string.Format("Generated and hid {0} Chests.", realcount));
                    Console.WriteLine(string.Format("There are {0} chests in the world.", totalchests));
                    InformPlayers();
                }
                else
                {
                    args.Player.SendMessage("That many chests would exceed the maximum chest limit.");
                    args.Player.SendMessage("Please select a smaller number of chests.");
                }

            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /genchests <number of Chests to generate> <Option name>"));
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
        private void DoIslandHouse(CommandArgs args)
        {
            int tryX = args.Player.TileX;
            int tryY = args.Player.TileY;
            WorldGen.IslandHouse(tryX, tryY +1);
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
        private void DoLake(CommandArgs args)
        {
            int tryX = args.Player.TileX;
            int tryY = args.Player.TileY;
            const int yoffset = 20;

            WorldGen.Lakinater(tryX, tryY +yoffset);
            InformPlayers();
        }

        private void DoHV(CommandArgs args)
        {
            int meX = args.Player.TileX;
            int meY = args.Player.TileY;
            const int maxsize = 25;
            const int bump = 4;
            int cx;
            int ypos = 0;
            int start = 0;
            
            int bottom = Main.maxTilesY - 150;
            int width = 3;
            if (args.Parameters.Count==1)
                width = Int32.Parse(args.Parameters[0]);
            if (width < 2) width = 2;
            if (width > maxsize) width = maxsize;
            start = meX - (width/2);
            ypos = meY + bump;
            start--;
            width++;
            for (cx=start; cx < width + start; cx++)
            {
                int xc;
                for (xc = ypos; xc < bottom; xc++)
                {
//                   WorldGen.KillTile(cx, xc,false,false,false);
                    if ((cx == start) || (cx == width + start - 1))
                    {
                        Main.tile[cx, xc].type = 121;
                        Main.tile[cx, xc].active = true;
                    }
                    else
                    {
                        WorldGen.KillTile(cx, xc, false, false, false);
                        Main.tile[cx, xc].wall = 25;
                    }
        //            Log.ConsoleError(string.Format("pos - x: {0} y: {1}",cx,xc));
                }
            }


            InformPlayers(true);
        }

        private void DoMountain(CommandArgs args)
        {
            int tryX = args.Player.TileX;
            int tryY = args.Player.TileY;
            const int yoffset = 20;


            WorldGen.Mountinater(tryX, tryY - yoffset);
            InformPlayers();
            args.Player.SendMessage("Gotta get you out of here");

            while (tryY - yoffset >30)
            {
                tryY -= yoffset;
                if (Main.tile[tryX, tryY].active) continue;
                args.Player.Teleport(tryX, tryY + 3);
                args.Player.SendMessage("I got your back bro :)");
                break;
            }
            InformPlayers();
        }

        private void CountEmpties(CommandArgs args)
        {
            int empty = 0;
            int tmpEmpty = 0;
            const int maxtries = 100000;
            var chests = Int32.Parse(args.Parameters[0]);
            const int threshold = 100;
            for (int x = 0; x < 1000; x++)
            {
                if (Main.chest[x] != null)
                {
                    tmpEmpty++;
                    bool found = false;
                    foreach (Item itm in Main.chest[x].item)
                        if (itm.netID != 0)
                            found = true;
                    if (found == false)
                    {
                        empty++;
                  //      TShock.Utils.Broadcast(string.Format("Found chest {0} empty at x {1} y {2}", x, Main.chest[x].x,
                  //                                           Main.chest[x].y));

                        // destroying
                        WorldGen.KillTile(Main.chest[x].x, Main.chest[x].y, false, false, false);
                        Main.chest[x] = null;

                    }

                }


            }
            TShock.Utils.Broadcast(string.Format("uprooted {0} empty out of {1} chests.", empty, tmpEmpty));
            InformPlayers();
            if (chests + tmpEmpty + threshold > 1000)
                chests = 1000 - tmpEmpty - threshold;
            if (chests >0)
            {
                int chestcount = 0;
                chestcount = tmpEmpty;
                int tries = 0;
                int newcount = 0;
                while (newcount < chests)
                {
                    int contain = WorldGen.genRand.Next(1, 603);
                    int tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                    int tryY = WorldGen.genRand.Next((int) Main.worldSurface, Main.maxTilesY - 200);
                        while (!Main.tile[tryX, tryY].active)
                        {
                            tryY++;
                        }
                        tryY--;



                    WorldGen.KillTile(tryX, tryY, false, false, false);
                    WorldGen.KillTile(tryX +1 , tryY, false, false, false);
                    WorldGen.KillTile(tryX, tryY + 1, false, false, false);
                    WorldGen.KillTile(tryX + 1, tryY, false, false, false);

                      

                    if (WorldGen.AddBuriedChest(tryX, tryY, contain, false, 1))
                    {
                        chestcount++;
                        newcount++;
                    }
                    if (tries + 1 >= maxtries)
                        break;

                    tries++;
                }
                args.Player.SendMessage(string.Format("generated {0} new chests - {1} total", newcount, chestcount));
                InformPlayers();
            }
        }
    }
}