using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WorldRefill.MicroBiomeExtensions
{
    class WorldGenExtensions : WorldGen
    {
        public static new void GrowLivingTree(int i, int j, bool patch = false)
        {
            int index1 = 0;
            int[] numArray1 = new int[1000];
            int[] numArray2 = new int[1000];
            int[] numArray3 = new int[1000];
            int[] numArray4 = new int[1000];
            int index2 = 0;
            int[] numArray5 = new int[2000];
            int[] numArray6 = new int[2000];
            bool[] flagArray = new bool[2000];
            
            int num1 = i - WorldGen.genRand.Next(2, 3);
            int num2 = i + WorldGen.genRand.Next(2, 3);
            if (WorldGen.genRand.Next(5) == 0)
            {
                if (WorldGen.genRand.Next(2) == 0)
                    --num1;
                else
                    ++num2;
            }
            int num3 = num2 - num1;
            bool flag1 = num3 >= 4;
            int num4 = i - 50;
            int num5 = i + 50;
            if (patch)
            {
                num4 = i - 20;
                num5 = i + 20;
                num1 = i - WorldGen.genRand.Next(1, 3);
                num2 = i + WorldGen.genRand.Next(1, 3);
                flag1 = num3 >= 4;
            }
            for (int index3 = num4; index3 <= num5; ++index3)
            {
                for (int index4 = 5; index4 < j - 5; ++index4)
                {
                    if (Main.tile[index3, index4].active())
                    {
                        
                        switch (Main.tile[index3, index4].type)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 191:
                            case 192:
                                continue;
                            default:
                                continue;
                        }
                    }
                }
            }
            int num6 = num1;
            int num7 = num2;
            int minl = num1;
            int minr = num2;
            bool flag2 = true;
            int num8 = WorldGen.genRand.Next(-8, -4);
            int num9 = WorldGen.genRand.Next(2);
            int index5 = j;
            int num10 = WorldGen.genRand.Next(5, 15);
            Main.tileSolid[48] = false;
            while (flag2)
            {
                ++num8;
                if (num8 > num10)
                {
                    num10 = WorldGen.genRand.Next(5, 15);
                    num8 = 0;
                    numArray2[index1] = index5 + WorldGen.genRand.Next(5);
                    if (WorldGen.genRand.Next(5) == 0)
                        num9 = num9 != 0 ? 0 : 1;
                    if (num9 == 0)
                    {
                        numArray3[index1] = -1;
                        numArray1[index1] = num1;
                        numArray4[index1] = num2 - num1;
                        if (WorldGen.genRand.Next(2) == 0)
                            ++num1;
                        ++num6;
                        num9 = 1;
                    }
                    else
                    {
                        numArray3[index1] = 1;
                        numArray1[index1] = num2;
                        numArray4[index1] = num2 - num1;
                        if (WorldGen.genRand.Next(2) == 0)
                            --num2;
                        --num7;
                        num9 = 0;
                    }
                    if (num6 == num7)
                        flag2 = false;
                    ++index1;
                }
                for (int index3 = num1; index3 <= num2; ++index3)
                {
                    Main.tile[index3, index5].type = (ushort)191;
                    Main.tile[index3, index5].active(true);
                    Main.tile[index3, index5].halfBrick(false);
                }
                --index5;
            }
            for (int index3 = 0; index3 < index1 - 1; ++index3)
            {
                int index4 = numArray1[index3] + numArray3[index3];
                int index6 = numArray2[index3];
                int num11 = (int)((double)numArray4[index3] * (1.0 + (double)WorldGen.genRand.Next(20, 30) * 0.100000001490116));
                Main.tile[index4, index6 + 1].type = (ushort)191;
                Main.tile[index4, index6 + 1].active(true);
                Main.tile[index4, index6 + 1].halfBrick(false);
                int num12 = WorldGen.genRand.Next(3, 5);
                while (num11 > 0)
                {
                    --num11;
                    Main.tile[index4, index6].type = (ushort)191;
                    Main.tile[index4, index6].active(true);
                    Main.tile[index4, index6].halfBrick(false);
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        if (WorldGen.genRand.Next(2) == 0)
                            --index6;
                        else
                            ++index6;
                    }
                    else
                        index4 += numArray3[index3];
                    if (num12 > 0)
                        --num12;
                    else if (WorldGen.genRand.Next(2) == 0)
                    {
                        num12 = WorldGen.genRand.Next(2, 5);
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            Main.tile[index4, index6].type = (ushort)191;
                            Main.tile[index4, index6].active(true);
                            Main.tile[index4, index6].halfBrick(false);
                            Main.tile[index4, index6 - 1].type = (ushort)191;
                            Main.tile[index4, index6 - 1].active(true);
                            Main.tile[index4, index6 - 1].halfBrick(false);
                            numArray5[index2] = index4;
                            numArray6[index2] = index6;
                            ++index2;
                        }
                        else
                        {
                            Main.tile[index4, index6].type = (ushort)191;
                            Main.tile[index4, index6].active(true);
                            Main.tile[index4, index6].halfBrick(false);
                            Main.tile[index4, index6 + 1].type = (ushort)191;
                            Main.tile[index4, index6 + 1].active(true);
                            Main.tile[index4, index6 + 1].halfBrick(false);
                            numArray5[index2] = index4;
                            numArray6[index2] = index6;
                            ++index2;
                        }
                    }
                    if (num11 == 0)
                    {
                        numArray5[index2] = index4;
                        numArray6[index2] = index6;
                        ++index2;
                    }
                }
            }
            int index7 = (num1 + num2) / 2;
            int index8 = index5;
            int num13 = WorldGen.genRand.Next(num3 * 3, num3 * 5);
            int num14 = 0;
            int num15 = 0;
            for (; num13 > 0; --num13)
            {
                Main.tile[index7, index8].type = (ushort)191;
                Main.tile[index7, index8].active(true);
                Main.tile[index7, index8].halfBrick(false);
                if (num14 > 0)
                    --num14;
                if (num15 > 0)
                    --num15;
                for (int index3 = -1; index3 < 2; ++index3)
                {
                    if (index3 != 0 && (index3 < 0 && num14 == 0 || index3 > 0 && num15 == 0) && WorldGen.genRand.Next(2) == 0)
                    {
                        int index4 = index7;
                        int index6 = index8;
                        int num11 = WorldGen.genRand.Next(num3, num3 * 3);
                        if (index3 < 0)
                            num14 = WorldGen.genRand.Next(3, 5);
                        if (index3 > 0)
                            num15 = WorldGen.genRand.Next(3, 5);
                        int num12 = 0;
                        while (num11 > 0)
                        {
                            --num11;
                            index4 += index3;
                            Main.tile[index4, index6].type = (ushort)191;
                            Main.tile[index4, index6].active(true);
                            Main.tile[index4, index6].halfBrick(false);
                            if (num11 == 0)
                            {
                                numArray5[index2] = index4;
                                numArray6[index2] = index6;
                                flagArray[index2] = true;
                                ++index2;
                            }
                            if (WorldGen.genRand.Next(5) == 0)
                            {
                                if (WorldGen.genRand.Next(2) == 0)
                                    --index6;
                                else
                                    ++index6;
                                Main.tile[index4, index6].type = (ushort)191;
                                Main.tile[index4, index6].active(true);
                                Main.tile[index4, index6].halfBrick(false);
                            }
                            if (num12 > 0)
                                --num12;
                            else if (WorldGen.genRand.Next(3) == 0)
                            {
                                num12 = WorldGen.genRand.Next(2, 4);
                                int index9 = index4;
                                int num16 = index6;
                                int index10 = WorldGen.genRand.Next(2) != 0 ? num16 + 1 : num16 - 1;
                                Main.tile[index9, index10].type = (ushort)191;
                                Main.tile[index9, index10].active(true);
                                Main.tile[index9, index10].halfBrick(false);
                                numArray5[index2] = index9;
                                numArray6[index2] = index10;
                                flagArray[index2] = true;
                                int index11 = index2 + 1;
                                numArray5[index11] = index9 + WorldGen.genRand.Next(-5, 6);
                                numArray6[index11] = index10 + WorldGen.genRand.Next(-5, 6);
                                flagArray[index11] = true;
                                index2 = index11 + 1;
                            }
                        }
                    }
                }
                numArray5[index2] = index7;
                numArray6[index2] = index8;
                ++index2;
                if (WorldGen.genRand.Next(4) == 0)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                        --index7;
                    else
                        ++index7;
                    Main.tile[index7, index8].type = (ushort)191;
                    Main.tile[index7, index8].active(true);
                    Main.tile[index7, index8].halfBrick(false);
                }
                --index8;
            }
            for (int i1 = minl; i1 <= minr; ++i1)
            {
                int num11 = WorldGen.genRand.Next(1, 6);
                int j1 = j + 1;
                while (num11 > 0)
                {
                    if (WorldGen.SolidTile(i1, j1, false))
                        --num11;
                    Main.tile[i1, j1].type = (ushort)191;
                    Main.tile[i1, j1].active(true);
                    Main.tile[i1, j1].halfBrick(false);
                    ++j1;
                }
                int num12 = j1;
                int num16 = WorldGen.genRand.Next(2, num3 + 1);
                for (int index3 = 0; index3 < num16; ++index3)
                {
                    int index4 = num12;
                    int num17 = (minl + minr) / 2;
                    int num18 = 1;
                    int num19 = i1 >= num17 ? 1 : -1;
                    if (i1 == num17 || num3 > 6 && (i1 == num17 - 1 || i1 == num17 + 1))
                        num19 = 0;
                    int num20 = num19;
                    int index6 = i1;
                    int num21 = WorldGen.genRand.Next((int)((double)num3 * 3.5), num3 * 6);
                    while (num21 > 0)
                    {
                        --num21;
                        index6 += num19;
                        if (Main.tile[index6, index4].wall != (ushort)244)
                        {
                            Main.tile[index6, index4].type = (ushort)191;
                            Main.tile[index6, index4].active(true);
                            Main.tile[index6, index4].halfBrick(false);
                        }
                        index4 += num18;
                        if (Main.tile[index6, index4].wall != (ushort)244)
                        {
                            Main.tile[index6, index4].type = (ushort)191;
                            Main.tile[index6, index4].active(true);
                            Main.tile[index6, index4].halfBrick(false);
                        }
                        if (!Main.tile[index6, index4 + 1].active())
                        {
                            num19 = 0;
                            num18 = 1;
                        }
                        if (WorldGen.genRand.Next(3) == 0)
                            num19 = num20 >= 0 ? (num20 <= 0 ? WorldGen.genRand.Next(-1, 2) : (num19 != 0 ? 0 : 1)) : (num19 != 0 ? 0 : -1);
                        if (WorldGen.genRand.Next(3) == 0)
                            num18 = num18 != 0 ? 0 : 1;
                    }
                }
            }
            for (int index3 = 0; index3 < index2; ++index3)
            {
                int num11 = (int)((double)WorldGen.genRand.Next(5, 8) * (1.0 + (double)num3 * 0.0500000007450581));
                if (flagArray[index3])
                    num11 = WorldGen.genRand.Next(6, 12) + num3;
                int num12 = numArray5[index3] - num11 * 2;
                int num16 = numArray5[index3] + num11 * 2;
                int num17 = numArray6[index3] - num11 * 2;
                int num18 = numArray6[index3] + num11 * 2;
                float num19 = (float)(2.0 - (double)WorldGen.genRand.Next(5) * 0.100000001490116);
                for (int i1 = num12; i1 <= num16; ++i1)
                {
                    for (int index4 = num17; index4 <= num18; ++index4)
                    {
                        if (Main.tile[i1, index4].type != (ushort)191)
                        {
                            if (flagArray[index3])
                            {
                                if ((double)(new Vector2((float)numArray5[index3], (float)numArray6[index3]) - new Vector2((float)i1, (float)index4)).Length() < (double)num11 * 0.9)
                                {
                                    Main.tile[i1, index4].type = (ushort)192;
                                    Main.tile[i1, index4].active(true);
                                    Main.tile[i1, index4].halfBrick(false);
                                }
                            }
                            else if ((double)Math.Abs(numArray5[index3] - i1) + (double)Math.Abs(numArray6[index3] - index4) * (double)num19 < (double)num11)
                            {
                                Main.tile[i1, index4].type = (ushort)192;
                                Main.tile[i1, index4].active(true);
                                Main.tile[i1, index4].halfBrick(false);
                            }
                        }
                    }
                    if (WorldGen.genRand.Next(30) == 0)
                    {
                        int j1 = num17;
                        if (!Main.tile[i1, j1].active())
                        {
                            while (!Main.tile[i1, j1 + 1].active() && j1 < num18)
                                ++j1;
                            if (Main.tile[i1, j1 + 1].type == (ushort)192)
                                WorldGen.PlaceTile(i1, j1, 187, true, false, -1, WorldGen.genRand.Next(50, 52));
                        }
                    }
                    if (!flagArray[index3] && WorldGen.genRand.Next(15) == 0)
                    {
                        int j1 = num18;
                        int num20 = j1 + 100;
                        if (!Main.tile[i1, j1].active())
                        {
                            while (!Main.tile[i1, j1 + 1].active() && j1 < num20)
                                ++j1;
                            if (Main.tile[i1, j1 + 1].type != (ushort)192)
                            {
                                if (WorldGen.genRand.Next(2) == 0)
                                {
                                    WorldGen.PlaceTile(i1, j1, 187, true, false, -1, WorldGen.genRand.Next(47, 50));
                                }
                                else
                                {
                                    int Y = WorldGen.genRand.Next(2);
                                    int X = 72;
                                    if (Y == 1)
                                        X = WorldGen.genRand.Next(59, 62);
                                    WorldGen.PlaceSmallPile(i1, j1, X, Y, (ushort)185);
                                }
                            }
                        }
                    }
                }
            }
            if (flag1)
            {
                bool flag3 = false;
                for (int j1 = j; j1 < j + 20 && (double)j1 < Main.worldSurface - 2.0; ++j1)
                {
                    for (int i1 = minl; i1 <= minr; ++i1)
                    {
                        if (Main.tile[i1, j1].wall == (ushort)0 && !WorldGen.SolidTile(i1, j1, false))
                            flag3 = true;
                    }
                }
                if (!flag3)
                    WorldGen.GrowLivingTree_MakePassage(j, num3, ref minl, ref minr, patch);
            }
            Main.tileSolid[48] = true;
            
        }
    }
    
}
