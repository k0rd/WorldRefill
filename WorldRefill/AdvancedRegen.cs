using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Terraria;


namespace WorldRefill
{
    class TempleBuilder
    {
        public static void makeTemple(int x, int y)
        {
            Microsoft.Xna.Framework.Rectangle[] rectangleArray = new Microsoft.Xna.Framework.Rectangle[100];
            float num1 = (float)(Main.maxTilesX / 4200);
            int maxValue = WorldGen.genRand.Next((int)((double)num1 * 10.0), (int)((double)num1 * 16.0));
            if (WorldGen.drunkWorldGen)
                maxValue *= 3;
            if (WorldGen.getGoodWorldGen)
                maxValue *= 3;
            int num2 = 1;
            if (WorldGen.genRand.Next(2) == 0)
                num2 = -1;
            int num3 = num2;
            int num4 = x;
            int num5 = y;
            int num6 = x;
            int num7 = y;
            int num8 = WorldGen.genRand.Next(1, 3);
            int num9 = 0;
            for (int index1 = 0; index1 < maxValue; ++index1)
            {
                ++num9;
                int num10 = num2;
                int num11 = num6;
                int num12 = num7;
                bool flag = true;
                int width1 = 0;
                int height1 = 0;
                int num13 = -10;
                Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(num11 - width1 / 2, num12 - height1 / 2, width1, height1);
                while (flag)
                {
                    int num14 = num6;
                    int num15 = num7;
                    int width2 = WorldGen.genRand.Next(25, 50);
                    int height2 = WorldGen.genRand.Next(20, 35);
                    if (height2 > width2)
                        height2 = width2;
                    if (index1 == maxValue - 1)
                    {
                        int num16 = WorldGen.genRand.Next(55, 65);
                        int num17 = WorldGen.genRand.Next(45, 50);
                        if (num17 > num16)
                            num17 = num16;
                        width2 = (int)((double)num16 * 1.6);
                        height2 = (int)((double)num17 * 1.35);
                        num15 += WorldGen.genRand.Next(5, 10);
                    }
                    if (num9 > num8)
                    {
                        num12 = num15 + (WorldGen.genRand.Next(height2 + 1, height2 + 3) + num13);
                        num11 = num14 + WorldGen.genRand.Next(-5, 6);
                        num10 = num2 * -1;
                    }
                    else
                    {
                        num11 = num14 + (WorldGen.genRand.Next(width2 + 1, width2 + 3) + num13) * num10;
                        num12 = num15 + WorldGen.genRand.Next(-5, 6);
                    }
                    flag = false;
                    rectangle = new Microsoft.Xna.Framework.Rectangle(num11 - width2 / 2, num12 - height2 / 2, width2, height2);
                    for (int index2 = 0; index2 < index1; ++index2)
                    {
                        if (rectangle.Intersects(rectangleArray[index2]))
                            flag = true;
                        if (WorldGen.genRand.Next(100) == 0)
                            ++num13;
                    }
                }
                if (num9 > num8)
                {
                    ++num8;
                    num9 = 1;
                }
                rectangleArray[index1] = rectangle;
                num2 = num10;
                num6 = num11;
                num7 = num12;
            }
            for (int index1 = 0; index1 < maxValue; ++index1)
            {
                for (int index2 = 0; index2 < 2; ++index2)
                {
                    for (int index3 = 0; index3 < maxValue; ++index3)
                    {
                        for (int index4 = 0; index4 < 2; ++index4)
                        {
                            int x1 = rectangleArray[index1].X;
                            if (index2 == 1)
                                x1 += rectangleArray[index1].Width - 1;
                            int y1 = rectangleArray[index1].Y;
                            int num10 = y1 + rectangleArray[index1].Height;
                            int x2 = rectangleArray[index3].X;
                            if (index4 == 1)
                                x2 += rectangleArray[index3].Width - 1;
                            int y2 = rectangleArray[index3].Y;
                            int num11 = y2 + rectangleArray[index3].Height;
                            while (x1 != x2 || y1 != y2 || num10 != num11)
                            {
                                if (x1 < x2)
                                    ++x1;
                                if (x1 > x2)
                                    --x1;
                                if (y1 < y2)
                                    ++y1;
                                if (y1 > y2)
                                    --y1;
                                if (num10 < num11)
                                    ++num10;
                                if (num10 > num11)
                                    --num10;
                                int index5 = x1;
                                for (int index6 = y1; index6 < num10; ++index6)
                                {
                                    Main.tile[index5, index6].active(true);
                                    Main.tile[index5, index6].type = (ushort)226;
                                    Main.tile[index5, index6].liquid = (byte)0;
                                    Main.tile[index5, index6].slope((byte)0);
                                    Main.tile[index5, index6].halfBrick(false);
                                }
                            }
                        }
                    }
                }
            }
            for (int index1 = 0; index1 < maxValue; ++index1)
            {
                if (true)
                {
                    for (int x1 = rectangleArray[index1].X; x1 < rectangleArray[index1].X + rectangleArray[index1].Width; ++x1)
                    {
                        for (int y1 = rectangleArray[index1].Y; y1 < rectangleArray[index1].Y + rectangleArray[index1].Height; ++y1)
                        {
                            Main.tile[x1, y1].active(true);
                            Main.tile[x1, y1].type = (ushort)226;
                            Main.tile[x1, y1].liquid = (byte)0;
                            Main.tile[x1, y1].slope((byte)0);
                            Main.tile[x1, y1].halfBrick(false);
                        }
                    }
                    int x2 = rectangleArray[index1].X;
                    int num10 = x2 + rectangleArray[index1].Width;
                    int y2 = rectangleArray[index1].Y;
                    int num11 = y2 + rectangleArray[index1].Height;
                    int num12 = x2 + WorldGen.genRand.Next(3, 8);
                    int num13 = num10 - WorldGen.genRand.Next(3, 8);
                    int num14 = y2 + WorldGen.genRand.Next(3, 8);
                    int num15 = num11 - WorldGen.genRand.Next(3, 8);
                    int num16 = num12;
                    int num17 = num13;
                    int num18 = num14;
                    int num19 = num15;
                    int num20 = (num12 + num13) / 2;
                    int num21 = (num14 + num15) / 2;
                    for (int index2 = num12; index2 < num13; ++index2)
                    {
                        for (int index3 = num14; index3 < num15; ++index3)
                        {
                            if (WorldGen.genRand.Next(20) == 0)
                                num18 += WorldGen.genRand.Next(-1, 2);
                            if (WorldGen.genRand.Next(20) == 0)
                                num19 += WorldGen.genRand.Next(-1, 2);
                            if (WorldGen.genRand.Next(20) == 0)
                                num16 += WorldGen.genRand.Next(-1, 2);
                            if (WorldGen.genRand.Next(20) == 0)
                                num17 += WorldGen.genRand.Next(-1, 2);
                            if (num16 < num12)
                                num16 = num12;
                            if (num17 > num13)
                                num17 = num13;
                            if (num18 < num14)
                                num18 = num14;
                            if (num19 > num15)
                                num19 = num15;
                            if (num16 > num20)
                                num16 = num20;
                            if (num17 < num20)
                                num17 = num20;
                            if (num18 > num21)
                                num18 = num21;
                            if (num19 < num21)
                                num19 = num21;
                            if (index2 >= num16 && index2 < num17 & index3 >= num18 && index3 <= num19)
                            {
                                Main.tile[index2, index3].active(false);
                                Main.tile[index2, index3].wall = (ushort)87;
                            }
                        }
                    }
                    for (int index2 = num15; index2 > num14; --index2)
                    {
                        for (int index3 = num13; index3 > num12; --index3)
                        {
                            if (WorldGen.genRand.Next(20) == 0)
                                num18 += WorldGen.genRand.Next(-1, 2);
                            if (WorldGen.genRand.Next(20) == 0)
                                num19 += WorldGen.genRand.Next(-1, 2);
                            if (WorldGen.genRand.Next(20) == 0)
                                num16 += WorldGen.genRand.Next(-1, 2);
                            if (WorldGen.genRand.Next(20) == 0)
                                num17 += WorldGen.genRand.Next(-1, 2);
                            if (num16 < num12)
                                num16 = num12;
                            if (num17 > num13)
                                num17 = num13;
                            if (num18 < num14)
                                num18 = num14;
                            if (num19 > num15)
                                num19 = num15;
                            if (num16 > num20)
                                num16 = num20;
                            if (num17 < num20)
                                num17 = num20;
                            if (num18 > num21)
                                num18 = num21;
                            if (num19 < num21)
                                num19 = num21;
                            if (index3 >= num16 && index3 < num17 & index2 >= num18 && index2 <= num19)
                            {
                                Main.tile[index3, index2].active(false);
                                Main.tile[index3, index2].wall = (ushort)87;
                                
                            }
                        }
                    }
                }
            }
            Vector2 templePath = new Vector2((float)num4, (float)num5);
            for (int index1 = 0; index1 < maxValue; ++index1)
            {
                Microsoft.Xna.Framework.Rectangle rectangle = rectangleArray[index1];
                rectangle.X += 8;
                rectangle.Y += 8;
                rectangle.Width -= 16;
                rectangle.Height -= 16;
                bool flag1 = true;
                while (flag1)
                {
                    int destX = WorldGen.genRand.Next(rectangle.X, rectangle.X + rectangle.Width);
                    int destY = WorldGen.genRand.Next(rectangle.Y, rectangle.Y + rectangle.Height);
                    if (index1 == maxValue - 1)
                    {
                        destX = rectangle.X + rectangle.Width / 2 + WorldGen.genRand.Next(-10, 10);
                        destY = rectangle.Y + rectangle.Height / 2 + WorldGen.genRand.Next(-10, 10);
                    }
                    templePath = WorldGen.templePather(templePath, destX, destY);
                    if ((double)templePath.X == (double)destX && (double)templePath.Y == (double)destY)
                        flag1 = false;
                }
                if (index1 < maxValue - 1)
                {
                    if (WorldGen.genRand.Next(3) != 0)
                    {
                        int index2 = index1 + 1;
                        if (rectangleArray[index2].Y >= rectangleArray[index1].Y + rectangleArray[index1].Height)
                        {
                            rectangle.X = rectangleArray[index2].X;
                            if (rectangleArray[index2].X < rectangleArray[index1].X)
                                rectangle.X += (int)((double)rectangleArray[index2].Width * 0.2);
                            else
                                rectangle.X += (int)((double)rectangleArray[index2].Width * 0.8);
                            rectangle.Y = rectangleArray[index2].Y;
                        }
                        else
                        {
                            rectangle.X = (rectangleArray[index1].X + rectangleArray[index1].Width / 2 + rectangleArray[index2].X + rectangleArray[index2].Width / 2) / 2;
                            rectangle.Y = (int)((double)rectangleArray[index2].Y + (double)rectangleArray[index2].Height * 0.8);
                        }
                        int x1 = rectangle.X;
                        int y1 = rectangle.Y;
                        bool flag2 = true;
                        while (flag2)
                        {
                            int destX = WorldGen.genRand.Next(x1 - 6, x1 + 7);
                            int destY = WorldGen.genRand.Next(y1 - 6, y1 + 7);
                            templePath = WorldGen.templePather(templePath, destX, destY);
                            if ((double)templePath.X == (double)destX && (double)templePath.Y == (double)destY)
                                flag2 = false;
                        }
                    }
                    else
                    {
                        int index2 = index1 + 1;
                        int num10 = (rectangleArray[index1].X + rectangleArray[index1].Width / 2 + rectangleArray[index2].X + rectangleArray[index2].Width / 2) / 2;
                        int num11 = (rectangleArray[index1].Y + rectangleArray[index1].Height / 2 + rectangleArray[index2].Y + rectangleArray[index2].Height / 2) / 2;
                        bool flag2 = true;
                        while (flag2)
                        {
                            int destX = WorldGen.genRand.Next(num10 - 6, num10 + 7);
                            int destY = WorldGen.genRand.Next(num11 - 6, num11 + 7);
                            templePath = WorldGen.templePather(templePath, destX, destY);
                            if ((double)templePath.X == (double)destX && (double)templePath.Y == (double)destY)
                                flag2 = false;
                        }
                    }
                }
            }
            int num22 = Main.maxTilesX - 20;
            int num23 = 20;
            int num24 = Main.maxTilesY - 20;
            int num25 = 20;
            for (int index = 0; index < maxValue; ++index)
            {
                if (rectangleArray[index].X < num22)
                    num22 = rectangleArray[index].X;
                if (rectangleArray[index].X + rectangleArray[index].Width > num23)
                    num23 = rectangleArray[index].X + rectangleArray[index].Width;
                if (rectangleArray[index].Y < num24)
                    num24 = rectangleArray[index].Y;
                if (rectangleArray[index].Y + rectangleArray[index].Height > num25)
                    num25 = rectangleArray[index].Y + rectangleArray[index].Height;
            }
            int num26 = num22 - 10;
            int num27 = num23 + 10;
            int num28 = num24 - 10;
            int num29 = num25 + 10;
            for (int x1 = num26; x1 < num27; ++x1)
            {
                for (int y1 = num28; y1 < num29; ++y1)
                    WorldGen.outerTempled(x1, y1);
            }
            for (int x1 = num27; x1 >= num26; --x1)
            {
                for (int y1 = num28; y1 < num29 / 2; ++y1)
                    WorldGen.outerTempled(x1, y1);
            }
            for (int y1 = num28; y1 < num29; ++y1)
            {
                for (int x1 = num26; x1 < num27; ++x1)
                    WorldGen.outerTempled(x1, y1);
            }
            for (int y1 = num29; y1 >= num28; --y1)
            {
                for (int x1 = num26; x1 < num27; ++x1)
                    WorldGen.outerTempled(x1, y1);
            }
            int num30 = -num3;
            Vector2 vector2 = new Vector2((float)num4, (float)num5);
            int num31 = WorldGen.genRand.Next(2, 5);
            bool flag3 = true;
            int num32 = 0;
            int num33 = WorldGen.genRand.Next(9, 14);
            while (flag3)
            {
                ++num32;
                if (num32 >= num33)
                {
                    num32 = 0;
                    --vector2.Y;
                }
                vector2.X += (float)num30;
                int x1 = (int)vector2.X;
                flag3 = false;
                for (int index = (int)vector2.Y - num31; (double)index < (double)vector2.Y + (double)num31; ++index)
                {
                    if (Main.tile[x1, index].wall == (ushort)87 || Main.tile[x1, index].active() && Main.tile[x1, index].type == (ushort)226)
                        flag3 = true;
                    if (Main.tile[x1, index].active() && Main.tile[x1, index].type == (ushort)226)
                    {
                        Main.tile[x1, index].active(false);
                        Main.tile[x1, index].wall = (ushort)87;
                    }
                }
            }
            int i1 = num4;
            int index7 = num5;
            while (!Main.tile[i1, index7].active())
                ++index7;
            int j1 = index7 - 4;
            int index8 = j1;
            while (Main.tile[i1, index8].active() && Main.tile[i1, index8].type == (ushort)226 || Main.tile[i1, index8].wall == (ushort)87)
                --index8;
            int num34 = index8 + 2;
            for (int index1 = i1 - 1; index1 <= i1 + 1; ++index1)
            {
                for (int index2 = num34; index2 <= j1; ++index2)
                {
                    Main.tile[index1, index2].active(true);
                    Main.tile[index1, index2].type = (ushort)226;
                    Main.tile[index1, index2].liquid = (byte)0;
                    Main.tile[index1, index2].slope((byte)0);
                    Main.tile[index1, index2].halfBrick(false);
                }
            }
            for (int index1 = i1 - 4; index1 <= i1 + 4; ++index1)
            {
                for (int index2 = j1 - 1; index2 < j1 + 3; ++index2)
                {
                    Main.tile[index1, index2].active(false);
                    Main.tile[index1, index2].wall = (ushort)87;
                }
            }
            for (int index1 = i1 - 1; index1 <= i1 + 1; ++index1)
            {
                for (int index2 = j1 - 5; index2 <= j1 + 8; ++index2)
                {
                    Main.tile[index1, index2].active(true);
                    Main.tile[index1, index2].type = (ushort)226;
                    Main.tile[index1, index2].liquid = (byte)0;
                    Main.tile[index1, index2].slope((byte)0);
                    Main.tile[index1, index2].halfBrick(false);
                }
            }
            for (int index1 = i1 - 3; index1 <= i1 + 3; ++index1)
            {
                for (int index2 = j1 - 2; index2 < j1 + 3; ++index2)
                {
                    if (index2 >= j1 || index1 < num4 - 1 || index1 > num4 + 1)
                    {
                        Main.tile[index1, index2].active(false);
                        Main.tile[index1, index2].wall = (ushort)87;
                    }
                }
            }
            WorldGen.PlaceTile(i1, j1, 10, true, false, -1, 11);
            for (int x1 = num26; x1 < num27; ++x1)
            {
                for (int y1 = num28; y1 < num29; ++y1)
                    WorldGen.templeCleaner(x1, y1);
            }
            for (int y1 = num29; y1 >= num28; --y1)
            {
                for (int x1 = num27; x1 >= num26; --x1)
                    WorldGen.templeCleaner(x1, y1);
            }
            for (int index1 = num26; index1 < num27; ++index1)
            {
                for (int index2 = num28; index2 < num29; ++index2)
                {
                    bool flag1 = true;
                    for (int index3 = index1 - 1; index3 <= index1 + 1; ++index3)
                    {
                        for (int index4 = index2 - 1; index4 <= index2 + 1; ++index4)
                        {
                            if ((!Main.tile[index3, index4].active() || Main.tile[index3, index4].type != (ushort)226) && Main.tile[index3, index4].wall != (ushort)87)
                            {
                                flag1 = false;
                                break;
                            }
                        }
                    }
                    if (flag1)
                        Main.tile[index1, index2].wall = (ushort)87;
                }
            }
            int num35 = 0;
            Microsoft.Xna.Framework.Rectangle rectangle1 = rectangleArray[maxValue - 1];
            int num36 = rectangle1.Width / 2;
            int num37 = rectangle1.Height / 2;
            do
            {
                ++num35;
                int i2 = rectangle1.X + num36 + 15 - WorldGen.genRand.Next(30);
                int j2 = rectangle1.Y + num37 + 15 - WorldGen.genRand.Next(30);
                WorldGen.PlaceTile(i2, j2, 237, false, false, -1, 0);
                if (Main.tile[i2, j2].type == (ushort)237)
                {
                    WorldGen.lAltarX = i2 - (int)Main.tile[i2, j2].frameX / 18;
                    WorldGen.lAltarY = j2 - (int)Main.tile[i2, j2].frameY / 18;
                    goto label_292;
                }
            }
            while (num35 < 1000);
            int num38 = rectangle1.X + num36;
            int num39 = rectangle1.Y + num37;
            int index9 = num38 + WorldGen.genRand.Next(-10, 11);
            int index10 = num39 + WorldGen.genRand.Next(-10, 11);
            while (!Main.tile[index9, index10].active())
                ++index10;
            Main.tile[index9 - 1, index10].active(true);
            Main.tile[index9 - 1, index10].slope((byte)0);
            Main.tile[index9 - 1, index10].halfBrick(false);
            Main.tile[index9 - 1, index10].type = (ushort)226;
            Main.tile[index9, index10].active(true);
            Main.tile[index9, index10].slope((byte)0);
            Main.tile[index9, index10].halfBrick(false);
            Main.tile[index9, index10].type = (ushort)226;
            Main.tile[index9 + 1, index10].active(true);
            Main.tile[index9 + 1, index10].slope((byte)0);
            Main.tile[index9 + 1, index10].halfBrick(false);
            Main.tile[index9 + 1, index10].type = (ushort)226;
            int num40 = index10 - 2;
            int num41 = index9 - 1;
            for (int index1 = -1; index1 <= 3; ++index1)
            {
                for (int index2 = -1; index2 <= 1; ++index2)
                {
                    x = num41 + index1;
                    y = num40 + index2;
                    Main.tile[x, y].active(false);
                }
            }
            WorldGen.lAltarX = num41;
            WorldGen.lAltarY = num40;
            for (int index1 = 0; index1 <= 2; ++index1)
            {
                for (int index2 = 0; index2 <= 1; ++index2)
                {
                    x = num41 + index1;
                    y = num40 + index2;
                    Main.tile[x, y].active(true);
                    Main.tile[x, y].type = (ushort)237;
                    Main.tile[x, y].frameX = (short)(index1 * 18);
                    Main.tile[x, y].frameY = (short)(index2 * 18);
                }
            }
            for (int index1 = 0; index1 <= 2; ++index1)
            {
                for (int index2 = 0; index2 <= 1; ++index2)
                {
                    x = num41 + index1;
                    y = num40 + index2;
                    WorldGen.SquareTileFrame(x, y, true);
                }
            }
        label_292:
            float num42 = (float)maxValue * 1.1f * (float)(1.0 + (double)WorldGen.genRand.Next(-25, 26) * 0.00999999977648258);
            if (WorldGen.drunkWorldGen)
                num42 *= 1.5f;
            int num43 = 0;
            while ((double)num42 > 0.0)
            {
                ++num43;
                int index1 = WorldGen.genRand.Next(maxValue);
                int index2 = WorldGen.genRand.Next(rectangleArray[index1].X, rectangleArray[index1].X + rectangleArray[index1].Width);
                int index3 = WorldGen.genRand.Next(rectangleArray[index1].Y, rectangleArray[index1].Y + rectangleArray[index1].Height);
                if (Main.tile[index2, index3].wall == (ushort)87 && !Main.tile[index2, index3].active())
                {
                    bool flag1 = false;
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        int num10 = 1;
                        if (WorldGen.genRand.Next(2) == 0)
                            num10 = -1;
                        while (!Main.tile[index2, index3].active())
                            index3 += num10;
                        int num11 = index3 - num10;
                        int num12 = WorldGen.genRand.Next(2);
                        int num13 = WorldGen.genRand.Next(3, 10);
                        bool flag2 = true;
                        for (int index4 = index2 - num13; index4 < index2 + num13; ++index4)
                        {
                            for (int index5 = num11 - num13; index5 < num11 + num13; ++index5)
                            {
                                if (Main.tile[index4, index5].active() && (Main.tile[index4, index5].type == (ushort)10 || Main.tile[index4, index5].type == (ushort)237))
                                {
                                    flag2 = false;
                                    break;
                                }
                            }
                        }
                        if (flag2)
                        {
                            for (int i2 = index2 - num13; i2 < index2 + num13; ++i2)
                            {
                                for (int j2 = num11 - num13; j2 < num11 + num13; ++j2)
                                {
                                    if (WorldGen.SolidTile(i2, j2, false) && Main.tile[i2, j2].type != (ushort)232 && !WorldGen.SolidTile(i2, j2 - num10, false))
                                    {
                                        Main.tile[i2, j2].type = (ushort)232;
                                        flag1 = true;
                                        if (num12 == 0)
                                        {
                                            Main.tile[i2, j2 - 1].type = (ushort)232;
                                            Main.tile[i2, j2 - 1].active(true);
                                            if (WorldGen.drunkWorldGen)
                                            {
                                                Main.tile[i2, j2 - 2].type = (ushort)232;
                                                Main.tile[i2, j2 - 2].active(true);
                                            }
                                        }
                                        else
                                        {
                                            Main.tile[i2, j2 + 1].type = (ushort)232;
                                            Main.tile[i2, j2 + 1].active(true);
                                            if (WorldGen.drunkWorldGen)
                                            {
                                                Main.tile[i2, j2 + 2].type = (ushort)232;
                                                Main.tile[i2, j2 + 2].active(true);
                                            }
                                        }
                                        ++num12;
                                        if (num12 > 1)
                                            num12 = 0;
                                    }
                                }
                            }
                        }
                        if (flag1)
                        {
                            num43 = 0;
                            --num42;
                        }
                    }
                    else
                    {
                        int num10 = 1;
                        if (WorldGen.genRand.Next(2) == 0)
                            num10 = -1;
                        while (!Main.tile[index2, index3].active())
                            index2 += num10;
                        int num11 = index2 - num10;
                        int num12 = WorldGen.genRand.Next(2);
                        int num13 = WorldGen.genRand.Next(3, 10);
                        bool flag2 = true;
                        for (int index4 = num11 - num13; index4 < num11 + num13; ++index4)
                        {
                            for (int index5 = index3 - num13; index5 < index3 + num13; ++index5)
                            {
                                if (Main.tile[index4, index5].active() && Main.tile[index4, index5].type == (ushort)10)
                                {
                                    flag2 = false;
                                    break;
                                }
                            }
                        }
                        if (flag2)
                        {
                            for (int i2 = num11 - num13; i2 < num11 + num13; ++i2)
                            {
                                for (int j2 = index3 - num13; j2 < index3 + num13; ++j2)
                                {
                                    if (WorldGen.SolidTile(i2, j2, false) && Main.tile[i2, j2].type != (ushort)232 && !WorldGen.SolidTile(i2 - num10, j2, false))
                                    {
                                        Main.tile[i2, j2].type = (ushort)232;
                                        flag1 = true;
                                        if (num12 == 0)
                                        {
                                            Main.tile[i2 - 1, j2].type = (ushort)232;
                                            Main.tile[i2 - 1, j2].active(true);
                                            if (WorldGen.drunkWorldGen)
                                            {
                                                Main.tile[i2 - 2, j2].type = (ushort)232;
                                                Main.tile[i2 - 2, j2].active(true);
                                            }
                                        }
                                        else
                                        {
                                            Main.tile[i2 + 1, j2].type = (ushort)232;
                                            Main.tile[i2 + 1, j2].active(true);
                                            if (WorldGen.drunkWorldGen)
                                            {
                                                Main.tile[i2 - 2, j2].type = (ushort)232;
                                                Main.tile[i2 - 2, j2].active(true);
                                            }
                                        }
                                        ++num12;
                                        if (num12 > 1)
                                            num12 = 0;
                                    }
                                }
                            }
                        }
                        if (flag1)
                        {
                            num43 = 0;
                            --num42;
                        }
                    }
                }
                if (num43 > 1000)
                {
                    num43 = 0;
                    --num42;
                }
            }
            int tLeft = num26;
            var tRight = num27;
            var tTop = num28;
            var tBottom = num29;
            var tRooms = maxValue;

            float num44 = (float)tRooms * 1.9f * (float)(1.0 + (double)WorldGen.genRand.Next(-15, 16) * 0.00999999977648258);
            int num45 = 0;
            while ((double)num44 > 0.0)
            {
                int x2 = WorldGen.genRand.Next(tLeft, tRight);
                int y2 = WorldGen.genRand.Next(tTop, tBottom);
                if (Main.tile[x2, y2].wall == (ushort)87 && !Main.tile[x2, y2].active())
                {
                    if (WorldGen.mayanTrap(x2, y2))
                    {
                        --num44;
                        num45 = 0;
                    }
                    else
                        ++num45;
                }
                else
                    ++num45;
                if (num45 > 100)
                {
                    num45 = 0;
                    --num44;
                }
            }
            Main.tileSolid[232] = false;
            float num46 = (float)tRooms * 0.35f * (float)(1.0 + (double)WorldGen.genRand.Next(-15, 16) * 0.00999999977648258);
            int contain = 1293;
            int num47 = 0;
            while ((double)num3 > 0.0)
            {
                int i = WorldGen.genRand.Next(tLeft, tRight);
                int j = WorldGen.genRand.Next(tTop, tBottom);
                if (Main.tile[i, j].wall == (ushort)87 && !Main.tile[i, j].active() && WorldGen.AddBuriedChest(i, j, contain, true, 16, false, (ushort)0))
                {
                    --num46;
                    num47 = 0;
                }
                ++num47;
                if (num47 > 10000)
                    break;
            }
            float num48 = (float)tRooms * 1.25f * (float)(1.0 + (double)WorldGen.genRand.Next(-25, 36) * 0.00999999977648258);
            int num49 = 0;
            while ((double)num5 > 0.0)
            {
                ++num49;
                int index1 = WorldGen.genRand.Next(tLeft, tRight);
                int index2 = WorldGen.genRand.Next(tTop, tBottom);
                if (Main.tile[index1, index2].wall == (ushort)87 && !Main.tile[index1, index2].active())
                {
                    int i = index1;
                    int index3 = index2;
                    while (!Main.tile[i, index3].active())
                    {
                        ++index3;
                        if (index3 > tBottom)
                            break;
                    }
                    int j = index3 - 1;
                    if (j <= tBottom)
                    {
                        WorldGen.PlaceTile(i, j, 105, true, false, -1, WorldGen.genRand.Next(43, 46));
                        if (Main.tile[i, j].type == (ushort)105)
                            --num48;
                    }
                }
            }
            float num50 = (float)tRooms * 1.35f * (float)(1.0 + (double)WorldGen.genRand.Next(-15, 26) * 0.00999999977648258);
            int num51 = 0;
            while ((double)num50 > 0.0)
            {
                ++num51;
                int index1 = WorldGen.genRand.Next(tLeft, tRight);
                int index2 = WorldGen.genRand.Next(tTop, tBottom);
                if (Main.tile[index1, index2].wall == (ushort)87 && !Main.tile[index1, index2].active())
                {
                    int i = index1;
                    int index3 = index2;
                    while (!Main.tile[i, index3].active())
                    {
                        ++index3;
                        if (index3 > tBottom)
                            break;
                    }
                    int j = index3 - 1;
                    if (j <= tBottom)
                    {
                        switch (WorldGen.genRand.Next(3))
                        {
                            case 0:
                                WorldGen.PlaceTile(i, j, 18, true, false, -1, 10);
                                if (Main.tile[i, j].type == (ushort)18)
                                {
                                    --num50;
                                    break;
                                }
                                break;
                            case 1:
                                WorldGen.PlaceTile(i, j, 14, true, false, -1, 9);
                                if (Main.tile[i, j].type == (ushort)14)
                                {
                                    --num50;
                                    break;
                                }
                                break;
                            case 2:
                                WorldGen.PlaceTile(i, j, 15, true, false, -1, 12);
                                if (Main.tile[i, j].type == (ushort)15)
                                {
                                    --num50;
                                    break;
                                }
                                break;
                        }
                    }
                }
                if (num51 > 10000)
                    break;
            }
            Main.tileSolid[232] = true;
        }
    }
}


