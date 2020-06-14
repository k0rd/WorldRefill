using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OTAPI.Tile;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Modules;
using Terraria.UI;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;
using TShockAPI.Net;

namespace WorldRefill
{
    [ApiVersion(2, 1)]
    public class WorldRefill : TerrariaPlugin
    {
        public WorldRefill(Main game)
            : base(game)
        {
            Order = +2;
        }
        #region Plugin Info
        public override Version Version
        {
            get { return new Version(2, 0, 0); }
        }
        public override string Name
        {
            get { return "World Refill"; }
        }
        public override string Author
        {
            get { return "k0rd, IcyPhoenix and Enerdy (Updated by Matheis)"; }
        }
        public override string Description
        {
            get { return "Refill your world!"; }
        }


        private static string savepath = TShock.SavePath;
        private static Config config;
        private IDbConnection ChestDB;
        #endregion

        #region Initialize
        public override void Initialize()
        {
            #region Gen Commands
            Commands.ChatCommands.Add(new Command("worldrefill.generate", Generate, "generate", "gen")
            {
                AllowServer = false,
            });


            #region Commands to Add
            //Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoLivingTree, "genltree"));		// Added on v1.7.1 (Not working)
            //Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoLake, "genlake"));			//lake
            //Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoMountain, "genmountain"));	//mountain
            // Desert Island 
            // Cloud Island 
            // Any other additions to 1.4
            #endregion
            #endregion

            ReadConfig();

            GeneralHooks.ReloadEvent += OnReload;



        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= OnReload;
            }
            base.Dispose(disposing);
        }
        #endregion



        #region Config
        // Config Code stolen from InanZed's DieMob
        #region Create
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
                TShock.Log.ConsoleError(ex.Message);
                config = new Config();
            }
        }
        #endregion
        #region Read
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
                    TShock.Log.ConsoleError("World Refill config not found. Creating new one...");
                    CreateConfig();
                    return false;
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(ex.Message);
            }
            return false;
        }
        #endregion
        #region Reload Command
        // Config Reload
        private void OnReload(ReloadEventArgs args)
        {
            if (ReadConfig())
                args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] {Name} config reloaded.");

            else
                args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Error reading config. Check log for details.");
            return;
        }
        #endregion

        class Config
        {
            // Variables to be added to WorldRefillConfig.json | Moved default chest IDs here so that people can edit them
            #region DefaultChestIDs
            public int[] DefaultChestIDs = new[]
            {
                168,
                20,
                22,
                40,
                42,
                28,
                292,
                298,
                299,
                290,
                8,
                31,
                72,
                280,
                284,
                281,
                282,
                279,
                285,
                21,
                289,
                303,
                291,
                304,
                49,
                50,
                52,
                53,
                54,
                55,
                51,
                43,
                167,
                188,
                295,
                302,
                305,
                73,
                301,
                159,
                65,
                158,
                117,
                265,
                294,
                288,
                297,
                300,
                218,
                112,
                220,
                985,
                267,
                156
            };
            #endregion
            public bool UseInfiniteChests = false;
            public bool GenInsideProtectedRegions = false;
            public int GenerationMaxTries = 1000000; // Setting this value higher may result in more lag when generating as this is the maximum amount of tries it will take to generate amenities.
        }
        #endregion
        #region Case Options




        readonly IReadOnlyList<string> optionslist = new List<string>
            {
               "crystals",
                "pots",
                "orbs",
                "altars",
                "cavetraps",
                "templetraps",
                "statuetraps",
                "lavatraps",
                "sandtraps",
                "statues",
                "ores",
                "webs",
                "shrooms"


            };
        readonly IReadOnlyList<string> structslist = new List<string>
        {
            "trees",
            "dungeon",
            //"temple"
            //"livingtree"
            "pyramid",
            "minehouse"




        };
        private void PrintOptions(CommandArgs args)
        {
            string options = "";
            string structs = "";
            args.Player.SendErrorMessage("Resources:");
            foreach (string option in optionslist)
            {
                options += $"| {option} | ";
            }
            args.Player.SendErrorMessage(options);
            args.Player.SendErrorMessage("Structures:");
            foreach (string structure in structslist)
            {
                structs += $"| {structure} | ";
            }
            args.Player.SendErrorMessage(structs);


        }




        #region Generate Command
        private void Generate(CommandArgs args)

        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Invalid Syntax! " +
                      "Please refer to the following options...");
                PrintOptions(args);

                return;
            }
            int amount;
            if (args.Parameters.Count == 1 && structslist.Contains(args.Parameters[0].ToLowerInvariant()))
            {
                amount = 0;

            }
            else
            {



                var amountregex = new Regex(@"\b(?<amount>[1-9]\d*){1}$");
                Match amountmatch;
                try
                {
                    amountmatch = amountregex.Match(args.Parameters[1]);
                }
                catch (ArgumentOutOfRangeException)
                {
                    amountmatch = amountregex.Match("0");
                }


                if (!amountmatch.Success || float.Parse(args.Parameters[1]) < 1)
                {
                    bool found = false;
                    foreach (string option in optionslist)
                    {
                        if (option == args.Parameters[0].ToLowerInvariant())
                        {
                            found = true;
                            if (option == "statues" || option == "ores")
                            {
                                string singular = option.Substring(0, option.LastIndexOf('s'));
                                args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Invalid Syntax! Use {Commands.Specifier}gen {option} <Amount of {option}> [Specified {singular}]");
                            }
                            else args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Invalid Syntax! Use {Commands.Specifier}gen {option} <Amount of {option}>");
                            break;
                        }
                    }
                    if (!found)
                    {
                        args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Invalid Syntax! " +
                        "Please refer to the following options...");
                        PrintOptions(args);

                    }
                    return;
                }
                amount = int.Parse(amountmatch.Value);
            }

            var surface = Main.worldSurface;
            var trycount = 0;
            //maxtries = retry amounts if generation of object fails (this is used to prevent lag issues)
            int maxtries = config.GenerationMaxTries;
            //realcount = actual amount of objects generated
            var realcount = 0;
            int tryX;
            int tryY;
            List<string> lines;


            switch (args.Parameters[0].ToLowerInvariant())
            {
                #region Life Crystals

                case "crystals":


                    //Attempting to generate Objects
                    while (trycount < maxtries)
                    {
                        if (WorldGen.AddLifeCrystal(WorldGen.genRand.Next(1, Main.maxTilesX), WorldGen.genRand.Next((int)(surface + 20.0), (int)(Main.maxTilesY - 100.0))))
                        {
                            realcount++;
                            //Determine if enough Objects have been generated
                            if (realcount == amount) break;
                        }
                        trycount++;
                    }
                    //Notify user on success
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Life Crystals");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Life Crystals", 71, 8, 185); InformPlayers();
                    break;





                #endregion
                #region Pots
                case "pots":



                    while (trycount < maxtries)
                    {
                        tryX = WorldGen.genRand.Next(1, Main.maxTilesX);

                        tryY = WorldGen.genRand.Next((int)Main.worldSurface - 5, Main.maxTilesY - 20);


                        // This code was stolen from Terraria source, so dont blame me because it doesn't look neat haha.

                        int tile = (int)Main.tile[tryX, tryY + 1].type;
                        int wall = (int)Main.tile[tryX, tryY].wall;
                        int style = WorldGen.genRand.Next(0, 4);
                        if (tile == 147 || tile == 161 || tile == 162)
                            style = WorldGen.genRand.Next(4, 7);
                        if (tile == 60)
                            style = WorldGen.genRand.Next(7, 10);
                        if (Main.wallDungeon[(int)Main.tile[tryX, tryY].wall])
                            style = WorldGen.genRand.Next(10, 13);
                        if (tile == 41 || tile == 43 || (tile == 44 || tile == 481) || (tile == 482 || tile == 483))
                            style = WorldGen.genRand.Next(10, 13);
                        if (tile == 22 || tile == 23 || tile == 25)
                            style = WorldGen.genRand.Next(16, 19);
                        if (tile == 199 || tile == 203 || (tile == 204 || tile == 200))
                            style = WorldGen.genRand.Next(22, 25);
                        if (tile == 367)
                            style = WorldGen.genRand.Next(31, 34);
                        if (tile == 226)
                            style = WorldGen.genRand.Next(28, 31);
                        if (wall == 187 || wall == 216)
                            style = WorldGen.genRand.Next(34, 37);
                        if (tryY > Main.UnderworldLayer)
                            style = WorldGen.genRand.Next(13, 16);


                        if (!WorldGen.oceanDepths(tryX, tryY) && WorldGen.PlacePot(tryX, tryY, 28, (int)style))
                        {


                            realcount++;
                            if (realcount == amount)
                                break;

                        }
                        trycount++;

                    }
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Pots.");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Pots.", 71, 8, 185); InformPlayers();
                    break;

                #endregion
                #region Orbs
                case "orbs":

                    while (trycount < maxtries)
                    {
                        tryX = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                        tryY = WorldGen.genRand.Next((int)surface + 20, Main.UnderworldLayer);

                        if ((!Main.tile[tryX, tryY].active()) && ((Main.tile[tryX, tryY].wall == WallID.EbonstoneUnsafe) || (Main.tile[tryX, tryY].wall == WallID.CrimstoneUnsafe)))
                        {
                            WorldGen.AddShadowOrb(tryX, tryY);

                            if (Main.tile[tryX, tryY].type == 31)
                            {

                                realcount++;
                                if (realcount == amount)
                                    break;
                            }
                        }
                        trycount++;


                    }
                    InformPlayers();
                    if (!WorldGen.crimson)
                    {
                        args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Shadow Orbs.");
                        if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Shadow Orbs.", 71, 8, 185);
                    }
                    else
                    {
                        args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Crimson Hearts.");
                        if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Crimson Hearts.", 71, 8, 185);
                    }
                    break;



                #endregion
                #region Altars
                case "altars":


                    while (trycount < maxtries)
                    {
                        tryX = WorldGen.genRand.Next(1, Main.maxTilesX);
                        tryY = WorldGen.genRand.Next((int)surface + 10, (int)Main.rockLayer);

                        if ((!Main.tile[tryX, tryY].active()) && ((Main.tile[tryX, tryY].wall == WallID.EbonstoneUnsafe) || (Main.tile[tryX, tryY].wall == WallID.CrimstoneUnsafe)))
                        {

                            if (!WorldGen.crimson) WorldGen.Place3x2(tryX, tryY, TileID.DemonAltar);
                            else WorldGen.Place3x2(tryX, tryY, TileID.DemonAltar, 1);


                            if (Main.tile[tryX, tryY].type == 26)
                            {

                                realcount++;
                                if (realcount == amount)
                                    break;
                            }
                        }
                        trycount++;
                    }
                    InformPlayers();
                    if (!WorldGen.crimson)
                    {
                        args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Demon Altars.");
                        if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Demon Altars.", 71, 8, 185);
                    }
                    else
                    {
                        args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Crimson Altars.");
                        if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Crimson Altars.", 71, 8, 185);
                    }

                    break;


                #endregion
                #region Cave Traps
                case "cavetraps":


                    while (trycount < maxtries)
                    {
                        tryX = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
                        tryY = WorldGen.genRand.Next((int)surface, Main.UnderworldLayer - 100);
                        var type = WorldGen.genRand.Next(-1, 1);
                        if (Main.tile[tryX, tryY].wall == WallID.None && WorldGen.placeTrap(tryX, tryY, type))
                        {
                            realcount++;
                            if (realcount == amount)
                                break;
                        }

                        trycount++;
                    }
                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Randomized Cave Traps.");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Randomized Cave Traps.", 71, 8, 185);
                    break;



                #endregion
                #region Temple Traps
                case "templetraps":


                    while (trycount < maxtries)

                    {
                        tryX = WorldGen.genRand.Next(250, Main.maxTilesX - 250);
                        tryY = WorldGen.genRand.Next((int)surface, Main.UnderworldLayer);


                        if (WorldGen.mayanTrap(tryX, tryY))
                        {
                            realcount++;
                            if (realcount == amount) break;
                        }



                        trycount++;

                    }
                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Randomized Temple Traps.");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Randomized Temple Traps.", 71, 8, 185);
                    break;


                #endregion
                #region Statue Traps
                case "statuetraps":


                    while (trycount < maxtries)

                    {
                        tryX = WorldGen.genRand.Next(11, Main.maxTilesX - 11);
                        tryY = WorldGen.genRand.Next((int)surface, Main.UnderworldLayer);

                        WorldGen.PlaceStatueTrap(tryX, tryY);

                        if ((int)Main.tile[tryX, tryY].type == TileID.Statues)
                        {

                            realcount++;
                            if (realcount == amount) break;
                        }



                        trycount++;

                    }
                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Randomized Statue Traps.");
                    TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Randomized Statue Traps.", 71, 8, 185);
                    break;


                #endregion
                #region Lava Traps
                case "lavatraps":



                    while (trycount < maxtries)

                    {
                        tryX = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                        tryY = WorldGen.genRand.Next(750, Main.UnderworldLayer);




                        if (WorldGen.placeLavaTrap(tryX, tryY))
                        {

                            realcount++;
                            if (realcount == amount) break;
                        }



                        trycount++;

                    }
                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Lava Traps.");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Lava Traps.", 71, 8, 185); break;

                #endregion
                #region Sand Traps
                case "sandtraps":


                    while (trycount < maxtries)

                    {
                        tryX = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                        tryY = WorldGen.genRand.Next((int)surface, Main.UnderworldLayer);




                        if (WorldGen.PlaceSandTrap(tryX, tryY))
                        {
                            args.Player.SendInfoMessage($"POS X {tryX}, POS Y {tryY}");
                            realcount++;
                            if (realcount == amount) break;
                        }



                        trycount++;

                    }
                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Sand Traps.");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Sand Traps.", 71, 8, 185);

                    break;


                #endregion
                #region Statues
                case "statues":

                    WorldGen.SetupStatueList();

                    List<Point16> statuelist = WorldGen.statueList.ToList(); //re-writing statuelist to include turtle and owl statues.
                    statuelist.Add(new Point16(TileID.Statues, 76));
                    statuelist.Add(new Point16(TileID.Statues, 79));
                    statuelist.Add(new Point16(TileID.BoulderStatue, 0));
                    WorldGen.statueList = statuelist.ToArray();



                    bool isinNonNaturalPlace(ushort tile)
                    {
                        List<ushort> bannedtile = new List<ushort>
                        {
                            19,
                            41,
                            43,
                            44,
                            225,
                            226,
                            481,
                            482,
                            483


                        };
                        if (bannedtile.Contains(tile))
                        {


                            return true;
                        }
                        else
                        {


                            return false;
                        }


                    }
                    bool tileValidation(int X, int Y)
                    {
                        int xoffset;
                        int yoffset;

                        if (Main.tile[X, Y].honey() || Main.tile[X, Y].lava() || (!Main.tile[X, Y + 1].active()) || (!Main.tile[X + 1, Y + 1].active()) || !Main.tileSolid[Main.tile[X, Y + 1].type] || !Main.tileSolid[Main.tile[X + 1, Y + 1].type]) return false;

                        for (xoffset = 0; xoffset < 2; xoffset++)
                        {
                            for (yoffset = 0; yoffset < 3; yoffset++)
                            {


                                if (Main.tile[X + xoffset, Y - yoffset].active()) return false;



                            }
                        }
                        return true;
                    }

                    if (args.Parameters.Count == 2)
                    {
                        while (trycount < maxtries)
                        {
                            var tryType = WorldGen.genRand.Next(0, WorldGen.statueList.Count() - 1);

                            tryX = WorldGen.genRand.Next(30, Main.maxTilesX - 30);
                            tryY = WorldGen.genRand.Next((int)surface + 20, Main.UnderworldLayer);
                            var statue = WorldGen.statueList[tryType];






                            while (!tileValidation(tryX, tryY))
                            {
                                tryY++;
                                if (tryY >= Main.UnderworldLayer)
                                {
                                    break;
                                }
                            }

                            if (tryY < Main.UnderworldLayer && (!isinNonNaturalPlace(Main.tile[tryX, tryY + 1].type)))
                            {



                                WorldGen.PlaceTile(tryX, tryY, statue.X, true, true, -1, statue.Y);

                                if (Main.tile[tryX, tryY].type == statue.X)
                                {

                                    realcount++;
                                    if (realcount == amount)
                                        break;
                                }
                            }
                            trycount++;
                        }
                        InformPlayers();
                        args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Statues.");
                        if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Statues.", 71, 8, 185);
                        break;
                    }

                    else
                    {

                        int stindex = 0;
                        List<string> stlist = new List<string>
                        {
                            "armor",
                            "angel",
                            "star",
                            "sword",
                            "slime",
                            "goblin",
                            "shield",
                            "bat",
                            "fish",
                            "bunny",
                            "skeleton",
                            "reaper",
                            "woman",
                            "imp",
                            "gargoyle",
                            "gloom",
                            "hornet",
                            "bomb",
                            "crab",
                            "hammer",
                            "potion",
                            "spear",
                            "cross",
                            "jellyfish",
                            "bow",
                            "boomerang",
                            "boot",
                            "chest",
                            "bird",
                            "axe",
                            "corrupt",
                            "tree",
                            "anvil",
                            "pickaxe",
                            "mushroom",
                            "eyeball",
                            "pillar",
                            "heart",
                            "pot",
                            "sunflower",
                            "king",
                            "queen",
                            "piranha",
                            "shark",
                            "wall creeper",
                            "unicorn",
                            "drippler",
                            "wraith",
                            "undead viking",
                            "medusa",
                            "harpy",
                            "pigron",
                            "hoplite",
                            "granite golem",
                            "blood zombie",
                            "squirrel",
                            "butterfly",
                            "worm",
                            "firefly",
                            "scorpion",
                            "snail",
                            "grasshopper",
                            "mouse",
                            "duck",
                            "penguin",
                            "frog",
                            "buggy",
                            "seagull",
                            "dragonfly",


                    };
                        if (Main.expertMode)
                        {
                            stlist.Add("bone skeleton");
                            stlist.Add("armed zombie");
                        }
                        stlist.Add("owl"); // These have to be added afterwards because of the way the statuelist array is generated, because i have implemented the owl and turtle statues myself they have to be added after the 
                        // the initialization
                        stlist.Add("turtle");
                        stlist.Add("boulder");
                        Dictionary<string, Point16> statues = new Dictionary<string, Point16>();
                        foreach (Point16 statue in WorldGen.statueList)
                        {

                            statues.Add(stlist[stindex], statue);


                            stindex++;

                        }








                        string selStatue = args.Parameters[2].ToLowerInvariant();
                        stlist = statues.Keys.ToList<string>();
                        var findStatue = FindMatches(selStatue, stlist);


                        if (findStatue.Count > 1)
                        {
                            args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] More than one match found: ");

                            lines = PaginationTools.BuildLinesFromTerms(findStatue.ToArray());
                            lines.ForEach(args.Player.SendInfoMessage);
                        }
                        else if (findStatue.Count < 1)
                        {
                            args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] No matches found for statue '{selStatue}'");
                            args.Player.SendInfoMessage($"[[c/FFFFFF:{Name}]] Full list of statues :");

                            lines = PaginationTools.BuildLinesFromTerms(stlist.ToArray());
                            lines.ForEach(args.Player.SendInfoMessage);
                        }
                        else
                        {
                            statues.TryGetValue(findStatue[0], out Point16 statue);
                            while (trycount < maxtries)
                            {
                                tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                                tryY = WorldGen.genRand.Next((int)surface + 20, Main.UnderworldLayer);

                                while (!tileValidation(tryX, tryY))
                                {
                                    tryY++;
                                    if (tryY >= Main.UnderworldLayer)
                                    {
                                        break;
                                    }
                                }
                                if (tryY < Main.UnderworldLayer && (!isinNonNaturalPlace(Main.tile[tryX, tryY + 1].type)))
                                {

                                    WorldGen.PlaceTile(tryX, tryY, statue.X, true, true, -1, statue.Y);

                                    if (Main.tile[tryX, tryY].type == statue.X)
                                    {

                                        realcount++;
                                        if (realcount == amount)
                                            break;
                                    }
                                }
                                trycount++;

                            }
                            InformPlayers();
                            args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} {findStatue[0]} Statues.");
                            if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} {findStatue[0]} Statues.", 71, 8, 185);

                        }

                    }
                    break;

                #endregion
                #region Ores
                case "ores":


                    ushort oreID;
                    int oreTier;
                    int minFrequency;
                    int maxFrequency;
                    int minSpread;
                    int maxSpread;
                    List<ushort[]> oreTiers = new List<ushort[]> {
                        new ushort[] {TileID.Copper,TileID.Tin, TileID.Iron, TileID.Lead },
                        new ushort[] {TileID.Silver, TileID.Tungsten, TileID.Gold, TileID.Platinum,TileID.Sapphire,TileID.Ruby,TileID.Topaz,TileID.Emerald,TileID.Amethyst,TileID.Diamond},
                        new ushort[] {TileID.Demonite, TileID.Crimtane},
                        new ushort[] {TileID.Hellstone},
                        new ushort[] {TileID.Cobalt,TileID.Palladium},
                        new ushort[] {TileID.Mythril, TileID.Orichalcum},
                        new ushort[] {TileID.Adamantite, TileID.Titanium},
                        new ushort[] {TileID.Chlorophyte}


                    };

                    int GetTier(ushort ore)
                    {
                        int i = 1;
                        foreach (ushort[] tiers in oreTiers)
                        {
                            if (tiers.Contains(ore)) return i;
                            i++;
                        }

                        return 0;
                    };

                    bool TileValidation(ITile tile, ushort ore)
                    {
                        List<ushort> JungleWalls = new List<ushort>
                        {
                            0,
                            WallID.Jungle,
                            WallID.Jungle1Echo,
                            WallID.Jungle2Echo,
                            WallID.Jungle3Echo,
                            WallID.Jungle4Echo,
                            WallID.JungleUnsafe,
                            WallID.JungleUnsafe1,
                            WallID.JungleUnsafe2,
                            WallID.JungleUnsafe3,
                            WallID.JungleUnsafe4,
                        };
                        if (ore != TileID.Chlorophyte) return true;
                        if (tile.type == TileID.Mud && ore == TileID.Chlorophyte && JungleWalls.Contains(tile.wall)) return true;
                        else return false;
                    };

                    Dictionary<string, ushort> ores = new Dictionary<string, ushort>
                    {
                        ["cobalt"] = TileID.Cobalt,
                        ["mythril"] = TileID.Mythril,
                        ["copper"] = TileID.Copper,
                        ["iron"] = TileID.Iron,
                        ["silver"] = TileID.Silver,
                        ["gold"] = TileID.Gold,
                        ["demonite"] = TileID.Demonite,
                        ["sapphire"] = TileID.Sapphire,
                        ["ruby"] = TileID.Ruby,
                        ["emerald"] = TileID.Emerald,
                        ["topaz"] = TileID.Topaz,
                        ["amethyst"] = TileID.Amethyst,
                        ["diamond"] = TileID.Diamond,
                        ["adamantite"] = TileID.Adamantite,
                        ["hellstone"] = TileID.Hellstone,
                        ["tin"] = TileID.Tin,
                        ["lead"] = TileID.Lead,
                        ["tungsten"] = TileID.Tungsten,
                        ["platinum"] = TileID.Platinum,
                        ["crimtane"] = TileID.Crimtane,
                        ["palladium"] = TileID.Palladium,
                        ["orichalcum"] = TileID.Orichalcum,
                        ["titanium"] = TileID.Titanium,
                        ["chlorophyte"] = TileID.Chlorophyte,
                        ["stone"] = TileID.Stone,
                        ["sand"] = TileID.Sand,
                        ["silt"] = TileID.Silt,



                    };
                    if (amount > 1000) args.Player.SendInfoMessage($"[[c/FFFFFF:{Name}]] This one may take a while to load. Please wait...");

                    if (args.Parameters.Count == 2)
                    {
                        List<ushort> totalores = ores.Values.ToList<ushort>();



                        amount = Math.Min(amount, 10000);




                        while (trycount < maxtries)
                        {

                            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                            double maxY = Main.maxTilesY;
                            double minY = Main.worldSurface;
                            var trytype = WorldGen.genRand.Next(0, totalores.Count - 1);
                            oreID = totalores[trytype];
                            if (WorldGen.crimson && oreID == TileID.Demonite) oreID = TileID.Crimtane; //If randomly generated ore, make the ore world specific.

                            oreTier = GetTier(oreID);

                            switch (oreTier)
                            {

                                case 1:
                                    maxY = Main.rockLayer;
                                    minY = surface;
                                    minFrequency = 5;
                                    minSpread = 5;
                                    maxFrequency = 8;
                                    maxSpread = 8;

                                    break;
                                case 2:
                                    maxY = 2 * Main.rockLayer;
                                    minY = Main.rockLayer;
                                    minFrequency = 4;
                                    minSpread = 4;
                                    maxFrequency = 6;
                                    maxSpread = 6;
                                    break;
                                case 3:
                                    maxY = Main.UnderworldLayer;
                                    minY = (3 / 2) * Main.rockLayer;
                                    minFrequency = 2;
                                    minSpread = 2;
                                    maxFrequency = 2;
                                    maxSpread = 4;
                                    break;
                                case 4:
                                    maxY = Main.maxTilesY;
                                    minY = Main.UnderworldLayer + 20;
                                    minFrequency = 4;
                                    minSpread = 4;
                                    maxFrequency = 9;
                                    maxSpread = 9;
                                    break;


                                case 5:
                                    minY = Main.rockLayer;
                                    maxY = Main.rockLayer * 2;
                                    minFrequency = 5;
                                    minSpread = 5;
                                    maxFrequency = 9;
                                    maxSpread = 9;
                                    break;
                                case 6:
                                    minY = Main.rockLayer * (3 / 2);
                                    maxY = Main.UnderworldLayer;
                                    minFrequency = 4;
                                    minSpread = 4;
                                    maxFrequency = 7;
                                    maxSpread = 7;
                                    break;
                                case 7:
                                    minY = Main.rockLayer * (5 / 2);
                                    maxY = Main.UnderworldLayer;
                                    minFrequency = 3;
                                    minSpread = 3;
                                    maxFrequency = 5;
                                    maxSpread = 5;
                                    break;
                                case 8:
                                    minY = Main.rockLayer;
                                    maxY = Main.UnderworldLayer;
                                    minFrequency = 5;
                                    minSpread = 5;
                                    maxFrequency = 9;
                                    maxSpread = 9;
                                    break;
                                default:
                                    maxY = Main.rockLayer;
                                    minFrequency = 5;
                                    minSpread = 9;
                                    maxFrequency = 5;
                                    maxSpread = 9;
                                    break;


                            }

                            //Gets random number based on minimum spawn point to maximum depth of map
                            int Y = WorldGen.genRand.Next((int)minY, (int)maxY);
                            if (TileValidation(Main.tile[X, Y], oreID))
                            {
                                WorldGen.OreRunner(X, Y, (double)WorldGen.genRand.Next(minSpread, maxSpread), WorldGen.genRand.Next(minFrequency, maxFrequency), oreID);


                                if (Main.tile[X, Y].type == oreID)
                                {

                                    realcount++;
                                    if (realcount == amount) break;
                                }
                            }
                            trycount++;
                        }

                        InformPlayers();
                        args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} randomized ores.");
                        if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} randomized ores.", 71, 8, 185);
                        break;
                    }



                    else
                    {
                        var selore = args.Parameters[2].ToLowerInvariant();
                        var searchlist = ores.Keys.ToList();
                        var findore = FindMatches(selore, searchlist);


                        if (findore.Count > 1)
                        {
                            args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] More than one match found: ");

                            lines = PaginationTools.BuildLinesFromTerms(findore.ToArray());
                            lines.ForEach(args.Player.SendInfoMessage);

                        }
                        else if (findore.Count < 1)
                        {
                            args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] No matches found for ore '{selore}'");
                            args.Player.SendInfoMessage($"[[c/FFFFFF:{Name}]] Full list of ores :");

                            lines = PaginationTools.BuildLinesFromTerms(ores.Keys.ToArray());
                            lines.ForEach(args.Player.SendInfoMessage);
                        }
                        else
                        {
                            ores.TryGetValue(findore[0], out oreID);


                            //If user specifies how many ores to generate (make sure not over 10000)



                            amount = Math.Min(amount, 10000);

                            //oreGened = track amount of ores generated already
                            double maxY = Main.maxTilesY;
                            double minY = Main.worldSurface + 50;
                            //Rare Ores  - Adamantite (Titanium), Demonite, Diamond, Chlorophyte
                            oreTier = GetTier(oreID);

                            switch (oreTier)
                            {

                                case 1:
                                    maxY = Main.rockLayer + 100;
                                    minY = surface;
                                    minFrequency = 5;
                                    minSpread = 5;
                                    maxFrequency = 8;
                                    maxSpread = 8;

                                    break;
                                case 2:
                                    maxY = 2 * Main.rockLayer;
                                    minY = Main.rockLayer;
                                    minFrequency = 4;
                                    minSpread = 4;
                                    maxFrequency = 6;
                                    maxSpread = 6;
                                    break;
                                case 3:
                                    maxY = Main.UnderworldLayer;
                                    minY = (3 / 2) * Main.rockLayer;
                                    minFrequency = 2;
                                    minSpread = 2;
                                    maxFrequency = 2;
                                    maxSpread = 4;
                                    break;
                                case 4:
                                    maxY = Main.maxTilesY;
                                    minY = Main.UnderworldLayer + 20;
                                    minFrequency = 4;
                                    minSpread = 4;
                                    maxFrequency = 9;
                                    maxSpread = 9;
                                    break;


                                case 5:
                                    minY = Main.rockLayer;
                                    maxY = Main.rockLayer * 2;
                                    minFrequency = 5;
                                    minSpread = 5;
                                    maxFrequency = 9;
                                    maxSpread = 9;
                                    break;
                                case 6:
                                    minY = Main.rockLayer * (3 / 2);
                                    maxY = Main.UnderworldLayer;
                                    minFrequency = 4;
                                    minSpread = 4;
                                    maxFrequency = 7;
                                    maxSpread = 7;
                                    break;
                                case 7:
                                    minY = Main.rockLayer * (5 / 2);
                                    maxY = Main.UnderworldLayer;
                                    minFrequency = 3;
                                    minSpread = 3;
                                    maxFrequency = 5;
                                    maxSpread = 5;
                                    break;
                                case 8:
                                    minY = Main.rockLayer;
                                    maxY = Main.UnderworldLayer;
                                    minFrequency = 5;
                                    minSpread = 5;
                                    maxFrequency = 9;
                                    maxSpread = 9;
                                    break;
                                default:
                                    maxY = Main.rockLayer;
                                    minFrequency = 5;
                                    minSpread = 9;
                                    maxFrequency = 5;
                                    maxSpread = 9;
                                    break;
                            }

                            while ((float)trycount < maxtries)
                            {
                                //Get random number from 100 tiles each side
                                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                                //Gets random number based on minimum spawn point to maximum depth of map
                                int Y = WorldGen.genRand.Next((int)minY, (int)maxY);

                                if (TileValidation(Main.tile[X, Y], oreID))
                                {

                                    WorldGen.OreRunner(X, Y, (double)WorldGen.genRand.Next(minSpread, maxSpread), WorldGen.genRand.Next(minFrequency, maxFrequency), oreID);


                                    if (Main.tile[X, Y].type == oreID)
                                    {

                                        realcount++;
                                        if (realcount == amount) break;
                                    }


                                }
                                trycount++;
                            }

                            InformPlayers();
                            args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} {findore[0]} ores.");
                            if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} {findore[0]} ores.", 71, 8, 185);
                        }
                    }
                    break;
                #endregion
                #region Webs
                case "webs":

                    List<ushort> SpiderWalls = new List<ushort>
                        {
                            WallID.Spider,
                            WallID.SpiderEcho,
                            WallID.SpiderUnsafe
                        };


                    while (trycount < maxtries)
                    {
                        tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                        tryY = WorldGen.genRand.Next(50, Main.UnderworldLayer);
                        int direction = WorldGen.genRand.Next(2);
                        if (direction == 0)
                        {
                            direction = -1;
                        }
                        else
                        {
                            direction = 1;
                        }

                        while (!SpiderWalls.Contains(Main.tile[tryX, tryY].wall))
                        {
                            tryY++;
                            if (tryY >= Main.UnderworldLayer) break;
                        }


                        if ((tryY < Main.UnderworldLayer) && (tryY > 50))
                        {

                            WorldGen.TileRunner(tryX, tryY, (double)WorldGen.genRand.Next(4, 11), WorldGen.genRand.Next(2, 4), 51, true, (float)direction, -1f, false, false);
                            args.Player.SendInfoMessage($"POS X:{tryX}, POS Y:{tryY}");
                            realcount++;
                            if (realcount == amount)
                                break;
                        }
                        trycount++;

                    }

                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} webs.");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} webs.", 71, 8, 185);
                    break;



                #endregion
                #region Trees
                case "trees":
                    var counter = 0;
                    while ((double)counter < (double)Main.maxTilesX * 0.003)
                    {
                        tryX = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                        tryY = WorldGen.genRand.Next(25, 50);
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

                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated trees on the surface!.");
                    TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated trees on the surface!", 71, 8, 185);
                    break;

                #endregion
                #region Shrooms
                case "shrooms":

                    List<ushort> GetShroom(ITile tile)
                    {
                        if (tile.type == TileID.Grass) return new List<ushort> { TileID.Plants, 21, 144 };
                        else if (tile.type == TileID.CorruptGrass) return new List<ushort> { TileID.CorruptPlants, 8, 144 };
                        else if (tile.type == TileID.CrimsonGrass) return new List<ushort> { TileID.CrimsonPlants, 15, 270 };
                        else if (tile.type == TileID.MushroomGrass) return new List<ushort> { TileID.MushroomPlants, 0, (ushort)tile.frameX };
                        else return null;

                    };

                    List<ushort> shroom;

                    ITile blockbelow;


                    while (trycount < maxtries)
                    {
                        tryX = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
                        tryY = WorldGen.genRand.Next((int)Main.worldSurface - 150, Main.UnderworldLayer);

                        blockbelow = Main.tile[tryX, tryY + 1];
                        shroom = GetShroom(blockbelow);

                        while (shroom == null)
                        {
                            tryY--;
                            blockbelow = Main.tile[tryX, tryY + 1];
                            shroom = GetShroom(blockbelow);
                            if (tryY < Main.worldSurface - 50) break;
                        }
                        if (shroom != null && !Main.tile[tryX, tryY].active())
                        {


                            WorldGen.PlaceTile(tryX, tryY, shroom[0], false, true, -1, shroom[1]);
                            Main.tile[tryX, tryY].frameX = (short)shroom[2];
                            if (Main.tile[tryX, tryY].type == shroom[0])
                            {

                                realcount++;
                                if (realcount == amount) break;
                            }
                        }
                        trycount++;
                    }

                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} mushrooms in their biomes!");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} mushrooms in their biomes!", 71, 8, 185);

                    break;
                #endregion
                #region Dungeon
                case "dungeon":


                    tryX = args.Player.TileX; //Gets X tile from the right of the character
                    tryY = args.Player.TileY; //Gets Y Tile from the head tile of the character

                    if ((Main.tile[tryX, tryY + 3].active() || Main.tile[tryX + 1, tryY + 3].active()) && inWorld(tryX, tryY)) // checks if player is standing on tile and the dungeon is not by the world border
                    {
                        WorldGen.MakeDungeon(tryX, tryY);
                        InformPlayers();
                        args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated a Dungeon at your location!");
                        TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated a Dungeon in the world!", 71, 8, 185);

                    }
                    else
                    {
                        args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Failed to create a Dungeon at this location! You have to be standing on a block and away from the edges of the world!");

                    }
                    break;
                #endregion

                /*
                #region Temple
            case "temple":
                TempleBuilder.makeTemple((int)args.Player.X, (int)args.Player.Y);

                InformPlayers();
                break;
            #endregion


                #region Living Tree
                // Needs revision, does not work for some reason
                case "livingtree":

                    tryX = args.Player.TileX;
                    tryY = args.Player.TileY;
                    WorldGen.GrowLivingTree(tryX, tryY+1, true);

                    args.Player.SendSuccessMessage("Attempted to grow a living tree at your position.");
                    InformPlayers();
                    break;

            #endregion
                */
                #region Pyramid
                case "pyramid":

                    tryX = args.Player.TileX;
                    tryY = args.Player.TileY;


                    if ((Main.tile[tryX, tryY + 3].active() || Main.tile[tryX + 1, tryY + 3].active()) && inWorld(tryX, tryY))
                        if (WorldGen.Pyramid(tryX, tryY))
                        {
                            InformPlayers();
                            args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] A Pyramid was Successfully Generated.");
                            TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated a Pyramid in the world!", 71, 8, 185);
                        }
                        else
                        {
                            args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Unexpected Error Occurred, Please contact the administrator!");
                        }
                    else
                    {
                        args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Failed to create a pyramid at this location! You have to be standing on a block and away from the edges of the world!");

                    }
                    break;

                #region Minehouse
                case "minehouse":

                    tryX = args.Player.TileX;
                    tryY = args.Player.TileY;
                    if (tryY > Main.worldSurface && tryY < Main.UnderworldLayer && inWorld(tryX,tryY))
                    {
                        WorldGen.MineHouse(tryX, tryY);
                        
                        InformPlayers();
                        args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] A Minehouse was Successfully Generated.");
                        TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated a Minehouse in the world!", 71, 8, 185);

                    }
                    else
                    {
                        args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Failed to create a Minehouse at this location! You have to be in the cavern layer and not near the world border!");
                    }
                    
                    break;

                #endregion

                #endregion
                #region Default case
                default:
                    args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Invalid Syntax! Please refer to the following options...");
                    PrintOptions(args);
                    break;
                    #endregion
            }

        }









        #endregion


        /*

            #region Hellevator Command
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
                if (args.Parameters.Count == 1)
                    width = Int32.Parse(args.Parameters[0]);
                if (width < 2) width = 2;
                if (width > maxsize) width = maxsize;
                start = meX - (width / 2);
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

                for (cx = start; cx < width + start; cx++)
                {
                    int xc;
                    for (xc = ypos; xc < bottom; xc++)
                    {
                        if ((cx == start) || (cx == width + start - 1))
                        {
                            Main.tile[cx, xc].type = (byte)tl;
                            Main.tile[cx, xc].active(true);
                            Main.tile[cx, xc].slope(0);
                            Main.tile[cx, xc].halfBrick(false);
                        }
                        else
                        {
                            WorldGen.KillTile(cx, xc, false, false, false);
                            Main.tile[cx, xc].wall = (byte)wl;
                        }
                    }
                }

                InformPlayers();
                args.Player.SendSuccessMessage("Going down?");
            }
            #endregion
            
            #region GenIslandHouse Command
            private void DoIslandHouse(CommandArgs args)
            {
                int housestyle;
                if ((!int.TryParse(args.Parameters[0], out housestyle) || args.Parameters[0] == null))
                {
                    args.Player.SendErrorMessage("Arguement was not an integer or no Arguement! /PLACEHOLDER/");
                    housestyle = 1;

                }
                else housestyle = int.Parse(args.Parameters[0]);
                // This is for testing purposes and should be removed in next build.
                args.Player.SendMessage(Convert.ToString(housestyle), 189, 0, 15);


                tryX = args.Player.TileX;
                tryY = args.Player.TileY;
                WorldGen.IslandHouse(tryX, tryY + 1, housestyle);
                args.Player.SendSuccessMessage("Attempted to generate an Island House here.");
                InformPlayers();
            }
            #endregion
            #region GenFloatingIsland Command

            private void DoIsland(CommandArgs args)
            {
                tryX = args.Player.TileX;
                tryY = args.Player.TileY;
                if (tryY <= 50)
                    tryY = 51;
                WorldGen.CloudIsland(tryX, tryY);
                args.Player.SendSuccessMessage("Attempted to generate a floating island at your position");
                InformPlayers();
            }
            #endregion
            #region GenCloudIsland Command
            private void DoCloudIsland(CommandArgs args)
            {
                tryX = args.Player.TileX;
                tryY = args.Player.TileY;
                WorldGen.CloudIsland(tryX, tryY + 9);
                WorldGen.IslandHouse(tryX, tryY + 1, 1);

                args.Player.SendSuccessMessage("Attempted to generate an un-looted floating island at your position.");
                InformPlayers();
            }
            #endregion


            #region GenChests Command
            private void DoChests(CommandArgs args)
            {
                if (args.Parameters.Count == 0 || args.Parameters.Count > 2)
                {
                    args.Player.SendInfoMessage("Usage: /genchests <amount> [gen mode: default/easy/all]");
                }
                int empty = 0;
                int tmpEmpty = 0;
                int chests = 0;
                int maxChests = 1000;

                string setting = "default";
                if (args.Parameters.Count > 1)
                {
                    setting = args.Parameters[1];
                }
                const int maxtries = 100000;
                Int32.TryParse(args.Parameters[0], out chests);
                const int threshold = 100;
                if (!config.UseInfiniteChests)
                {
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
                    args.Player.SendSuccessMessage("Uprooted {0} empty out of {1} chests.", empty, tmpEmpty);
                }
                else
                {

                    try
                    {
                        switch (TShock.Config.StorageType.ToLowerInvariant())
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
                    }
                    catch (Exception ex)
                    {
                        TShock.Log.ConsoleError(ex.ToString());
                    }
                }
                if (chests + tmpEmpty + threshold > maxChests)
                    chests = maxChests - tmpEmpty - threshold;
                if (chests > 0)
                {
                    int chestcount = 0;
                    chestcount = tmpEmpty;
                    int tries = 0;
                    int newcount = 0;
                    while (newcount < chests)
                    {
                        int contain;
                        if (setting == "default")
                        {
                            // Moved item list into a separate .txt file
                            int[] itemID = config.DefaultChestIDs;
                            contain = itemID[WorldGen.genRand.Next(0, itemID.Length)];
                        }
                        else if (setting == "all")
                        {
                            // Updated item list to 1.2.4.1
                            contain = WorldGen.genRand.Next(ItemList[0], ItemList.Last() + 1);
                        }
                        else if (setting == "easy")
                        {
                            contain = WorldGen.genRand.Next(-24, 364);
                        }
                        else
                        {
                            args.Player.SendWarningMessage("Warning! Typo in second argument: {0}", args.Parameters[1]);
                            return;
                        }
                        tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                        tryY = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 200);
                        if (!config.GenInsideProtectedRegions && IsProtected(tryX, tryY))
                            continue;
                        while (!Main.tile[tryX, tryY].active())
                        {
                            tryY++;
                        }
                        tryY--;
                        WorldGen.KillTile(tryX, tryY, false, false, false);
                        WorldGen.KillTile(tryX + 1, tryY, false, false, false);
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
                                    try
                                    {
                                        ChestDB.Query("INSERT INTO Chests (X, Y, Account, Items, WorldID) VALUES (@0, @1, '', @2, @3)",
                                                c.x, c.y, items.ToString(), Main.worldID);
                                    }
                                    catch (Exception ex)
                                    {
                                        TShock.Log.ConsoleError(ex.ToString());
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
                    args.Player.SendSuccessMessage("Generated {0} new chests - {1} total", newcount, chestcount);
                    InformPlayers();
                }
            }
            #endregion
        */
        #region Utils
        #region InformPlayers
        //Updating all players Reloads Tile Sections
        public static void InformPlayers(bool hard = false)
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
                                Netplay.Clients[i].TileSections[j, k] = false;
                            }
                        }
                    }
                }
            }

        }
        #endregion
        #region IsProtected
        bool IsProtected(int x, int y)
        {
            var regions = TShock.Regions.InAreaRegion(x, y);
            foreach (var region in regions)
            {
                if (region.DisableBuild)
                    return true;
            }
            return false;
        }
        bool inWorld(int X, int Y)
        {
            if (X > 200 && X < Main.maxTilesX - 200 && Y > 150) return true;
            else return false;
        }
        #endregion
        List<string> FindMatches(string search, List<string> List)
        {
            var found = new List<string> { };

            if (search == null) return found;
            foreach (string find in List)
            {
                if (find != null)
                {
                    if (search == find)
                    {

                        return new List<string> { find };

                    }
                    if (find.ToLower().StartsWith(search)) found.Add(find);

                }

            }
            return found;
        }
        private List<int> ItemList
        {
            get
            {
                List<int> list = new List<int>();
                var items = typeof(Terraria.ID.ItemID).GetFields();
                for (int i = 0; i < items.Length; i++)
                {
                    list.Add((int)items[i].GetValue(items[i]));
                }
                list.Sort();
                return list;
            }
        }
        #endregion
    }
}
#endregion
