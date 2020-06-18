using Newtonsoft.Json;
using System;
using System.IO;
using TShockAPI;


namespace WorldRefill
{

    public class Config
    {
        private static string savepath = TShock.SavePath;
        public static Config config;
        // Config Code stolen from InanZed's DieMob
        #region Create
        private static void CreateConfig()
        {
            string filepath = Path.Combine(savepath, "WorldRefillConfig.json");
            try
            {
                using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    using (var sr = new StreamWriter(stream))
                    {
                        config = new Config();
                        var configString = JsonConvert.SerializeObject(config, Formatting.Indented);
                        sr.Write(configString);
                    }
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(ex.Message);
                config = new Config();
            }
        }
        #endregion
        #region Read
        public static bool ReadConfig()
        {
            string filepath = Path.Combine(savepath, "WorldRefillConfig.json");
            try
            {
                if (File.Exists(filepath))
                {
                    using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            var configString = sr.ReadToEnd();
                            config = JsonConvert.DeserializeObject<Config>(configString);
                        }
                        stream.Close();
                    }
                    return true;
                }
                else
                {
                    TShock.Log.ConsoleError("World Refill config not found. Creating new one...");
                    CreateConfig();
                    return false;
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(ex.Message);
            }
            return false;
        }
        #endregion
        // Variables to be added to WorldRefillConfig.json | Moved default chest IDs here so that people can edit them
        #region DefaultChestIDs
        public int[] DefaultChestIDs = new[]
        {
                168,
                20,
                22,
                40,
                42,
                28,
                292,
                298,
                299,
                290,
                8,
                31,
                72,
                280,
                284,
                281,
                282,
                279,
                285,
                21,
                289,
                303,
                291,
                304,
                49,
                50,
                52,
                53,
                54,
                55,
                51,
                43,
                167,
                188,
                295,
                302,
                305,
                73,
                301,
                159,
                65,
                158,
                117,
                265,
                294,
                288,
                297,
                300,
                218,
                112,
                220,
                985,
                267,
                156
            };
        #endregion
        
        public const int GenerationMaxTries = 1000000;
        public bool UseInfiniteChests { get; } = false;
        public bool GenInsideProtectedRegions { get; } = false;
        // Setting this value higher may result in more lag when generating as this is the maximum amount of tries it will take to generate amenities.
    }
}
