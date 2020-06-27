using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OTAPI;
using Terraria.ID;
using Terraria;

namespace WorldRefill
{
    static class StoredLists
    {
        public static Dictionary<string, ushort> ores = new Dictionary<string, ushort>
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
        public static List<string> getStatueList()
        {



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
            return stlist;
        }
        public static Dictionary<string, int> IslandList = new Dictionary<string, int>
        {

            ["floating"] = 0,
            ["desert"] = 1,
            ["snow"] = 2,
            ["skylake"] = 3


        };
    }
}
