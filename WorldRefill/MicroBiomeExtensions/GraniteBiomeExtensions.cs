using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Biomes;
using Terraria.WorldBuilding;

namespace WorldRefill.MicroBiomeExtensions
{
    class GraniteBiomeExtensions : GraniteBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {

            origin.X -= this._sourceMagmaMap.GetLength(0) / 2;
            origin.Y -= this._sourceMagmaMap.GetLength(1) / 2;
            this.BuildMagmaMap(origin);
            Microsoft.Xna.Framework.Rectangle effectedMapArea;
            this.SimulatePressure(out effectedMapArea);
            this.PlaceGranite(origin, effectedMapArea);
            this.CleanupTiles(origin, effectedMapArea);
            this.PlaceDecorations(origin, effectedMapArea);
            structures.AddStructure(effectedMapArea, 8);
            return true;
        }
    }
    
}
