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
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;

namespace WorldRefill
{
    public class InfChestsDatabase : IDisposable
    {

        private IDbConnection ChestDB;
        private bool _disposed = false;
        private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);
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

                        break;
                    case "sqlite":
                        string sql = Path.Combine(TShock.SavePath, "InfChests3.sqlite");
                        ChestDB = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));
                        

                        break;
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(Path.Combine(Config.savepath, "WRLog.txt"), $"[{DateTime.UtcNow}] / [WR ERROR] FAILED TO CONNECT TO DATABASE, EXCEPTION : {e.Message}\t IS THE MYSQL DATABASE ON?\n");
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


                        string query = $"INSERT INTO InfChests3 (UserID, X, Y, Items, Public, Users, Groups, Refill, WorldID) VALUES ({-1}, {Main.chest[i].x}, {Main.chest[i].y}, '{items}', {0},'{string.Join(",", new List<int>())}','{string.Join(",", new List<string>())}',{-1},{Main.worldID});";

                        ChestDB.Query(query);
                        Main.chest[i] = null;
                    }
                }

            });

        }
       
        public Task<List<Point>> PruneChests()
        {
            return Task.Run(() =>
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("~");
                for (int i = 0; i < Chest.maxItems; i++)
                {
                    sb.Append("0,0,0~");
                }
                string emptychest = sb.ToString();

                List<Point> points = new List<Point>();

                string query = $"SELECT * FROM InfChests3 WHERE Items = '{emptychest}' AND WorldID = {Main.worldID} AND Refill = -1;";
                using (var reader = ChestDB.QueryReader(query))
                {
                    while (reader.Read())
                    {
                        Point point = new Point(reader.Get<int>("X"), reader.Get<int>("Y"));
                        points.Add(point);
                    }
                }

                query = $"DELETE FROM InfChests3 WHERE Items = '{emptychest}' AND WorldID = {Main.worldID} AND Refill = -1;";
                
                ChestDB.Query(query);
                return Task.FromResult(points);
            });

        }


        public void Dispose()
        {
            ChestDB.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                _safeHandle?.Dispose();
            }

            _disposed = true;
        }
    }
}
