using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.WorldBuilding;

namespace WorldRefill.MicroBiomeExtensions
{
    class HiveBiomeExtensions : HiveBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            {
                
               
                
                int index1 = 0;
                int[] numArray1 = new int[1000];
                int[] numArray2 = new int[1000];
                Vector2 position1 = origin.ToVector2();
                int num1 = WorldGen.genRand.Next(2, 5);
                if (WorldGen.drunkWorldGen)
                    num1 += WorldGen.genRand.Next(7, 10);
                for (int index2 = 0; index2 < num1; ++index2)
                {
                    Vector2 vector2 = position1;
                    int num2 = WorldGen.genRand.Next(2, 5);
                    for (int index3 = 0; index3 < num2; ++index3)
                        vector2 = CreateHiveTunnel((int)position1.X, (int)position1.Y, WorldGen.genRand);
                    position1 = vector2;
                    numArray1[index1] = (int)position1.X;
                    numArray2[index1] = (int)position1.Y;
                    ++index1;
                }
                FrameOutAllHiveContents(origin, 50);
                for (int index2 = 0; index2 < index1; ++index2)
                {
                    int x1 = numArray1[index2];
                    int y = numArray2[index2];
                    int dir = 1;
                    if (WorldGen.genRand.Next(2) == 0)
                        dir = -1;
                    bool flag = false;
                    while (WorldGen.InWorld(x1, y, 10) && BadSpotForHoneyFall(x1, y))
                    {
                        x1 += dir;
                        if (Math.Abs(x1 - numArray1[index2]) > 50)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        int x2 = x1 + dir;
                        if (!SpotActuallyNotInHive(x2, y))
                        {
                            CreateBlockedHoneyCube(x2, y);
                            CreateDentForHoneyFall(x2, y, dir);
                        }
                    }
                }
                CreateStandForLarva(position1);
                WorldGen.PlaceTile((int)position1.X, (int)position1.Y, 231, true, false, -1, 0);
                if (WorldGen.drunkWorldGen)
                {
                    for (int index2 = 0; index2 < 1000; ++index2)
                    {
                        Vector2 position2 = position1;
                        position2.X += (float)WorldGen.genRand.Next(-50, 51);
                        position2.Y += (float)WorldGen.genRand.Next(-50, 51);
                        if (WorldGen.InWorld((int)position2.X, (int)position2.Y, 0) && (double)Vector2.Distance(position1, position2) > 10.0 && (!Main.tile[(int)position2.X, (int)position2.Y].active() && Main.tile[(int)position2.X, (int)position2.Y].wall == (ushort)86))
                        {
                            CreateStandForLarva(position2);
                            WorldGen.PlaceTile((int)position2.X, (int)position2.Y, 231, true, false, -1, 0);
                            break;
                        }
                    }
                }
                structures.AddProtectedStructure(new Microsoft.Xna.Framework.Rectangle(origin.X - 50, origin.Y - 50, 100, 100), 5);
                return true;
            }
        }
    }
}
