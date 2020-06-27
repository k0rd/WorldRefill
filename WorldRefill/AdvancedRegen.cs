using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;


namespace WorldRefill
{
    class AdvancedRegen
    {
        
        public static string ProcessSeed(string processedSeed)
        {
            
            WorldGen.notTheBees = processedSeed.ToLower() == "not the bees" || processedSeed.ToLower() == "not the bees!";
            WorldGen.getGoodWorldGen = processedSeed.ToLower() == "for the worthy";
            return processedSeed;
            




        }
    }
}


