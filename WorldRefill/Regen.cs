using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OTAPI.Tile;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TShockAPI;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using TerrariaApi.Server;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace WorldRefill
{
    static class Regen
    {

        
        public static Task AsyncGenLifeCrystals(short amount)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            return Task.Run(() =>
            {
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)
                {
                    if (WorldGen.AddLifeCrystal(WorldGen.genRand.Next(1, Main.maxTilesX), WorldGen.genRand.Next((int)(Main.rockLayer), (int)(Main.UnderworldLayer + 100))))
                    {

                        realcount++;
                        //Determine if enough Objects have been generated
                        if (realcount == amount) break;
                    }

                }

                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());
            }
        public static Task AsyncGenPots(short amount)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            return Task.Run(() =>
            {
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)
                {
                    int tryX = WorldGen.genRand.Next(1, Main.maxTilesX);

                    int tryY = WorldGen.genRand.Next((int)Main.worldSurface - 5, Main.maxTilesY - 20);



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
                        if (realcount == amount) break;


                    }

                }
                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());
            }
        public static Task AsyncGenerateOrbs(short amount)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            return Task.Run(() =>
            {
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)
                {
                    int tryX = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                    int tryY = WorldGen.genRand.Next((int)Main.worldSurface + 20, Main.UnderworldLayer);

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



                }
                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());
        }
        public static Task AsyncGenerateAltars(short amount)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            return Task.Run(() =>
            {
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)
                {
                    int tryX = WorldGen.genRand.Next(1, Main.maxTilesX);
                    int tryY = WorldGen.genRand.Next((int)Main.worldSurface + 10, (int)Main.rockLayer);

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

                }
                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());
        }
        public static Task AsyncGenerateCavetraps(short amount)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            return Task.Run(() =>
            {
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)
                {
                    int tryX = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
                    int tryY = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer - 100);
                    var type = WorldGen.genRand.Next(-1, 1);
                    if (Main.tile[tryX, tryY].wall == WallID.None && WorldGen.placeTrap(tryX, tryY, type))
                    {
                        realcount++;
                        if (realcount == amount)
                            break;
                    }


                }
                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());
        }
        public static Task AsyncGenerateTempletraps(short amount)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            return Task.Run(() =>
            {
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)

                {
                    int tryX = WorldGen.genRand.Next(250, Main.maxTilesX - 250);
                    int tryY = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);


                    if (WorldGen.mayanTrap(tryX, tryY))
                    {
                        realcount++;
                        if (realcount == amount) break;
                    }





                }
                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());
        }
       
        public static Task AsyncGenerateLavatraps(short amount)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            return Task.Run(() =>
            {
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)

                {
                    int tryX = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                    int tryY = WorldGen.genRand.Next(750, Main.UnderworldLayer);




                    if (WorldGen.placeLavaTrap(tryX, tryY))
                    {

                        realcount++;
                        if (realcount == amount) break;
                    }

                }
                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());
        }
        public static Task AsyncGenerateSandtraps(short amount)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            return Task.Run(() =>
            {
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)

                {
                    int tryX = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                    int tryY = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);




                    if (WorldGen.PlaceSandTrap(tryX, tryY))
                    {

                        realcount++;
                        if (realcount == amount) break;
                    }


                }
                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());
        }
        public static Task AsyncGenerateRandStatues(short amount)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            return Task.Run(() =>
            {
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)
                {

                    int tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                    int tryY = WorldGen.genRand.Next((int)Main.rockLayer, Main.UnderworldLayer);
                    int tryType = WorldGen.genRand.Next(0, WorldGen.statueList.Count() - 1);
                    Point16 randstatue = WorldGen.statueList[tryType];






                    while (!TileValidation.StatueTileValidation(tryX, tryY))
                    {
                        tryY++;
                        if (tryY >= Main.UnderworldLayer)
                        {
                            break;
                        }
                    }

                    if (tryY < Main.UnderworldLayer && (!TileValidation.isinNonNaturalStatuePlace(Main.tile[tryX, tryY + 1].type)))
                    {



                        WorldGen.PlaceTile(tryX, tryY, randstatue.X, true, true, -1, randstatue.Y);

                        if (Main.tile[tryX, tryY].type == randstatue.X)
                        {

                            realcount++;
                            if (realcount == amount)
                                break;
                        }
                    }

                }
                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());
        }
        public static Task AsyncGenerateStatues(short amount, short tileid = 0, short style = 0)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            return Task.Run(() =>
            {
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)
                {

                    int tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                    int tryY = WorldGen.genRand.Next((int)Main.rockLayer, Main.UnderworldLayer);
                    int tryType = WorldGen.genRand.Next(0, WorldGen.statueList.Count() - 1);







                    while (!TileValidation.StatueTileValidation(tryX, tryY))
                    {
                        tryY++;
                        if (tryY >= Main.UnderworldLayer)
                        {
                            break;
                        }
                    }

                    if (tryY < Main.UnderworldLayer && (!TileValidation.isinNonNaturalStatuePlace(Main.tile[tryX, tryY + 1].type)))
                    {



                        WorldGen.PlaceTile(tryX, tryY, tileid, true, true, -1, style);

                        if (Main.tile[tryX, tryY].type == tileid)
                        {

                            realcount++;
                            if (realcount == amount)
                                break;
                        }
                    }

                }
                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());
        }
        public static Task AsyncGenerateRandOres(short amount, List<ushort[]> oreTiers)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            int oreTier;
            int minFrequency;
            int maxFrequency;
            int minSpread;
            int maxSpread;
            List<ushort> totalores = StoredLists.ores.Values.ToList<ushort>();

            return Task.Run(() =>
            {
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)
                {

                    int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);

                    double maxY;
                    double minY = Main.worldSurface - 20;
                    var trytype = WorldGen.genRand.Next(0, totalores.Count - 1);

                    ushort oreID = totalores[trytype];
                    if (WorldGen.crimson && oreID == TileID.Demonite) oreID = TileID.Crimtane; //If randomly generated ore, make the ore world specific.
                    else if (!WorldGen.crimson && oreID == TileID.Crimtane) oreID = TileID.Demonite;

                    oreTier = TileValidation.GetOreTier(oreID, oreTiers);

                    switch (oreTier)
                    {

                        case 1:
                            maxY = Main.rockLayer;
                            minY = Main.worldSurface;
                            minFrequency = 5;
                            minSpread = 5;
                            maxFrequency = 8;
                            maxSpread = 8;

                            break;
                        case 2:
                            maxY = Main.rockLayer * 3 / 2;
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
                            maxFrequency = 4;
                            maxSpread = 4;
                            break;
                        case 4:
                            maxY = Main.maxTilesY - 1;
                            minY = Main.UnderworldLayer + 20;
                            minFrequency = 4;
                            minSpread = 4;
                            maxFrequency = 9;
                            maxSpread = 9;
                            break;


                        case 5:
                            minY = Main.rockLayer;
                            maxY = Main.rockLayer * (3 / 2);
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
                            minY = Main.rockLayer * (5 / 3);
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
                            minSpread = 5;
                            maxFrequency = 9;
                            maxSpread = 9;
                            break;


                    }

                    //Gets random number based on minimum spawn point to maximum depth of map
                    int Y = WorldGen.genRand.Next((int)minY, (int)maxY);

                    if (TileValidation.TileOreValidation(Main.tile[X, Y], oreID))
                    {
                        WorldGen.OreRunner(X, Y, (double)WorldGen.genRand.Next(minSpread, maxSpread), WorldGen.genRand.Next(minFrequency, maxFrequency), oreID);



                        if (Main.tile[X, Y].type == oreID)
                        {

                            realcount++;
                            if (realcount == amount) break;
                        }
                    }

                }
                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());
        }
        public static Task AsyncGenerateOres(short amount, ushort oreID, List<ushort[]> oreTiers)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            int oreTier;
            int minFrequency;
            int maxFrequency;
            int minSpread;
            int maxSpread;

            //oreGened = track amount of ores generated already
            double maxY;
            double minY = Main.worldSurface + 50;
            //Rare Ores  - Adamantite (Titanium), Demonite, Diamond, Chlorophyte
            return Task.Run(() =>
            {
                oreTier = TileValidation.GetOreTier(oreID, oreTiers);

                switch (oreTier)
                {

                    case 1:
                        maxY = Main.rockLayer;
                        minY = Main.worldSurface;
                        minFrequency = 5;
                        minSpread = 5;
                        maxFrequency = 8;
                        maxSpread = 8;

                        break;
                    case 2:
                        maxY = Main.rockLayer * 3 / 2;
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
                        maxFrequency = 4;
                        maxSpread = 4;
                        break;
                    case 4:
                        maxY = Main.maxTilesY - 1;
                        minY = Main.UnderworldLayer + 20;
                        minFrequency = 4;
                        minSpread = 4;
                        maxFrequency = 9;
                        maxSpread = 9;
                        break;


                    case 5:
                        minY = Main.rockLayer;
                        maxY = Main.rockLayer * (3 / 2);
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
                        minY = Main.rockLayer * (5 / 3);
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
                        minSpread = 5;
                        maxFrequency = 9;
                        maxSpread = 9;
                        break;


                }
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)
                {
                    //Get random number from 100 tiles each side
                    int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                    //Gets random number based on minimum spawn point to maximum depth of map
                    int Y = WorldGen.genRand.Next((int)minY, (int)maxY);

                    if (TileValidation.TileOreValidation(Main.tile[X, Y], oreID))
                    {

                        WorldGen.OreRunner(X, Y, (double)WorldGen.genRand.Next(minSpread, maxSpread), WorldGen.genRand.Next(minFrequency, maxFrequency), oreID);


                        if (Main.tile[X, Y].type == oreID)
                        {

                            realcount++;
                            if (realcount == amount) break;
                        }


                    }

                }
                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());
        }
        public static Task AsyncGenerateWebs(short amount, List<ushort> SpiderWalls)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            return Task.Run(() => {
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)
                {
                    int tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
                    int tryY = WorldGen.genRand.Next(50, Main.UnderworldLayer);
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

                        realcount++;
                        if (realcount == amount)
                            break;
                    }
                    trycount++;

                }
                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());
        }
        public static Task AsyncGenerateTrees()
        {
            WorldRefill.isTaskRunning = true;
            WorldRefill.realcount = 1;
            int count = 0;
            for (int counter = 0; counter < Main.maxTilesX * 0.003; counter++)
            {
                int tryX = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                int tryY = WorldGen.genRand.Next(25, 50);
                for (var tick = tryX - tryY; tick < tryX + tryY; tick++)
                {

                    for (int offset = 20; offset < Main.worldSurface; offset++)
                    {
                        WorldGen.GrowEpicTree(tick, offset);
                        count++;
                    }
                }

            }
            return Task.Run(() => WorldGen.AddTrees()).ContinueWith((d) => FinishGen());

        }
        public static Task AsyncGenerateShrooms(short amount)
        {
            WorldRefill.isTaskRunning = true;
            int realcount = 0;
            return Task.Run(() =>
            {
                for (int trycount = 0; trycount <= Config.ConfigFile.GenerationMaxTries; trycount++)
                {
                    int tryX = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
                    int tryY = WorldGen.genRand.Next((int)Main.worldSurface - 150, Main.UnderworldLayer);

                    ITile blockbelow = Main.tile[tryX, tryY + 1];
                    List<ushort> shroom = TileValidation.GetShroom(blockbelow);

                    while (shroom == null)
                    {
                        tryY--;
                        blockbelow = Main.tile[tryX, tryY + 1];
                        shroom = TileValidation.GetShroom(blockbelow);
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

                }
                WorldRefill.realcount = realcount;
            }).ContinueWith((d) => FinishGen());

        }
        public static Task AsyncGenerateHellevator(int posX, int posY, List<ushort> trees)
        {
            WorldRefill.isTaskRunning = true;
            WorldRefill.realcount = 1;



            Task<int> bottom()
            {
                int hell;
                int xtile;



                for (hell = Main.UnderworldLayer + 10; hell <= Main.maxTilesY - 100; hell++)
                {
                    xtile = posX;
                    Parallel.For(posX, posX + 8, (cwidth, state) =>
                    {
                        if (Main.tile[cwidth, hell].active() && !Main.tile[cwidth,hell].lava())
                        {
                            state.Stop();

                            xtile = cwidth;

                            return;
                        }
                    });

                    if (!Main.tile[xtile, hell].active()) break;
                }

                return Task.FromResult(hell);

            };






            Dictionary<string, int> hellevator = new Dictionary<string, int>
            { // Using a dictionary to keep track of hellevator information, instead of using variables.
                ["X"] = posX,
                ["Y"] = posY + 3,
                ["Width"] = 7,
                ["Bottom"] = bottom().Result


            };
            hellevator.TryGetValue("X", out int Xstart);
            hellevator.TryGetValue("Y", out int Ystart);
            hellevator.TryGetValue("Width", out int Width);
            hellevator.TryGetValue("Bottom", out int Bottom);

            int count = 0;
            ushort tile = TileID.ObsidianBrick;
            ushort wall = WallID.StarlitHeavenWallpaper;
            return Task.Run(() =>
            {
                Parallel.For(Xstart, Xstart + Width, (cx) =>
                {

                    Parallel.For(Ystart - 10, Ystart, (cy) =>
                        {
                            if (Main.tile[cx, cy].active())
                            {
                                WorldGen.KillTile(cx, cy, false, false, true);
                            }

                        });



                Parallel.For(Ystart, Bottom, (cy) =>
                {
                    if ((cx == Xstart) || (cx == Width + Xstart - 1))
                    {
                        Main.tile[cx, cy].type = tile;
                        Main.tile[cx, cy].active(true);
                        Main.tile[cx, cy].slope(0);
                        Main.tile[cx, cy].halfBrick(false);
                    }
                    else
                    {
                        if (trees.Contains(Main.tile[cx, cy - 1].type))
                        {
                            WorldGen.KillTile(cx, cy - 1);
                            count++;
                        }
                        WorldGen.KillTile(cx, cy, false, false, true);
                        count++;
                        Main.tile[cx, cy].wall = wall;
                        if ((cx == Xstart + 1 || cx == Width + Xstart - 2) && cy != Ystart)
                        {
                            Main.tile[cx, cy].type = TileID.WebRope;
                            Main.tile[cx, cy].active(true);
                            Main.tile[cx, cy].slope(0);
                            Main.tile[cx, cy].halfBrick(false);
                        }
                    }
                });
                WorldGen.PlaceTile(cx, Ystart, TileID.Platforms, false, false, -1, 13);

                


                });
            }).ContinueWith((d) => FinishGen());

        }
        public static Task AsyncGenerateIsland(int island, int posX, int posY)
        {
            WorldRefill.isTaskRunning = true;
            WorldRefill.realcount = 1;
            return Task.Run(() =>
            {
                switch (island)
                {
                    case 0:
                        WorldGen.CloudIsland(posX, posY);
                        
                        for(int x = posX-20; x< posX+20; x++)
                        {
                            int cloudsurface()
                            {
                                int cY = posY;
                                while (Main.tile[x, cY].active())
                                {
                                    cY--;
                                }
                                cY++;
                                return cY;

                            };
                            WorldGen.SpreadGrassDirect(posX, cloudsurface());
                            
                        }
                        
                           
                        
                        WorldGen.IslandHouse(posX, posY - 6, WorldGen.floatingIslandStyle[WorldGen.genRand.Next(0, WorldGen.floatingIslandStyle.Count() - 1)]);

                        break;
                    case 1:
                        WorldGen.DesertCloudIsland(posX, posY);
                        WorldGen.IslandHouse(posX, posY - 6, WorldGen.floatingIslandStyle[WorldGen.genRand.Next(0, WorldGen.floatingIslandStyle.Count() - 1)]);

                        break;

                    case 2:

                        WorldGen.SnowCloudIsland(posX, posY);
                        WorldGen.IslandHouse(posX, posY - 6, WorldGen.floatingIslandStyle[WorldGen.genRand.Next(0, WorldGen.floatingIslandStyle.Count() - 1)]);


                        break;
                    case 3:
                        WorldGen.CloudLake(posX, posY);
                        break;


                }
            }).ContinueWith((d) => FinishGen());
        }
        public static Task AsyncGenerateDungeon(int posX, int posY)
        {
            WorldRefill.isTaskRunning = true;
            WorldRefill.realcount = 1;

            return Task.Run(() => WorldGen.MakeDungeon(posX, posY)).ContinueWith((d) => FinishGen());



        }
        public static Task<bool> AsyncGeneratePyramid(int posX, int posY)
        {
            WorldRefill.isTaskRunning = true;
            WorldRefill.realcount = 1;
            Task.Run(() => WorldGen.Pyramid(posX, posY)).ContinueWith((d) => FinishGen());
            return Task.FromResult(WorldGen.Pyramid(posX, posY));
        }
        public static Task AsyncGenerateMinehouse(int posX, int posY)
        {
            WorldRefill.isTaskRunning = true;
            WorldRefill.realcount = 1;
            return Task.Run(() => WorldGen.MineHouse(posX, posY)).ContinueWith((d) => FinishGen());
        }
        public static Task AsyncGenerateWorld(int seed)
        {
            WorldRefill.isTaskRunning = true;
            WorldRefill.realcount = 1;
            
                if (Config.ConfigFile.UseInfiniteChests)
                {
                    InfChestsDatabase WRConn = new InfChestsDatabase();
                    WRConn.DeleteChests(Main.worldID);
                }
            
            WorldGen.clearWorld();
           
            return Task.Run(() => WorldGen.GenerateWorld(seed)).ContinueWith((d) => FinishGen());

        }
        public static void FinishGen()
        {
            WorldRefill.isTaskRunning = false;
            
            if (WorldRefill.realcount != 0)
            {
                foreach (TSPlayer player in TShock.Players)
                {
                    if (player != null)
                    {
                        NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(player.TPlayer.position, 1, 0, 10, -16));
                        
                    }
                }
            }
            if (Config.ConfigFile.UseInfiniteChests)
            {
                InfChestsDatabase WRConn = new InfChestsDatabase();
                WRConn.AddChests();
                Console.WriteLine("Converted Some Chests");
                TShock.Utils.SaveWorld();
            }
            
        }

    }
}
