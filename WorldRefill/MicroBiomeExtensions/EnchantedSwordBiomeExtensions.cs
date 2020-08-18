using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace WorldRefill.MicroBiomeExtensions
{
    class EnchantedSwordBiomeExtensions : EnchantedSwordBiome
    {

        private new float _chanceOfEntrance = 1;
        
        private new float _chanceOfRealSword = 1;

        public override bool Place(Point origin, StructureMap structures)
        {
            
            Point result;
            bool flag = WorldUtils.Find(origin, Searches.Chain((GenSearch)new Searches.Up(1000), new Conditions.IsSolid().AreaOr(1, 100).Not()), out result);
            result.Y += 100;
            ShapeData data1 = new ShapeData();
            ShapeData shapeData = new ShapeData();
            Point point1 = new Point(origin.X, origin.Y + 20);
            Point point2 = new Point(origin.X, origin.Y + 30);
            bool[] validTiles = new bool[TileID.Sets.GeneralPlacementTiles.Length];
            for (int index = 0; index < validTiles.Length; ++index)
                validTiles[index] = TileID.Sets.GeneralPlacementTiles[index];
            validTiles[21] = false;
            validTiles[467] = false;
            float xScale = (float)(0.800000011920929 + (double)GenBase._random.NextFloat() * 0.5);
           
            WorldUtils.Gen(point1, (GenShape)new Shapes.Slime(20, xScale, 1f), Actions.Chain((GenAction)new Modifiers.Blotches(2, 0.4), new Actions.ClearTile(true).Output(data1)));
            WorldUtils.Gen(point2, (GenShape)new Shapes.Mound(14, 14), Actions.Chain((GenAction)new Modifiers.Blotches(2, 1, 0.8), (GenAction)new Actions.SetTile((ushort)0, false, true), new Actions.SetFrames(true).Output(shapeData)));
            data1.Subtract(shapeData, point1, point2);
            WorldUtils.Gen(point1, (GenShape)new ModShapes.InnerOutline(data1, true), Actions.Chain((GenAction)new Actions.SetTile((ushort)2, false, true), (GenAction)new Actions.SetFrames(true)));
            WorldUtils.Gen(point1, (GenShape)new ModShapes.All(data1), Actions.Chain((GenAction)new Modifiers.RectangleMask(-40, 40, 0, 40), (GenAction)new Modifiers.IsEmpty(), (GenAction)new Actions.SetLiquid(0, byte.MaxValue)));
            WorldUtils.Gen(point1, (GenShape)new ModShapes.All(data1), Actions.Chain((GenAction)new Actions.PlaceWall((ushort)68, true), (GenAction)new Modifiers.OnlyTiles(new ushort[1]
            {
        (ushort) 2
            }), (GenAction)new Modifiers.Offset(0, 1), (GenAction)new ActionVines(3, 5, 382)));
            if ((double)GenBase._random.NextFloat() <= (double)this._chanceOfEntrance)
            {
                ShapeData data2 = new ShapeData();
                WorldUtils.Gen(new Point(origin.X, result.Y + 10), (GenShape)new Shapes.Rectangle(1, origin.Y - result.Y - 9), Actions.Chain((GenAction)new Modifiers.Blotches(2, 0.2), (GenAction)new Modifiers.SkipTiles(new ushort[2]
                {
          (ushort) 191,
          (ushort) 192
                }), new Actions.ClearTile(false).Output(data2), (GenAction)new Modifiers.Expand(1), (GenAction)new Modifiers.OnlyTiles(new ushort[1]
                {
          (ushort) 53
                }), new Actions.SetTile((ushort)397, false, true).Output(data2)));
                WorldUtils.Gen(new Point(origin.X, result.Y + 10), (GenShape)new ModShapes.All(data2), (GenAction)new Actions.SetFrames(true));
            }
            if ((double)GenBase._random.NextFloat() <= (double)this._chanceOfRealSword)
                WorldGen.PlaceTile(point2.X, point2.Y - 15, 187, true, false, -1, 17);
            else
                WorldGen.PlaceTile(point2.X, point2.Y - 15, 186, true, false, -1, 15);
            WorldUtils.Gen(point2, (GenShape)new ModShapes.All(shapeData), Actions.Chain((GenAction)new Modifiers.Offset(0, -1), (GenAction)new Modifiers.OnlyTiles(new ushort[1]
            {
        (ushort) 2
            }), (GenAction)new Modifiers.Offset(0, -1), (GenAction)new ActionGrass()));
            structures.AddProtectedStructure(new Microsoft.Xna.Framework.Rectangle(point1.X - (int)(20.0 * (double)xScale), point1.Y - 20, (int)(40.0 * (double)xScale), 40), 10);
            return true;
        }
    }
}
