﻿using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace WorldRefill
{
	[ApiVersion(1, 22)]
	public class WorldRefill : TerrariaPlugin
	{
		public WorldRefill(Main game)
			: base(game)
		{
		}
		private static string savepath = TShock.SavePath;
		private static Config config;
		private IDbConnection ChestDB;

		#region Initialize
		public override void Initialize()
		{
			#region Gen Commands
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoCrystals, "gencrystals"));		//Life Crystals
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoPots, "genpots"));				//Pots
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoOrbs, "genorbs"));				//Orbs
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoAltars, "genaltars"));			//Demon Altars
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoTraps, "gentraps"));			//Traps
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoStatues, "genstatues"));		//Statues
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoOres, "genores"));				//ores
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoWebs, "genwebs"));				//webs
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoMineHouse, "genhouse"));		//mine house
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoTrees, "gentrees"));			//trees
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoIsland, "genisland"));			//floating island
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoShrooms, "genpatch"));			//mushroom patch
			//Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoLake, "genlake"));			//lake
			//Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoMountain, "genmountain"));	//mountain
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoChests, "genchests"));		//chests
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoIslandHouse, "genihouse"));		//island house
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoHV, "hellevator"));				//hellevator
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoPyramid, "genpyramid"));		//pyramid
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoCloudIsland, "gencisland"));	//cloud island + house
			//Commands.ChatCommands.Add(new Command("tshock.world.causeevents", DoLivingTree, "genltree"));		// Added on v1.7.1 (Not working)
			#endregion
			Commands.ChatCommands.Add(new Command("tshock.world.causeevents", ConfigReload, "refillreload"));
			ReadConfig();
		}
		#endregion

		#region Dispose
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{

			}
			base.Dispose(disposing);
		}
		#endregion

		#region Plugin Info
		public override Version Version
		{
			get { return new Version(1, 7, 5); }
		}
		public override string Name
		{
			get { return "World Refill Plugin"; }
		}
		public override string Author
		{
			get { return "k0rd, IcyPhoenix and Enerdy"; }
		}
		public override string Description
		{
			get { return "Refill your world!"; }
		}
		#endregion

		#region Config
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
		private static bool ReadConfig()
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
					TShock.Log.ConsoleError("WorldRefill config not found. Creating new one...");
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
		#region Reload Command
		// Config Reload
		private void ConfigReload(CommandArgs args)
		{
			if (ReadConfig())
				args.Player.SendSuccessMessage("WorldEdit config reloaded.");
			else
				args.Player.SendErrorMessage("Error reading config. Check log for details.");
			return;
		}
		#endregion

		class Config
		{
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
			public bool UseInfiniteChests = false;
			public bool GenInsideProtectedRegions = false;
		}
		#endregion

		#region GenCrystals Command
		private void DoCrystals(CommandArgs args)
		{

			if (args.Parameters.Count == 1)
			{
				var mCry = Int32.Parse(args.Parameters[0]);
				var surface = Main.worldSurface;
				var trycount = 0;
				//maxtries = retry amounts if generation of object fails
				const int maxtries = 1000000;
				//realcount = actual amount of objects generated
				var realcount = 0;

				//Attempting to generate Objects
				while (trycount < maxtries)
				{
					if (WorldGen.AddLifeCrystal(WorldGen.genRand.Next(1, Main.maxTilesX), WorldGen.genRand.Next((int)(surface + 20.0), Main.maxTilesY)))
					{
						realcount++;
						//Determine if enough Objects have been generated
						if (realcount == mCry) break;
					}
					trycount++;
				}
				//Notify user on success
				args.Player.SendSuccessMessage("Generated and hid {0} Life Crystals.", realcount);

				InformPlayers();
			}
			else
			{
				//notify user of command failure
				args.Player.SendInfoMessage("Usage: /gencrystals (number of crystals to generate)");
			}

		}
		#endregion
		#region GenPots Command
		private void DoPots(CommandArgs args)
		{
			if (args.Parameters.Count == 1)
			{

				var mPot = Int32.Parse(args.Parameters[0]);
				var surface = Main.worldSurface;
				var trycount = 0;
				const int maxtries = 1000000;
				var realcount = 0;
				while (trycount < maxtries)
				{
					var tryX = WorldGen.genRand.Next(1, Main.maxTilesX);
					var tryY = WorldGen.genRand.Next((int)surface - 10, Main.maxTilesY);
					if (WorldGen.PlacePot(tryX, tryY, 28))
					{
						realcount++;
						if (realcount == mPot)
							break;
					}
					trycount++;

				}
				args.Player.SendSuccessMessage("Generated and hid {0} Pots.", realcount);
				InformPlayers();
			}
			else
			{
				args.Player.SendInfoMessage("Usage: /genpots (number of pots to generate)");
			}
		}
		#endregion
		#region GenOrbs Command
		private void DoOrbs(CommandArgs args)
		{
			if (args.Parameters.Count == 1)
			{

				var mOrb = Int32.Parse(args.Parameters[0]);
				var surface = Main.worldSurface;
				var trycount = 0;
				const int maxtries = 1000000;
				var realcount = 0;
				while (trycount < maxtries)
				{
					var tryX = WorldGen.genRand.Next(1, Main.maxTilesX);
					var tryY = WorldGen.genRand.Next((int)surface + 20, Main.maxTilesY);

					if ((!Main.tile[tryX, tryY].active()) && ((Main.tile[tryX, tryY].wall == (byte)3) || (Main.tile[tryX, tryY].wall == (byte)83)))
					{
						WorldGen.AddShadowOrb(tryX, tryY);
						if (Main.tile[tryX, tryY].type == 31)
						{
							realcount++;
							if (realcount == mOrb)
								break;
						}
					}
					trycount++;
				}
				InformPlayers();
				args.Player.SendSuccessMessage("Generated and hid {0} Orbs.", realcount);
			}
			else
			{
				args.Player.SendInfoMessage("Usage: /genorbs (number of orbs to generate)");
			}
		}
		#endregion
		#region GenAltars Command
		private void DoAltars(CommandArgs args)
		{
			if (args.Parameters.Count == 1)
			{

				var mAltar = Int32.Parse(args.Parameters[0]);
				var surface = Main.worldSurface;
				var trycount = 0;
				const int maxtries = 1000000;
				var realcount = 0;
				while (trycount < maxtries)
				{
					var tryX = WorldGen.genRand.Next(1, Main.maxTilesX);
					var tryY = WorldGen.genRand.Next((int)surface + 10, Main.maxTilesY);

					WorldGen.Place3x2(tryX, tryY, 26);
					if (Main.tile[tryX, tryY].type == 26)
					{
						realcount++;
						if (realcount == mAltar)
							break;
					}

					trycount++;
				}
				InformPlayers();
				args.Player.SendSuccessMessage("Generated and hid {0} Demon Altars.", realcount);
			}
			else
			{
				args.Player.SendInfoMessage("Usage: /genaltars (number of Demon Altars to generate)");
			}
		}
		#endregion
		#region GenTraps Command
		private void DoTraps(CommandArgs args)
		{
			if (args.Parameters.Count == 1)
			{
				args.Player.SendInfoMessage("Generating traps.. this may take a while..");
				var mTrap = Int32.Parse(args.Parameters[0]);
				var surface = Main.worldSurface;
				var trycount = 0;
				const int maxtries = 100000;
				var realcount = 0;
				while (trycount < maxtries)
				{
					var tryX = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
					var tryY = WorldGen.genRand.Next((int)surface, Main.maxTilesY - 300);


					if (Main.tile[tryX, tryY].wall == 0 && WorldGen.placeTrap(tryX, tryY, -1))
					{
						realcount++;
						if (realcount == mTrap)
							break;
					}

					trycount++;
				}
				InformPlayers();
				args.Player.SendSuccessMessage("Generated and hid {0} traps.", realcount);
			}
			else
			{
				args.Player.SendInfoMessage("Usage: /gentraps (number of Traps to generate)");
			}
		}
		#endregion
		#region GenStatues Command
		private void DoStatues(CommandArgs args)
		{
			if (args.Parameters.Count == 1)
			{
				args.Player.SendInfoMessage("Generating statues.. this may take a while..");
				var mStatue = Int32.Parse(args.Parameters[0]);
				var surface = Main.worldSurface;
				var trycount = 0;
				const int maxtries = 100000;
				var realcount = 0;
				while (trycount < maxtries)
				{
					var tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					var tryY = WorldGen.genRand.Next((int)surface + 20, Main.maxTilesY - 300);
					var tryType = WorldGen.genRand.Next((int)2, 47);

					while (!Main.tile[tryX, tryY].active())
					{
						tryY++;
					}
					tryY--;

					if (tryY < Main.maxTilesY - 300)
					{

						WorldGen.PlaceTile(tryX, tryY, 105, true, true, -1, tryType);

						if (Main.tile[tryX, tryY].type == 105)
						{
							realcount++;
							if (realcount == mStatue)
								break;
						}
					}
					trycount++;
				}
				args.Player.SendSuccessMessage("Generated and hid {0} Statues.", realcount);
				InformPlayers();
			}
			else if (args.Parameters.Count == 2)
			{
				#region types
				List<string> types = new List<string>
                                         {
                                             "Armor",
                                             "Angel",
                                             "Star",
                                             "Sword",
                                             "Slime",
                                             "Goblin",
                                             "Shield",
                                             "Bat",
                                             "Fish",
                                             "Bunny",
                                             "Skeleton",
                                             "Reaper",
                                             "Woman",
                                             "Imp",
                                             "Gargoyle",
                                             "Gloom",
                                             "Hornet",
                                             "Bomb",
                                             "Crab",
                                             "Hammer",
                                             "Potion",
                                             "Spear",
                                             "Cross",
                                             "Jellyfish",
                                             "Bow",
                                             "Boomerang",
                                             "Boot",
                                             "Chest",
                                             "Bird",
                                             "Axe",
                                             "Corrupt",
                                             "Tree",
                                             "Anvil",
                                             "Pickaxe",
                                             "Mushroom",
                                             "Eyeball",
                                             "Pillar",
                                             "Heart",
                                             "Pot",
                                             "Sunflower",
                                             "King",
                                             "Queen",
                                             "Piranha",
											 "Lihzahrd",
											 "Lihzhard Watcher",
											 "Lihzahrd Guardian",
                                             "Ukown"
                                         };
				#endregion

				string mReqs = args.Parameters[1].ToLowerInvariant();
				var mStatue = Int32.Parse(args.Parameters[0]);
				var surface = Main.worldSurface;
				var trycount = 0;
				const int maxtries = 100000;
				var realcount = 0;
				int stid = 0;
				string found = "unknown type!";
				foreach (string ment in types)
				{
					found = ment.ToLowerInvariant();
					if (found.StartsWith(mReqs))
					{
						break;
					}
					stid++;
				}
				if (stid < 44)
				{

					args.Player.SendInfoMessage("Generating {0} statues.. this may take a while..", found);
					while (trycount < maxtries)
					{
						var tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
						var tryY = WorldGen.genRand.Next((int)surface + 20, Main.maxTilesY - 300);

						while (!Main.tile[tryX, tryY].active())
						{
							tryY++;
						}
						tryY--;

						if (tryY < Main.maxTilesY - 300)
						{

							WorldGen.PlaceTile(tryX, tryY, 105, true, true, -1, stid);

							if (Main.tile[tryX, tryY].type == 105)
							{
								realcount++;
								if (realcount == mStatue)
									break;
							}
						}
						trycount++;
					}
					args.Player.SendSuccessMessage("Generated and hid {0} {1} ({2})Statues.", realcount, found, stid);
					InformPlayers();
				}
				else
				{
					args.Player.SendErrorMessage("Couldn't find a match for {0}.", mReqs);
				}
			}
			else
			{
				args.Player.SendInfoMessage("Usage: /genstatues (number of statues to generate) [(optional)name of statue]");
			}
		}
		#endregion
		#region GenOres Command
		private static void DoOres(CommandArgs args)
		{
			if (WorldGen.genRand == null)
				WorldGen.genRand = new Random();

			TSPlayer ply = args.Player;
			ushort oreType;
			float oreAmts = 100f;
			//Sigh why did I bother checking what this does...gets completely overwritten
			//Mod altarcount divide 3 - gives number between: 0 to 2
			//int num = WorldGen.altarCount % 3;
			//Altarcount divide 3 + 1 - gives number between: 1 to infinity
			//int num2 = WorldGen.altarCount / 3 + 1;
			//4200 = small world size
			//6400 = medium world size
			//8400 = large world size
			//returns value: - 0 , 1.523809523809524, 2
			//float num3 = (float)(Main.maxTilesX / 4200);
			//Gives number between: -1 to 1
			//int num4 = 1 - num;
			//num3 * 310f - returns value: 0, 472.3809523809524, 620
			//(float)(85 * num) - gives number between 0 to 170
			//Returns value: -170, 302.3809523809524, 450
			//num3 = num3 * 310f - (float)(85 * num);
			//Returns Value: -144.5, 257.0238095238095, 382.5
			//num3 *= 0.85f;
			//gives number between: -144.5 to 382.5
			//num3 /= (float)num2;

			if (args.Parameters.Count < 1)
			{
				ply.SendInfoMessage("Usage: /genores (type) (amount)");
				return;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "cobalt")
			{
				oreType = Terraria.ID.TileID.Cobalt;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "mythril")
			{
				oreType = Terraria.ID.TileID.Mythril;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "copper")
			{
				oreType = Terraria.ID.TileID.Copper;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "iron")
			{
				oreType = Terraria.ID.TileID.Iron;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "silver")
			{
				oreType = Terraria.ID.TileID.Silver;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "gold")
			{
				oreType = Terraria.ID.TileID.Gold;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "demonite")
			{
				oreType = Terraria.ID.TileID.Demonite;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "sapphire")
			{
				oreType = Terraria.ID.TileID.Sapphire;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "ruby")
			{
				oreType = Terraria.ID.TileID.Ruby;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "emerald")
			{
				oreType = Terraria.ID.TileID.Emerald;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "topaz")
			{
				oreType = Terraria.ID.TileID.Topaz;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "amethyst")
			{
				oreType = Terraria.ID.TileID.Amethyst;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "diamond")
			{
				oreType = Terraria.ID.TileID.Diamond;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "adamantite")
			{
				oreType = Terraria.ID.TileID.Adamantite;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "hellstone")
			{
				oreType = Terraria.ID.TileID.Hellstone;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "meteorite")
			{
				oreType = Terraria.ID.TileID.Meteorite;
			}

			// New Ores
			else if (args.Parameters[0].ToLowerInvariant() == "tin")
			{
				oreType = Terraria.ID.TileID.Tin;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "lead")
			{
				oreType = Terraria.ID.TileID.Lead;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "tungsten")
			{
				oreType = Terraria.ID.TileID.Tungsten;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "platinum")
			{
				oreType = Terraria.ID.TileID.Platinum;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "crimtane")
			{
				oreType = Terraria.ID.TileID.Crimtane;
			}

			// 1.2 Hardmode Ores
			else if (args.Parameters[0].ToLowerInvariant() == "palladium")
			{
				oreType = Terraria.ID.TileID.Palladium;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "orichalcum")
			{
				oreType = Terraria.ID.TileID.Orichalcum;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "titanium")
			{
				oreType = Terraria.ID.TileID.Titanium;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "chlorophyte")
			{
				oreType = Terraria.ID.TileID.Chlorophyte;
			}

			// Others
			else if (args.Parameters[0].ToLowerInvariant() == "dirt")
			{
				oreType = Terraria.ID.TileID.Dirt;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "stone")
			{
				oreType = Terraria.ID.TileID.Stone;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "sand")
			{
				oreType = Terraria.ID.TileID.Sand;
			}
			else if (args.Parameters[0].ToLowerInvariant() == "silt")
			{
				oreType = Terraria.ID.TileID.Silt;
			}
			else
			{
				ply.SendErrorMessage("Warning! Typo in Tile name or Tile does not exist");
				return;
			}

			//If user specifies how many ores to generate (make sure not over 10000)
			if (args.Parameters.Count > 1)
			{
				float.TryParse(args.Parameters[1], out oreAmts);
				oreAmts = Math.Min(oreAmts, 10000f);
			}
			//oreGened = track amount of ores generated already
			int oreGened = 0;
			int minFrequency;
			int maxFrequency;
			int minSpread;
			int maxSpread;
			while ((float)oreGened < oreAmts)
			{
				//Get random number from 100 tiles each side
				int i2 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
				double worldY = Main.worldSurface;
				//Rare Ores  - Adamantite (Titanium), Demonite, Diamond, Chlorophyte
				if ((oreType == Terraria.ID.TileID.Adamantite) ||
					(oreType == Terraria.ID.TileID.Demonite) ||
					(oreType == Terraria.ID.TileID.Crimtane) ||
					(oreType == Terraria.ID.TileID.Chlorophyte) ||
					(oreType == Terraria.ID.TileID.Titanium) ||
					((oreType >= Terraria.ID.TileID.Sapphire) && (oreType <= Terraria.ID.TileID.Diamond)))
				{
					//Some formula created by k0rd for getting somewhere between hell and roughly half way after rock
					worldY = (Main.rockLayer + Main.rockLayer + (double)Main.maxTilesY) / 3.0;
					minFrequency = 2;
					minSpread = 2;
					maxFrequency = 3;
					maxSpread = 3;
				}
				//Hellstone Only
				else if (oreType == Terraria.ID.TileID.Hellstone)
				{
					//roughly where hell is
					worldY = Main.maxTilesY - 200;
					minFrequency = 4;
					minSpread = 4;
					maxFrequency = 9;
					maxSpread = 9;
				}
				else
				{
					worldY = Main.rockLayer;
					minFrequency = 5;
					minSpread = 9;
					maxFrequency = 5;
					maxSpread = 9;
				}
				//Gets random number based on minimum spawn point to maximum depth of map
				int j2 = WorldGen.genRand.Next((int)worldY, Main.maxTilesY - 150);
				WorldGen.OreRunner(i2, j2, (double)WorldGen.genRand.Next(minSpread, maxSpread), WorldGen.genRand.Next(minFrequency, maxFrequency), oreType);
				oreGened++;
			}
			ply.SendSuccessMessage("Spawned {0} tiles of {1}", Math.Floor(oreAmts), args.Parameters[0].ToLowerInvariant());
			InformPlayers();
		}
		#endregion
		#region GenWebs Command
		private void DoWebs(CommandArgs args)
		{
			if (args.Parameters.Count == 1)
			{

				var mWeb = Int32.Parse(args.Parameters[0]);
				var surface = Main.worldSurface;
				var trycount = 0;
				const int maxtries = 1000000;
				var realcount = 0;
				while (trycount < maxtries)
				{
					var tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					var tryY = WorldGen.genRand.Next(150, Main.maxTilesY - 300);
					int direction = WorldGen.genRand.Next(2);
					if (direction == 0)
					{
						direction = -1;
					}
					else
					{
						direction = 1;
					}
					while (!Main.tile[tryX, tryY].active() && tryY > 149)
					{
						tryY--;
					}
					tryY++;
					while (!Main.tile[tryX, tryY].active() && tryX > 10 && tryX < Main.maxTilesX - 10)
					{
						tryX += direction;
					}
					tryX -= direction;

					if ((tryY < Main.maxTilesY - 300) && (tryX < Main.maxTilesX - 20) && (tryX > 20) && (tryY > 150))
					{

						WorldGen.TileRunner(tryX, tryY, (double)WorldGen.genRand.Next(4, 11), WorldGen.genRand.Next(2, 4), 51, true, (float)direction, -1f, false, false);
						realcount++;
						if (realcount == mWeb)
							break;
					}
					trycount++;

				}
				args.Player.SendSuccessMessage("Generated and hid {0} Webs.", realcount);
				InformPlayers();
			}
			else
			{
				args.Player.SendInfoMessage("Usage: /genwebs (number of webs to generate)");
			}
		}
		#endregion
		#region GenTrees Command
		private void DoTrees(CommandArgs args)
		{
			var counter = 0;
			while ((double)counter < (double)Main.maxTilesX * 0.003)
			{
				int tryX = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
				int tryY = WorldGen.genRand.Next(25, 50);
				for (var tick = tryX - tryY; tick < tryX + tryY; tick++)
				{
					var offset = 20;
					while ((double)offset < Main.worldSurface)
					{
						WorldGen.GrowEpicTree(tick, offset);
						offset++;
					}
				}
				counter++;
			}
			WorldGen.AddTrees();
			args.Player.SendSuccessMessage("Enjoy your trees.");
			InformPlayers();
		}
		#endregion
		#region GenShrooms Command
		private void DoShrooms(CommandArgs args)
		{
			int tryX = args.Player.TileX;
			int tryY = args.Player.TileY;
			const int offset = 25;
			WorldGen.ShroomPatch(tryX, tryY + 1);
			for (int z = args.Player.TileX - offset; z < args.Player.TileX + offset; z++)
			{
				for (int y = args.Player.TileY - offset; y < args.Player.TileY + offset; y++)
				{
					if (Main.tile[z, y].active())
					{
						WorldGen.SpreadGrass(z, y, 59, 70, false);

					}
				}
			}

			InformPlayers();
			args.Player.SendSuccessMessage("Mushroom Farm generated.");
		}
		#endregion

		#region GenPyramid Command
		private void DoPyramid(CommandArgs args)
		{
			int tryX = args.Player.TileX;
			int tryY = args.Player.TileY;
			bool resulta;
			resulta = WorldGen.Pyramid(tryX, tryY);

			if (resulta)
			{
				InformPlayers();
				args.Player.SendSuccessMessage("A pyramid was created.");
			}
			else
			{
				args.Player.SendErrorMessage("A pyramid cannot be created here.");
			}
		}
		#endregion
		#region Hellevator Command
		private void DoHV(CommandArgs args)
		{
			int meX = args.Player.TileX;
			int meY = args.Player.TileY;
			const int maxsize = 25;
			const int bump = 4;
			int cx;
			int ypos = 0;
			int start = 0;

			int bottom = Main.maxTilesY - 150;
			int width = 3;
			if (args.Parameters.Count == 1)
				width = Int32.Parse(args.Parameters[0]);
			if (width < 2) width = 2;
			if (width > maxsize) width = maxsize;
			start = meX - (width / 2);
			ypos = meY + bump;
			start--;
			width++;
			int tl = 121;
			int wl = 25;
			int a = WorldGen.genRand.Next(1, 14);
			switch (a)
			{
				case 1:
					tl = 38;
					break;
				case 2:
					tl = 30;
					break;
				case 3:
					tl = 41;
					break;
				case 4:
					tl = 43;
					break;
				case 5:
					tl = 44;
					break;
				case 6:
					tl = 45;
					break;
				case 7:
					tl = 46;
					break;
				case 8:
					tl = 47;
					break;
				case 9:
					tl = 75;
					break;
				case 10:
					tl = 76;
					break;
				case 11:
					tl = 119;
					break;
				case 12:
					tl = 121;
					break;
				case 13:
					tl = 122;
					break;
			}
			a = WorldGen.genRand.Next(1, 16);
			switch (a)
			{
				case 1:
					wl = 4;
					break;
				case 2:
					wl = 5;
					break;
				case 3:
					wl = 6;
					break;
				case 4:
					wl = 10;
					break;
				case 5:
					wl = 11;
					break;
				case 6:
					wl = 12;
					break;
				case 7:
					wl = 17;
					break;
				case 8:
					wl = 18;
					break;
				case 9:
					wl = 19;
					break;
				case 10:
					wl = 20;
					break;
				case 11:
					wl = 21;
					break;
				case 12:
					wl = 23;
					break;
				case 13:
					wl = 24;
					break;
				case 14:
					wl = 25;
					break;
				case 15:
					wl = 26;
					break;
			}

			for (cx = start; cx < width + start; cx++)
			{
				int xc;
				for (xc = ypos; xc < bottom; xc++)
				{
					if ((cx == start) || (cx == width + start - 1))
					{
						Main.tile[cx, xc].type = (byte)tl;
						Main.tile[cx, xc].active(true);
						Main.tile[cx, xc].slope(0);
						Main.tile[cx, xc].halfBrick(false);
					}
					else
					{
						WorldGen.KillTile(cx, xc, false, false, false);
						Main.tile[cx, xc].wall = (byte)wl;
					}
				}
			}

			InformPlayers();
			args.Player.SendSuccessMessage("Going down?");
		}
		#endregion
		#region GenMineHouse Command
		private void DoMineHouse(CommandArgs args)
		{
			int tryX = args.Player.TileX;
			int tryY = args.Player.TileY;
			WorldGen.MineHouse(tryX, tryY + 1);
			args.Player.SendSuccessMessage("Attempted to generate a Mine House here.");
			InformPlayers();
		}
		#endregion
		#region GenIslandHouse Command
		private void DoIslandHouse(CommandArgs args)
		{
			int tryX = args.Player.TileX;
			int tryY = args.Player.TileY;
			WorldGen.IslandHouse(tryX, tryY + 1);
			args.Player.SendSuccessMessage("Attempted to generate an Island House here.");
			InformPlayers();
		}
		#endregion
		#region GenFloatingIsland Command
		private void DoIsland(CommandArgs args)
		{
			int tryX = args.Player.TileX;
			int tryY = args.Player.TileY;
			if (tryY <= 50)
				tryY = 51;
			WorldGen.CloudIsland(tryX, tryY - 50);
			args.Player.SendSuccessMessage("Attempted to generate a floating island above you.");
			InformPlayers();
		}
		#endregion
		#region GenCloudIsland Command
		private void DoCloudIsland(CommandArgs args)
		{
			int tryX = args.Player.TileX;
			int tryY = args.Player.TileY;
			WorldGen.CloudIsland(tryX, tryY + 9);
			WorldGen.IslandHouse(tryX, tryY + 1);

			args.Player.SendSuccessMessage("Attempted to generate an un-looted floating island at your position.");
			InformPlayers();
		}
		#endregion
		#region GenLivingTree Command (TODO)
		// Needs revision, does not work for some reason
		private void DoLivingTree(CommandArgs args)
		{
			int tryX = args.Player.TileX;
			int tryY = args.Player.TileY;
			WorldGen.GrowLivingTree(tryX, tryY);

			args.Player.SendSuccessMessage("Attempted to grow a living tree at your position.");
			InformPlayers();
		}
		#endregion

		#region GenChests Command
		private void DoChests(CommandArgs args)
		{
			if (args.Parameters.Count == 0 || args.Parameters.Count > 2)
			{
				args.Player.SendInfoMessage("Usage: /genchests <amount> [gen mode: default/easy/all]");
			}
			int empty = 0;
			int tmpEmpty = 0;
			int chests = 0;
			int maxChests = 1000;

			string setting = "default";
			if (args.Parameters.Count > 1)
			{
				setting = args.Parameters[1];
			}
			const int maxtries = 100000;
			Int32.TryParse(args.Parameters[0], out chests);
			const int threshold = 100;
			if (!config.UseInfiniteChests)
			{
				for (int x = 0; x < maxChests; x++)
				{
					if (Main.chest[x] != null)
					{
						tmpEmpty++;
						bool found = false;
						foreach (Item itm in Main.chest[x].item)
							if (itm.netID != 0)
								found = true;
						if (found == false)
						{
							empty++;
							//      TShock.Utils.Broadcast(string.Format("Found chest {0} empty at x {1} y {2}", x, Main.chest[x].x,
							//                                           Main.chest[x].y));

							// destroying
							WorldGen.KillTile(Main.chest[x].x, Main.chest[x].y, false, false, false);
							Main.chest[x] = null;

						}

					}

				}
				args.Player.SendSuccessMessage("Uprooted {0} empty out of {1} chests.", empty, tmpEmpty);
			}
			else
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
							string sql = Path.Combine(TShock.SavePath, "chests.sqlite");
							ChestDB = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));
							break;
					}
				}
				catch (Exception ex)
				{
					TShock.Log.ConsoleError(ex.ToString());
				}
			}
			if (chests + tmpEmpty + threshold > maxChests)
				chests = maxChests - tmpEmpty - threshold;
			if (chests > 0)
			{
				int chestcount = 0;
				chestcount = tmpEmpty;
				int tries = 0;
				int newcount = 0;
				while (newcount < chests)
				{
					int contain;
					if (setting == "default")
					{
						// Moved item list into a separate .txt file
						int[] itemID = config.DefaultChestIDs;
						contain = itemID[WorldGen.genRand.Next(0, itemID.Length)];
					}
					else if (setting == "all")
					{
						// Updated item list to 1.2.4.1
						contain = WorldGen.genRand.Next(ItemList[0], ItemList.Last() + 1);
					}
					else if (setting == "easy")
					{
						contain = WorldGen.genRand.Next(-24, 364);
					}
					else
					{
						args.Player.SendWarningMessage("Warning! Typo in second argument: {0}", args.Parameters[1]);
						return;
					}
					int tryX = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					int tryY = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 200);
					if (!config.GenInsideProtectedRegions && IsProtected(tryX, tryY))
						continue;
					while (!Main.tile[tryX, tryY].active())
					{
						tryY++;
					}
					tryY--;
					WorldGen.KillTile(tryX, tryY, false, false, false);
					WorldGen.KillTile(tryX + 1, tryY, false, false, false);
					WorldGen.KillTile(tryX, tryY + 1, false, false, false);
					WorldGen.KillTile(tryX + 1, tryY, false, false, false);

					if (WorldGen.AddBuriedChest(tryX, tryY, contain, true, 1))
					{
						chestcount++;
						newcount++;
						if (config.UseInfiniteChests)
						{

							StringBuilder items = new StringBuilder();
							Terraria.Chest c = Main.chest[0];
							if (c != null)
							{
								for (int j = 0; j < 40; j++)
								{
									items.Append(c.item[j].netID + "," + c.item[j].stack + "," + c.item[j].prefix);
									if (j != 39)
									{
										items.Append(",");
									}
								}
								try
								{
									ChestDB.Query("INSERT INTO Chests (X, Y, Account, Items, WorldID) VALUES (@0, @1, '', @2, @3)",
											c.x, c.y, items.ToString(), Main.worldID);
								}
								catch (Exception ex)
								{
									TShock.Log.ConsoleError(ex.ToString());
								}
								items.Clear();
								Main.chest[0] = null;
							}
						}

					}
					if (tries + 1 >= maxtries)
						break;

					tries++;
				}
				if (config.UseInfiniteChests)
					ChestDB.Dispose();
				args.Player.SendSuccessMessage("Generated {0} new chests - {1} total", newcount, chestcount);
				InformPlayers();
			}
		}
		#endregion

		#region Utils
		#region InformPlayers
		//Updating all players
		public static void InformPlayers(bool hard = false)
		{
			foreach (TSPlayer person in TShock.Players)
			{
				if ((person != null) && (person.Active))
				{
					for (int i = 0; i < 255; i++)
					{
						for (int j = 0; j < Main.maxSectionsX; j++)
						{
							for (int k = 0; k < Main.maxSectionsY; k++)
							{
								Netplay.serverSock[i].tileSection[j, k] = false;
							}
						}
					}
				}
			}

		}
		#endregion
		#region IsProtected
		bool IsProtected(int x, int y)
		{
			var regions = TShock.Regions.InAreaRegion(x, y);
			foreach (var region in regions)
			{
				if (region.DisableBuild)
					return true;
			}
			return false;
		}
		#endregion

		private List<int> ItemList
		{
			get
			{
				List<int> list = new List<int>();
				var items = typeof(Terraria.ID.ItemID).GetFields();
				for (int i = 0; i < items.Length; i++)
				{
					list.Add((int)items[i].GetValue(items[i]));
				}
				list.Sort();
				return list;
			}
		}
		#endregion
	}
}
