using TShockAPI;
using System.Data;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Terraria;
using TShockAPI.DB;
using System.Text;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using System.Reflection.Emit;

namespace WorldRefill
{
    public class InfChestsDatabase
    {

        private IDbConnection ChestDB;
        public InfChestsDatabase()
        {
            try
            {
                switch (TShock.Config.StorageType.ToLowerInvariant())
                {
                    case "mysql":
                        string[] host = TShock.Config.MySqlHost.Split(':');
                        ChestDB = new MySqlConnection()
                        {
                            ConnectionString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
                                host[0],
                                host.Length == 1 ? "3306" : host[1],
                                TShock.Config.MySqlDbName,
                                TShock.Config.MySqlUsername,
                                TShock.Config.MySqlPassword)
                        };
                        Console.WriteLine($"Connection Established with {ChestDB}");
                        break;
                    case "sqlite":
                        string sql = Path.Combine(Config.savepath, "InfChests3.sqlite");
                        ChestDB = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));
                        Console.WriteLine("Connetion Established");
                        break;
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(Path.Combine(Config.savepath, "WRLog.txt"), $"[{DateTime.Now}] / [WR ERROR] FAILED TO CONNECT TO DATABASE, EXCEPTION : {e.Message}\t IS THE MYSQL DATABASE ON?\n");
            }

        }
        public Task DeleteChests(int worldID)
        {
            return Task.Run(() =>
            {
                string query = $"DELETE FROM infchests3 Where WorldID = {worldID};";
                ChestDB.Query(query);
            });
        }
        public Task AddChests()
        {
            return Task.Run(() =>
            {
            
            for (int i = 0; i < Main.chest.Length; i++)
                {
                    if (Main.chest[i] != null)
                    {
                        StringBuilder iteminfo = new StringBuilder();
                        iteminfo.Append("~");
                        foreach (Item item in Main.chest[i].item)
                        {
                           string temp = $"{item.netID},{item.stack},{item.prefix}~";
                            iteminfo.Append(temp);

                        }
                        string items = iteminfo.ToString();
                        Console.WriteLine(items);

                        string query = $"INSERT INTO InfChests3 (UserID, X, Y, Items, Public, Users, Groups, Refill, WorldID) VALUES ({-1}, {Main.chest[i].x}, {Main.chest[i].y}, '{items}', {0},'{string.Join(",", new List<int>())}','{string.Join(",", new List<string>())}',{-1},{Main.worldID});";
                        Console.WriteLine(query);
                        ChestDB.Query(query);
                        Main.chest[i] = null;
                    }
                }

            });

        }
        public Task<List<Chest>> GetChests()
        {
            return Task.Run(() =>
            {
                string query = $"SELECT * FROM InfChests3 WHERE WorldID = {Main.worldID};";
                using (var reader = ChestDB.QueryReader(query))

                {
                    List<Chest> chests = new List<Chest>();
                    while (reader.Read())
                    {
                        Chest chest = new Chest()
                        {
                            x = reader.Get<int>("X"),
                            y = reader.Get<int>("Y")

                        };
                        chests.Add(chest);

                    }
                    return Task.FromResult(chests);
                }

            });
        }
       



    }
}
