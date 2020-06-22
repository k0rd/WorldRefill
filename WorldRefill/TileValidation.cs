using OTAPI.Tile;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Modules;
using Terraria.UI;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace WorldRefill
{
    class TileValidation
    {
        public static bool onSurface(int X, int Y)
        {
            List<ushort> surfacetiles = new List<ushort>
        {
            TileID.Dirt,
            TileID.SnowBlock,
            TileID.Mud,
            TileID.Sand,
            TileID.IceBlock,
            TileID.Grass,
            TileID.CorruptGrass,
            TileID.CrimsonGrass,
            TileID.HallowedGrass,
            TileID.CorruptIce,
            TileID.HallowedIce,
            TileID.FleshIce,
            TileID.Crimsand,
            TileID.Ebonsand,
            TileID.JungleGrass


        };
            bool violation = true;

            if (!Main.tile[X, Y + 3].active() && !Main.tile[X + 1, Y + 3].active())
            {

                violation = false;
            }//block stand check

            Parallel.For(Y, 0, (i) =>

            {

                if (surfacetiles.Contains(Main.tile[X, i].type))
                {


                    if (Main.tile[X, i].active() && Main.tileSolidTop[Main.tile[X,i].type])
                    {

                        violation = false;
                    }//checks to see if surface blocks are above player

                }
            });
            return violation;
        }
        public static bool inWorld(int X, int Y)
        {
            if (X > 250 && X < Main.maxTilesX - 250 && Y > 125 && Y <= Main.rockLayer) return true; // checks to see if player is near world border
            else return false;
        }

        public static List<string> FindMatches(string search, List<string> List)
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
        public static bool isinNonNaturalStatuePlace(ushort tile)
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
        public static bool StatueTileValidation(int X, int Y)
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
        public static int GetOreTier(ushort ore, List<ushort[]> oreTiers)
        {
            int i = 1;
            foreach (ushort[] tiers in oreTiers)
            {
                if (tiers.Contains(ore)) return i;
                i++;
            }

            return 0;
        }

        public static bool TileOreValidation(ITile tile, ushort ore)
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
        }
        public static List<ushort> GetShroom(ITile tile)
        {
            if (tile.type == TileID.Grass) return new List<ushort> { TileID.Plants, 21, 144 };
            else if (tile.type == TileID.CorruptGrass) return new List<ushort> { TileID.CorruptPlants, 8, 144 };
            else if (tile.type == TileID.CrimsonGrass) return new List<ushort> { TileID.CrimsonPlants, 15, 270 };
            else if (tile.type == TileID.MushroomGrass) return new List<ushort> { TileID.MushroomPlants, 0, (ushort)tile.frameX };
            else return null;
            
        }

        
    }
}
