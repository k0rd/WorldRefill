using System;
using System.IO;
using System.Collections.Generic;
using TShockAPI;
using TShockAPI.DB;
using Terraria;
using TerrariaApi.Server;
using Newtonsoft.Json;
using System.Data;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using System.Text;


namespace WorldRefill
{
    [ApiVersion(1, 16)]
    public class WorldRefill : TerrariaPlugin
    {
        public WorldRefill(Main game)
            : base(game)
        {
        }
        private static string savepath = TShock.SavePath;
        private static Config config;
		private IDbConnection ChestDB;
        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoCrystals, "gencrystals"));     //Life Crystals
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoPots, "genpots"));             //Pots
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoOrbs, "genorbs"));             //Orbs
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoAltars, "genaltars"));         //Demon Altars
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoTraps, "gentraps"));           //Traps
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoStatues, "genstatues"));       //Statues
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoOres, "genores"));             //ores
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoWebs, "genwebs"));             //webs
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoMineHouse, "genhouse"));       //mine house
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoTrees, "gentrees"));           //trees
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoIsland, "genisland"));         //floating island
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoShrooms, "genpatch"));         //mushroom patch
            //Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoLake, "genlake"));           //lake
            //Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoMountain, "genmountain"));   //mountain
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", CountEmpties, "genchests"));     //chests
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoIslandHouse, "genihouse"));    //island house
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoHV, "hellevator"));
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoPyramid, "genpyramid"));
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoCloudIsland, "gencisland"));   // NEW
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", ConfigReload, "refillreload"));  // NEW
            //Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoLivingTree, "genltree"));      // Added on v1.7.1 (Not working)
            ReadConfig();
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
            get { return new Version(1, 7, 2); }
        }
        public override string Name
        {
            get { return "World Refill Plugin"; }
        }
        public override string Author
        {
            get { return "k0rd, IcyPhoenix and Enerdy"; }
        }
        public override string Description
        {
            get { return "Refill your world!"; }
        }

        // Config Code stolen from InanZed's DieMob
        private static void CreateConfig()
        {
            string filepath = Path.Combine(savepath, "WorldRefillConfig.json");
            try
            {
                using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    using (var sr = new StreamWriter(stream))
                    {
                        config = new Config();
                        var configString = JsonConvert.SerializeObject(config, Formatting.Indented);
                        sr.Write(configString);
                    }
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
                config = new Config();
            }
        }

        private static bool ReadConfig()
        {
            string filepath = Path.Combine(savepath, "WorldRefillConfig.json");
            try
            {
                if (File.Exists(filepath))
                {
                    using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            var configString = sr.ReadToEnd();
                            config = JsonConvert.DeserializeObject<Config>(configString);
                        }
                        stream.Close();
                    }
                    return true;
                }
                else
                {
                    Log.ConsoleError("WorldRefill config not found. Creating new one...");
                    CreateConfig();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
            }
            return false;
        }

        // Variables to be added to WorldRefillConfig.json | Moved default chest IDs here so that people can edit them
        class Config
        {
            public int[] DefaultChestIDs = new int[] { 168, 20, 22, 40, 42, 28, 292, 298, 299, 290, 8, 31, 72, 280, 284, 281, 282, 279, 285, 21, 289, 303, 291, 304, 49, 50, 52, 53, 54, 55, 51, 43, 167, 188, 295, 302, 305, 73, 301, 159, 65, 158, 117, 265, 294, 288, 297, 300, 218, 112, 220, 985, 267, 156 };
			public bool UseInfiniteChests = false;
        }
        //Updating all players
        public static void InformPlayers(bool hard=false)
        {
            foreach (TSPlayer person in TShock.Players)
            {
                if ((person != null) && (person.Active))
                {
				for (int i = 0; i < 255; i++)
					{
						for (int j = 0; j < Main.maxSectionsX; j++)
						{
							for (int k = 0; k < Main.maxSectionsY; k++)
							{
								Netplay.serverSock[i].tileSection[j, k] = false;
							}
						}
					}
                }
            }

        }

        // Config Reload
        private void ConfigReload(CommandArgs args)
        {
                if (ReadConfig())
                    args.Player.SendMessage("WorldEdit config reloaded.", Color.Green);
                else
                    args.Player.SendErrorMessage("Error reading config. Check log for details.");
                return;
        }

        private void DoCrystals(CommandArgs args)
        {

            if (args.Parameters.Count == 1)
            {
                var mCry = Int32.Parse(args.Parameters[0]);
                var surface = Main.worldSurface;
                var trycount = 0;
                //maxtries = retry amounts if generation of object fails
                const int maxtries = 1000000;
                //realcount = actual amount of objects generated
                var realcount = 0;

                //Attempting to generate Objects
                while (trycount < maxtries)
                {
                    if (WorldGen.AddLifeCrystal(WorldGen.genRand.Next(1, Main.maxTilesX), WorldGen.genRand.Next((int)(surface + 20.0), Main.maxTilesY)))
                    {
                        realcount++;
                        //Determine if enough Objects have been generated
                        if (realcount == mCry) break;
                    }
                    trycount++;
                }
                //Notify user on success
                args.Player.SendMessage(string.Format("Generated and hid {0} Life Crystals.", realcount), Color.Green);

                InformPlayers();
            }
            else
            {
                //notify user of command failure
                args.Player.SendMessage(string.Format("Usage: /gencrystals (number of crystals to generate)"), Color.Green);
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
                args.Player.SendMessage(string.Format("Generated and hid {0} Pots.", realcount), Color.Green);
                InformPlayers();
            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /genpots (number of pots to generate)"), Color.Green);
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

                    if ((!Main.tile[tryX, tryY].active()) && ((Main.tile[tryX, tryY].wall == (byte)3) || (Main.tile[tryX, tryY].wall == (byte)83)))
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
                args.Player.SendMessage(string.Format("Generated and hid {0} Orbs.", realcount), Color.Green);
            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /genorbs (number of orbs to generate)"), Color.Green);
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
                args.Player.SendMessage(string.Format("Generated and hid {0} Demon Altars.", realcount), Color.Green);
            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /genaltars (number of Demon Altars to generate)"), Color.Green);
            }
        }
        private void DoTraps(CommandArgs args)
        {
            if (args.Parameters.Count == 1)
            {
                args.Player.SendMessage("Generating traps.. this may take a while..", Color.Green);
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
                args.Player.SendMessage(string.Format("Generated and hid {0} traps.", realcount), Color.Green);
            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /gentraps (number of Traps to generate)"), Color.Green);
            }
        }
        private void DoStatues(CommandArgs args)
        {
            if (args.Parameters.Count == 1)
            {
                args.Player.SendMessage("Generating statues.. this may take a while..", Color.Green);
                var mStatue = Int32.Parse(args.Parameters[0]);
                var surface = Main.worldSurface;
                var trycount = 0;
                const int maxtries = 100000;
                var realcount = 0;
                while (trycount < maxtries)
                {
                    var tryX = WorldGen.genRand.Next(20, Main.maxTilesX -20);
                    var tryY = WorldGen.genRand.Next((int)surface + 20, Main.maxTilesY -300);
                    var tryType = WorldGen.genRand.Next((int) 2, 47);
                   
                    while (!Main.tile[tryX, tryY].active())
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
                args.Player.SendMessage(string.Format("Generated and hid {0} Statues.", realcount), Color.Green);
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
											 "Lihzahrd",
											 "Lihzhard Watcher",
											 "Lihzahrd Guardian",
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

                    args.Player.SendMessage(string.Format("Generating {0} statues.. this may take a while..", found), Color.Green);
                    while (trycount < maxtries)
                    {
                        var tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                        var tryY = WorldGen.genRand.Next((int) surface + 20, Main.maxTilesY - 300);

                        while (!Main.tile[tryX, tryY].active())
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
                    args.Player.SendMessage(string.Format("Generated and hid {0} {1} ({2})Statues.", realcount, found, stid), Color.Green);
                    InformPlayers();
                }
                else
                {
                    args.Player.SendMessage(string.Format("Couldn't find a match for {0}.", mReqs), Color.Green);
                }
            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /genstatues (number of statues to generate) [(optional)name of statue]"), Color.Green);
            }
        }

        private static void DoOres(CommandArgs args)
        {
            if (WorldGen.genRand == null)
                WorldGen.genRand = new Random();

            TSPlayer ply = args.Player;
            ushort oreType;
            float oreAmts = 100f;
            //Sigh why did I bother checking what this does...gets completely overwritten
            //Mod altarcount divide 3 - gives number between: 0 to 2
            //int num = WorldGen.altarCount % 3;
            //Altarcount divide 3 + 1 - gives number between: 1 to infinity
            //int num2 = WorldGen.altarCount / 3 + 1;
            //4200 = small world size
            //6400 = medium world size
            //8400 = large world size
            //returns value: - 0 , 1.523809523809524, 2
            //float num3 = (float)(Main.maxTilesX / 4200);
            //Gives number between: -1 to 1
            //int num4 = 1 - num;
            //num3 * 310f - returns value: 0, 472.3809523809524, 620
            //(float)(85 * num) - gives number between 0 to 170
            //Returns value: -170, 302.3809523809524, 450
            //num3 = num3 * 310f - (float)(85 * num);
            //Returns Value: -144.5, 257.0238095238095, 382.5
            //num3 *= 0.85f;
            //gives number between: -144.5 to 382.5
            //num3 /= (float)num2;

            if (args.Parameters.Count < 1)
            {
                ply.SendMessage("Usage: /genores (type) (amount)", Color.Red);    //should this be a help message instead?
                return;
            }
            else if (args.Parameters[0].ToLower() == "cobalt")
            {
                oreType = 107;
            }
            else if (args.Parameters[0].ToLower() == "mythril")
            {
                oreType = 108;
            }
            else if (args.Parameters[0].ToLower() == "copper")
            {
                oreType = 7;
            }
            else if (args.Parameters[0].ToLower() == "iron")
            {
                oreType = 6;
            }
            else if (args.Parameters[0].ToLower() == "silver")
            {
                oreType = 9;
            }
            else if (args.Parameters[0].ToLower() == "gold")
            {
                oreType = 8;
            }
            else if (args.Parameters[0].ToLower() == "demonite")
            {
                oreType = 22;
            }
            else if (args.Parameters[0].ToLower() == "sapphire")
            {
                oreType = 63;
            }
            else if (args.Parameters[0].ToLower() == "ruby")
            {
                oreType = 64;
            }
            else if (args.Parameters[0].ToLower() == "emerald")
            {
                oreType = 65;
            }
            else if (args.Parameters[0].ToLower() == "topaz")
            {
                oreType = 66;
            }
            else if (args.Parameters[0].ToLower() == "amethyst")
            {
                oreType = 67;
            }
            else if (args.Parameters[0].ToLower() == "diamond")
            {
                oreType = 68;
            }
            else if (args.Parameters[0].ToLower() == "adamantite")
            {
                oreType = 111;
            }
            else if (args.Parameters[0].ToLower() == "hellstone")
            {
                oreType = 58;
            }

            // New Ores
            else if (args.Parameters[0].ToLower() == "tin")
            {
                oreType = 166;
            }
            else if (args.Parameters[0].ToLower() == "lead")
            {
                oreType = 167;
            }
            else if (args.Parameters[0].ToLower() == "tungsten")
            {
                oreType = 168;
            }
            else if (args.Parameters[0].ToLower() == "platinum")
            {
                oreType = 169;
            }
            else if (args.Parameters[0].ToLower() == "crimtane") //NEW
            {
                oreType = 204;
            }

            // 1.2 Hardmode Ores
            else if (args.Parameters[0].ToLower() == "palladium")
            {
                oreType = 221;
            }
            else if (args.Parameters[0].ToLower() == "orichalcum")
            {
                oreType = 222;
            }
            else if (args.Parameters[0].ToLower() == "titanium")
            {
                oreType = 223;
            }
            else if (args.Parameters[0].ToLower() == "chlorophyte") //NEW
            {
                oreType = 211;
            }

            // Others
            else if (args.Parameters[0].ToLower() == "silt") //NEW
            {
                oreType = 123;
            }
            else
            {
                ply.SendMessage("Warning! Typo in Tile name or Tile does not exist", Color.Red);    //should this be a help message instead?
                return;
            }

            //If user specifies how many ores to generate (make sure not over 10000)
            if (args.Parameters.Count > 1)
            {
                float.TryParse(args.Parameters[1], out oreAmts);
                oreAmts = Math.Min(oreAmts, 10000f);
            }
            //oreGened = track amount of ores generated already
            int oreGened = 0;
            int minFrequency;
            int maxFrequency;
            int minSpread;
            int maxSpread;
            while ((float)oreGened < oreAmts)
            {
                //Get random number from 100 tiles each side
                int i2 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                double worldY = Main.worldSurface;
                //Rare Ores  - Adamantite (Titanium), Demonite, Diamond, Chlorophyte
                if ((oreType == 111) || (oreType == 22) || (oreType == 204) || (oreType == 211) || (oreType == 223) || (oreType >= 63) && (oreType <= 68))
                {
                    //Some formula created by k0rd for getting somewhere between hell and roughly half way after rock
                    worldY = (Main.rockLayer + Main.rockLayer + (double)Main.maxTilesY) / 3.0;
                    minFrequency = 2;
                    minSpread = 2;
                    maxFrequency = 3;
                    maxSpread = 3;
                }
                //Hellstone Only
                else if (oreType == 58)
                {
                    //roughly where hell is
                    worldY = Main.maxTilesY - 200;
                    minFrequency = 4;
                    minSpread = 4;
                    maxFrequency = 9;
                    maxSpread = 9;
                }
                else
                {
                    worldY = Main.rockLayer;
                    minFrequency = 5;
                    minSpread = 9;
                    maxFrequency = 5;
                    maxSpread = 9;
                }
                //Gets random number based on minimum spawn point to maximum depth of map
                int j2 = WorldGen.genRand.Next((int)worldY, Main.maxTilesY - 150);
                WorldGen.OreRunner(i2, j2, (double)WorldGen.genRand.Next(minSpread, maxSpread), WorldGen.genRand.Next(minFrequency, maxFrequency), oreType);
                oreGened++;
            }
            ply.SendMessage(String.Format("Spawned {0} tiles of {1}", Math.Floor(oreAmts), args.Parameters[0].ToLower()), Color.Green);
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
                     while (!Main.tile[tryX, tryY].active() && tryY>149)
                     {
                         tryY--;
                     }
                    tryY++;
                     while (!Main.tile[tryX, tryY].active() && tryX > 10 && tryX < Main.maxTilesX - 10)
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
                args.Player.SendMessage(string.Format("Generated and hid {0} Webs.", realcount),Color.Green);
                InformPlayers();
            }
            else
            {
                args.Player.SendMessage(string.Format("Usage: /genwebs (number of webs to generate)"), Color.Green);
            }
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
                    if (Main.tile[z, y].active())
                    {
                        WorldGen.SpreadGrass(z, y ,59, 70, false );

                    }
                }
            }

                InformPlayers();
                args.Player.SendMessage("Mushroom Farm generated.", Color.Green);
        }
		
		private void DoPyramid(CommandArgs args)
		{
			int tryX = args.Player.TileX;
			int tryY = args.Player.TileY;
			bool resulta;
			resulta=WorldGen.Pyramid(tryX, tryY);
			
			if (resulta)
			{
				InformPlayers();
				args.Player.SendMessage("A pyramid was created.", Color.Green);
			}
			else
			{
				args.Player.SendMessage("A pyramid cannot be created here.", Color.Red);
			}
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
            int tl = 121;
            int wl = 25;
            int a = WorldGen.genRand.Next(1, 14);
            switch (a)
            {
                case 1:
                    tl = 38;
                    break;
                case 2:
                    tl = 30;
                    break;
                case 3:
                    tl = 41;
                    break;
                case 4:
                    tl = 43;
                    break;
                case 5:
                    tl = 44;
                    break;
                case 6:
                    tl = 45;
                    break;
                case 7:
                    tl = 46;
                    break;
                case 8:
                    tl = 47;
                    break;
                case 9:
                    tl = 75;
                    break;
                case 10:
                    tl = 76;
                    break;
                case 11:
                    tl = 119;
                    break;
                case 12:
                    tl = 121;
                    break;
                case 13:
                    tl = 122;
                    break;
            }
            a = WorldGen.genRand.Next(1, 16);
            switch (a)
            {
                case 1:
                    wl = 4;
                    break;
                case 2:
                    wl = 5;
                    break;
                case 3:
                    wl = 6;
                    break;
                case 4:
                    wl = 10;
                    break;
                case 5:
                    wl = 11;
                    break;
                case 6:
                    wl = 12;
                    break;
                case 7:
                    wl = 17;
                    break;
                case 8:
                    wl = 18;
                    break;
                case 9:
                    wl = 19;
                    break;
                case 10:
                    wl = 20;
                    break;
                case 11:
                    wl = 21;
                    break;
                case 12:
                    wl = 23;
                    break;
                case 13:
                    wl = 24;
                    break;
                case 14:
                    wl = 25;
                    break;
                case 15:
                    wl = 26;
                    break;
            }

            for (cx=start; cx < width + start; cx++)
            {
                int xc;
                for (xc = ypos; xc < bottom; xc++)
                {
                    if ((cx == start) || (cx == width + start - 1))
                    {
                        Main.tile[cx, xc].type = (byte) tl;
                        Main.tile[cx, xc].active(true);
						Main.tile[cx, xc].slope(0);
						Main.tile[cx, xc].halfBrick(false);
                    }
                    else
                    {
                        WorldGen.KillTile(cx, xc, false, false, false);
                        Main.tile[cx, xc].wall = (byte) wl;
                    }
                }
            }


            InformPlayers();
			args.Player.SendMessage("Going down?", Color.Green);
        }
		private void DoMineHouse(CommandArgs args)
        {
            int tryX = args.Player.TileX;
            int tryY = args.Player.TileY;
            WorldGen.MineHouse(tryX, tryY +1);
			args.Player.SendMessage("Attempted to generate a Mine House here.",Color.Green);
            InformPlayers();
        }
        private void DoIslandHouse(CommandArgs args)
        {
            int tryX = args.Player.TileX;
            int tryY = args.Player.TileY;
            WorldGen.IslandHouse(tryX, tryY +1);
			args.Player.SendMessage("Attempted to generate an Island House here.",Color.Green);
            InformPlayers();
        }
		
		private void DoIsland(CommandArgs args)
		{
			int tryX = args.Player.TileX;
			int tryY = args.Player.TileY;
			if (tryY <= 50)
				tryY=51;
			WorldGen.CloudIsland(tryX, tryY -50);
			args.Player.SendMessage("Attempted to generate a floating island above you.",Color.Green);
			InformPlayers();
		}

        private void DoCloudIsland(CommandArgs args)
        {
            int tryX = args.Player.TileX;
            int tryY = args.Player.TileY;
            WorldGen.CloudIsland(tryX, tryY + 9);
            WorldGen.IslandHouse(tryX, tryY + 1);

            args.Player.SendMessage("Attempted to generate an un-looted floating island at your position.", Color.Green);
            InformPlayers();
        }

        // Needs revision, does not work for some reason
        private void DoLivingTree(CommandArgs args)
        {
            int tryX = args.Player.TileX;
            int tryY = args.Player.TileY;
            WorldGen.GrowLivingTree(tryX, tryY);

            args.Player.SendMessage("Attempted to grow a living tree at your position.", Color.Green);
            InformPlayers();
        }

        private void CountEmpties(CommandArgs args)
        {
            if (args.Parameters.Count == 0 || args.Parameters.Count > 2)
            {
                args.Player.SendMessage("Usage: /genchests <amount> [gen mode: default/easy/all]", Color.Green);
            }
            int empty = 0;
            int tmpEmpty = 0;
            int chests = 0;
			int maxChests=1000;
			
            string setting = "default";
            if( args.Parameters.Count > 1 )
            {
                setting = args.Parameters[1];
            }
            const int maxtries = 100000;
            Int32.TryParse(args.Parameters[0], out chests);
            const int threshold = 100;
			if (!config.UseInfiniteChests){
            for (int x = 0; x < maxChests; x++)
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
            args.Player.SendMessage(string.Format("uprooted {0} empty out of {1} chests.", empty, tmpEmpty), Color.Green);
			}
			else
			{

				try{
                switch (TShock.Config.StorageType.ToLower())
                {
                    case "mysql":
                        string[] host = TShock.Config.MySqlHost.Split(':');
                        ChestDB = new MySqlConnection()
                        {
                            ConnectionString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
                                host[0],
                                host.Length == 1 ? "3306" : host[1],
                                TShock.Config.MySqlDbName,
                                TShock.Config.MySqlUsername,
                                TShock.Config.MySqlPassword)
                        };
                        break;
                    case "sqlite":
                        string sql = Path.Combine(TShock.SavePath, "chests.sqlite");
                        ChestDB = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));
                        break;
                }
				} catch (Exception ex)
				{
					Log.ConsoleError(ex.ToString());
				}
			}
            if (chests + tmpEmpty + threshold > maxChests)
                chests = maxChests - tmpEmpty - threshold;
            if (chests >0)
            {
                int chestcount = 0;
                chestcount = tmpEmpty;
                int tries = 0;
                int newcount = 0;
                while (newcount < chests)
                {
                    int contain;
                    if( setting == "default" )
                    {
                        // Moved item list into a separate .txt file
                        int[] itemID = config.DefaultChestIDs;
                        contain = itemID[WorldGen.genRand.Next(0, itemID.GetUpperBound(0))];
                    }
                    else if (setting == "all")
                    {
						// Updated item list to 1.2.4.1
                        contain = WorldGen.genRand.Next(-48, 2744);
                    }
                    else if (setting == "easy")
                    {
                        contain = WorldGen.genRand.Next(-24, 364);
                    }
                    else
                    {
                        args.Player.SendMessage(string.Format("Warning! Typo in second argument: {0}",args.Parameters[1]), Color.Red);
                        return;
                    }
                    int tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                    int tryY = WorldGen.genRand.Next((int) Main.worldSurface, Main.maxTilesY - 200);
                        while (!Main.tile[tryX, tryY].active())
                        {
                            tryY++;
                        }
                        tryY--;
                    WorldGen.KillTile(tryX, tryY, false, false, false);
                    WorldGen.KillTile(tryX +1 , tryY, false, false, false);
                    WorldGen.KillTile(tryX, tryY + 1, false, false, false);
                    WorldGen.KillTile(tryX + 1, tryY, false, false, false);

                    if (WorldGen.AddBuriedChest(tryX, tryY, contain, true, 1))
                    {
                        chestcount++;
                        newcount++;
						if (config.UseInfiniteChests)
						{
						
						 StringBuilder items = new StringBuilder();                   
                                Terraria.Chest c = Main.chest[0];
                                if (c != null)
                                {
                                        for (int j = 0; j < 40; j++)
                                        {
                                                items.Append(c.item[j].netID + "," + c.item[j].stack + "," + c.item[j].prefix);
                                                if (j != 39)
                                                {
                                                        items.Append(",");
                                                }
                                        }
										try{
                                        ChestDB.Query("INSERT INTO Chests (X, Y, Account, Items, WorldID) VALUES (@0, @1, '', @2, @3)",
                                                c.x, c.y, items.ToString(), Main.worldID);
                                        }catch (Exception ex)
										{
										Log.ConsoleError(ex.ToString());
										}
                                        items.Clear();
                                        Main.chest[0] = null;
                                }
                        }
						
                    }
                    if (tries + 1 >= maxtries)
                        break;

                    tries++;
                }
				if (config.UseInfiniteChests)
					ChestDB.Dispose();
                args.Player.SendMessage(string.Format("generated {0} new chests - {1} total", newcount, chestcount), Color.Green);
                InformPlayers();
            }
        }
    }
}
