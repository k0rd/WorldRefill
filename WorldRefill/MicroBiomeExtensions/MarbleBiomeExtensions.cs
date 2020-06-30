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
    class MarbleBiomeExtensions : MarbleBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
           
            if (this._slabs == null)
                this._slabs = new MarbleBiome.Slab[56, 26];
            int num1 = GenBase._random.Next(80, 150) / 3;
            int num2 = GenBase._random.Next(40, 60) / 3;
            int num3 = (num2 * 3 - GenBase._random.Next(20, 30)) / 3;
            origin.X -= num1 * 3 / 2;
            origin.Y -= num2 * 3 / 2;
            for (int index1 = -1; index1 < num1 + 1; ++index1)
            {
                double num4 = (double)(index1 - num1 / 2) / (double)num1 + 0.5;
                int num5 = (int)((0.5 - Math.Abs(num4 - 0.5)) * 5.0) - 2;
                for (int index2 = -1; index2 < num2 + 1; ++index2)
                {
                    bool hasWall = true;
                    bool flag1 = false;
                    bool flag2 = MarbleBiome.IsGroupSolid(index1 * 3 + origin.X, index2 * 3 + origin.Y, 3);
                    int num6 = Math.Abs(index2 - num2 / 2) - num3 / 4 + num5;
                    if (num6 > 3)
                    {
                        flag1 = flag2;
                        hasWall = false;
                    }
                    else if (num6 > 0)
                    {
                        flag1 = index2 - num2 / 2 > 0 | flag2;
                        hasWall = index2 - num2 / 2 < 0 || num6 <= 2;
                    }
                    else if (num6 == 0)
                        flag1 = GenBase._random.Next(2) == 0 && index2 - num2 / 2 > 0 | flag2;
                    if (Math.Abs(num4 - 0.5) > 0.349999994039536 + (double)GenBase._random.NextFloat() * 0.100000001490116 && !flag2)
                    {
                        hasWall = false;
                        flag1 = false;
                    }
                    this._slabs[index1 + 1, index2 + 1] = MarbleBiome.Slab.Create(flag1 ? new MarbleBiome.SlabState(MarbleBiome.SlabStates.Solid) : new MarbleBiome.SlabState(MarbleBiome.SlabStates.Empty), hasWall);
                }
            }
            for (int index1 = 0; index1 < num1; ++index1)
            {
                for (int index2 = 0; index2 < num2; ++index2)
                    this.SmoothSlope(index1 + 1, index2 + 1);
            }
            int num7 = num1 / 2;
            int val1 = num2 / 2;
            int num8 = (val1 + 1) * (val1 + 1);
            float num9 = (float)((double)GenBase._random.NextFloat() * 2.0 - 1.0);
            float num10 = (float)((double)GenBase._random.NextFloat() * 2.0 - 1.0);
            float num11 = (float)((double)GenBase._random.NextFloat() * 2.0 - 1.0);
            float num12 = 0.0f;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                float num4 = (float)val1 / (float)num7 * (float)(index1 - num7);
                int num5 = Math.Min(val1, (int)Math.Sqrt((double)Math.Max(0.0f, (float)num8 - num4 * num4)));
                if (index1 < num1 / 2)
                    num12 += MathHelper.Lerp(num9, num10, (float)index1 / (float)(num1 / 2));
                else
                    num12 += MathHelper.Lerp(num10, num11, (float)((double)index1 / (double)(num1 / 2) - 1.0));
                for (int index2 = val1 - num5; index2 <= val1 + num5; ++index2)
                    this.PlaceSlab(this._slabs[index1 + 1, index2 + 1], index1 * 3 + origin.X, index2 * 3 + origin.Y + (int)num12, 3);
            }
            structures.AddStructure(new Rectangle(origin.X, origin.Y, num1 * 3, num2 * 3), 8);
            return true;
        }
    }
}
