using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Biomes.CaveHouse;
using Terraria.WorldBuilding;
using Terraria;
using Terraria.ID;
using Newtonsoft.Json;

namespace WorldRefill.MicroBiomeExtensions
{
    class CaveHouseBiomeExtensions : CaveHouseBiome
    {
        
        public new float IceChestChance { get; set; } = 1.0f;

        
        public new float JungleChestChance { get; set; } = 1.0f;

        
        public new float GoldChestChance { get; set; } = 1.0f;

        
        public new float GraniteChestChance { get; set; } = 1.0f;

        
        public new float MarbleChestChance { get; set; } = 1.0f;

        
        public new float MushroomChestChance { get; set; } = 1.0f;

       
        public new float DesertChestChance { get; set; } = 1.0f;

        public override bool Place(Point origin, StructureMap structures)
        {

            HouseBuilder builder = HouseUtils.CreateBuilder(origin, structures);

            ApplyConfigurationToBuilder(builder);

            builder.Place(_builderContext, structures);
            return true;
        }
        private new void ApplyConfigurationToBuilder(HouseBuilder builder)
        {
            switch (builder.Type)
            {
                case HouseType.Wood:
                    builder.ChestChance = GoldChestChance;
                    break;
                case HouseType.Ice:
                    builder.ChestChance = IceChestChance;
                    break;
                case HouseType.Desert:
                    builder.ChestChance = DesertChestChance;
                    break;
                case HouseType.Jungle:
                    builder.ChestChance = JungleChestChance;
                    break;
                case HouseType.Mushroom:
                    builder.ChestChance = MushroomChestChance;
                    break;
                case HouseType.Granite:
                    builder.ChestChance = GraniteChestChance;
                    break;
                case HouseType.Marble:
                    builder.ChestChance = MarbleChestChance;
                    break;
            }
        }


    }
    
}

