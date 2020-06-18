using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OTAPI.Tile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
using Terraria.GameContent.Biomes;
using Terraria.WorldBuilding;
using System.Security.Cryptography;
using Terraria.GameContent.Generation;
using System.Threading.Tasks;


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

            Config.ReadConfig();

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

        #endregion
        #region Reload Command
        // Config Reload
        private async void OnReload(ReloadEventArgs args)
        {
            if (await Task.Run(() => Config.ReadConfig()))
                args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] {Name} Config reloaded.");

            else
                args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Error reading config. Check log for details.");
            return;
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
            "minehouse",
            "hellevator",
            "world",
            "test"





        };
        private async void PrintOptions(CommandArgs args)
        {
            await Task.Run(() =>
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
            });

        }




        #region Generate Command
        private async void Generate(CommandArgs args)

        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Invalid Syntax! " +
                      "Please refer to the following options...");
                PrintOptions(args);

                return;
            }
            short amount;
            if (structslist.Contains(args.Parameters[0].ToLowerInvariant()))
            {
                amount = 0;

            }
            else
            {



                var amountregex = new Regex(@"\b(?<amount>[1-9]\d*){1,5}$");
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

                try
                {
                    amount = Int16.Parse(amountmatch.Value);
                }
                catch (OverflowException)
                {
                    amount = Int16.MaxValue;
                }

            }



            //maxtries = retry amounts if generation of object fails (this is used to prevent lag issues)

            int realcount;
            int tryX;
            int tryY;
            //realcount = actual amount of objects generated


            List<string> lines;


            switch (args.Parameters[0].ToLowerInvariant())
            {
                #region Life Crystals

                case "crystals":


                    //Attempting to generate Objects
                    realcount = await Task.Run(() => Regen.AsyncGenLifeCrystals(amount));


                    //Notify user on success
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Life Crystals");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Life Crystals", 71, 8, 185); InformPlayers();
                    break;





                #endregion
                #region Pots
                case "pots":


                    realcount = await Task.Run(() => Regen.AsyncGenPots(amount));

                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Pots.");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Pots.", 71, 8, 185);
                    InformPlayers();
                    break;

                #endregion
                #region Orbs
                case "orbs":

                    realcount = await Task.Run(() => Regen.AsyncGenerateOrbs(amount));

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

                    realcount = await Task.Run(() => Regen.AsyncGenerateAltars(amount));

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
                    
                    realcount = await Task.Run(() => Regen.AsyncGenerateCavetraps(amount));
                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Randomized Cave Traps.");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Randomized Cave Traps.", 71, 8, 185);
                    break;



                #endregion
                #region Temple Traps
                case "templetraps":

                    realcount = await Task.Run(() => Regen.AsyncGenerateTempletraps(amount));
                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Randomized Temple Traps.");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Randomized Temple Traps.", 71, 8, 185);
                    break;


                #endregion
                #region Statue Traps
                case "statuetraps":

                    realcount = await Task.Run(() => Regen.AsyncGenerateStatuetraps(amount));

                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Randomized Statue Traps.");
                    TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Randomized Statue Traps.", 71, 8, 185);
                    break;


                #endregion
                #region Lava Traps
                case "lavatraps":


                    realcount = await Task.Run(() => Regen.AsyncGenerateLavatraps(amount));
                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} Lava Traps.");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} Lava Traps.", 71, 8, 185); break;

                #endregion
                #region Sand Traps
                case "sandtraps":


                    realcount = await Task.Run(() => Regen.AsyncGenerateSandtraps(amount));
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





                    if (args.Parameters.Count == 2)
                    {
                        realcount = await Task.Run(() => Regen.AsyncGenerateRandStatues(amount));
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
                        var findStatue = ITileValidation.FindMatches(selStatue, stlist);


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
                            realcount = await Task.Run(() => Regen.AsyncGenerateStatues(amount, statue.X, statue.Y));
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






                        realcount = await Task.Run(() => Regen.AsyncGenerateRandOres(amount, oreTiers, ores));

                        InformPlayers();
                        args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} randomized ores.");
                        if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} randomized ores.", 71, 8, 185);
                        break;
                    }



                    else
                    {
                        var selore = args.Parameters[2].ToLowerInvariant();
                        var searchlist = ores.Keys.ToList();
                        var findore = ITileValidation.FindMatches(selore, searchlist);


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



                            realcount = await Task.Run(() => Regen.AsyncGenerateOre(amount, oreID, oreTiers));

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

                    realcount = await Task.Run(() => Regen.AsyncGenerateWebs(amount, SpiderWalls));

                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} webs.");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} webs.", 71, 8, 185);
                    break;



                #endregion
                #region Trees
                case "trees":

                    await Task.Run(() => Regen.AsyncGenerateTrees());
                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated trees on the surface!.");
                    TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated trees on the surface!", 71, 8, 185);
                    break;

                #endregion
                #region Shrooms
                case "shrooms":







                    realcount = await Task.Run(() => Regen.AsyncGenerateShrooms(amount));

                    InformPlayers();
                    args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated and hid {realcount} mushrooms in their biomes!");
                    if (realcount != 0) TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated and hid {realcount} mushrooms in their biomes!", 71, 8, 185);

                    break;
                #endregion
                #region Dungeon
                case "dungeon":


                    tryX = args.Player.TileX; //Gets X tile from the right of the character
                    tryY = args.Player.TileY; //Gets Y Tile from the head tile of the character


                    if (ITileValidation.inWorld(tryX, tryY) && ITileValidation.onSurface(tryX, tryY)) // checks if player is standing on tile and the dungeon is not by the world border
                    {
                        await Task.Run(() => WorldGen.MakeDungeon(tryX, tryY));
                        InformPlayers();
                        args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] Generated a Dungeon at your location!");
                        TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated a Dungeon in the world!", 71, 8, 185);

                    }
                    else
                    {
                        args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Failed to create a Dungeon at this location! You have to be standing on the surface and away from the edges of the world!");

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


                    if (ITileValidation.inWorld(tryX, tryY) && ITileValidation.onSurface(tryX, tryY))
                        if (await Task.Run(() => WorldGen.Pyramid(tryX, tryY)))
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
                        args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Failed to create a Pyramid at this location! You have to be standing on the surface and away from the edges of the world!");

                    }


                    break;
                #endregion
                #region Minehouse
                case "minehouse":

                    tryX = args.Player.TileX;
                    tryY = args.Player.TileY;
                    if (tryY > Main.worldSurface && tryY < Main.UnderworldLayer && ITileValidation.inWorld(tryX, tryY))
                    {
                        await Task.Run(() => WorldGen.MineHouse(tryX, tryY));

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
                #region Hellevator
                case "hellevator":

                    List<ushort> trees = new List<ushort>
                    {
                        TileID.Trees,
                        TileID.TreeAmber,
                        TileID.TreeAmethyst,
                        TileID.TreeDiamond,
                        TileID.TreeEmerald,
                        TileID.TreeRuby,
                        TileID.TreeSapphire,
                        TileID.TreeTopaz,
                        TileID.MushroomTrees,
                        TileID.PalmTree,
                        TileID.VanityTreeYellowWillow,
                        TileID.VanityTreeSakura
                    };
                    int posX = args.Player.TileX;
                    int posY = args.Player.TileY;

                    if (ITileValidation.onSurface(posX, posY) && ITileValidation.inWorld(posX, posY))
                    {
                        await Task.Run(() => Regen.AsyncGenerateHellevator(args, posX, posY, trees));
                        InformPlayers();
                        args.Player.SendSuccessMessage($"[[c/FFFFFF:{Name}]] A Hellevator was Successfully Generated.");
                        TSPlayer.All.SendMessage($"[[c/FFFFFF:{Name}]] [c/BCFF00:{args.Player.Name}] has generated a Hellevator at X: [c/BCFF00:{posX}], Y: [c/BCFF00:{posY}] !", 71, 8, 185);
                    }

                    else args.Player.SendErrorMessage($"[[c/FFFFFF:{Name}]] Failed to create a Hellevator at this location! You have to be standing on the surface and away from the edges of the world!");
                    break;
                #endregion
                #region World
                case "world":

                    if (args.Parameters.Count < 2)
                    {
                        args.Player.SendInfoMessage($"[[c/FFFFFF:{Name}]] This will delete the world and replace it with a new one are you sure?");
                        args.Player.SendInfoMessage($"[[c/FFFFFF:{Name}]] To confirm please put /gen world true");
                        break;
                    }
                    bool confirm = bool.Parse(args.Parameters[1].ToLowerInvariant());



                    if (confirm == false)
                    {
                        args.Player.SendInfoMessage($"[[c/FFFFFF:{Name}]] This will delete the world and replace it with a new one are you sure?");
                        args.Player.SendInfoMessage($"[[c/FFFFFF:{Name}]] To confirm please put /gen world true");
                    }
                    else
                    {

                        args.Player.SendInfoMessage($"[[c/FFFFFF:{Name}]] This one may take a while to load. Please wait... DO NOT DROP ANY ITEMS");

                        await Task.Run(() =>
                         {
                             WorldGen.clearWorld();
                             WorldGen.GenerateWorld(WorldGen._lastSeed);



                         });
                        InformPlayers();
                        foreach (TSPlayer plr in TShock.Players)
                        {
                            if (plr != null && Main.tile[plr.TileX, plr.TileY].active())
                            {
                                plr.Teleport(Main.spawnTileX * 16, (Main.spawnTileY * 16) - 48);
                            }

                        }
                    }

                    break;



                #endregion
                case "test":
                    args.Player.SendInfoMessage($"SURFACE :{Main.worldSurface}, CAVERN:{Main.rockLayer}, HELL:{Main.UnderworldLayer}");
                    break;


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

        #endregion
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
