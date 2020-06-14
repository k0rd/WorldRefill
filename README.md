# World Refill - TShock Plugin 🛠️

> World Refill Plugin by k0rd, IcyPhoenix and Enerdy (Updated by Illuminousity/Matheis)
 
## Commands List 

| Command        |Description           |Usage  |
| ------------- |:-------------:| -----:|
| gen      |Command Used to generate objects |/gen (option) [number required] [specific type of certain option] |
| genrewind | tba | tba |


## Options List
| Option	|Description		|Usage	|
| :-------------: |:-------------:| :-----:|
| crystals   | Life Crystals |/gen crystals (number of crystals) |
| pots  | Pots |/gen pots (number of pots) |
| orbs | Orbs |/gen orbs (number of orbs) |
| altars | Altars |/gen altars (number of altars) |
| cavetraps | Traps that spawn in caves naturally |/gen cavetraps (number of cavetraps) |
| templetraps | Traps that spawn in the temple naturally|/gen templetraps (number of templetraps) |
| lavatraps | Hidden lava traps in the world that drowns them in lava |/gen lavatraps (number of lavatraps) |
| sandtraps | Traps hidden in the desert to make sand fall on players |/gen sandtraps (number of sandtraps) |
| statues | Statues |/gen statues (number of statues) [type of statue] |
| ores | Ores |/gen ores (number of ores) [type of ore] |
| webs | Spawns Webs in Spider biomes |/gen webs (number of webs) |
| shrooms | Spawns mushrooms in their respective biome |/gen shrooms (number of shrooms) |
| trees | Spawns trees in the world |/gen trees |
| dungeon | Spawns a dungeon in the world |/gen dungeon |
| pyramid | Spawns a pyramid in the world |/gen pyramid |
| minehouse | Spawns a minehouse in the world |/gen minehouse


 
## References 💾
References/Dependencies can be found in the References folder of the Directory
These are up to date as of 12/06/2020

## Build 🧱
You can find the compiled dll in WorldRefill>bin>release (Worldrefill.dll)

## Changelog ⌛

Why Version 2.0? Here's why:

Using a switch statement instead of a command for each possible generation, makes things look alot neater :)

Maxtries now can be customised in the config json file! this controls how many attempts it has at generating an object.

Pots now actually generate their biome variants, no longer do we have the default pot in hell or in the dungeon!

Input validation has been implemented - checks are now in place to make sure that strings and 0/negative integers stay away! go away :(

the old trap generation used to just generate geysers, we now have a random number implemented so that it will switch between the various 
cave traps!

Temple traps have been implemented, this is completely new for this plugin, i went through the effort of checking for lihzahrd bricks in the vicinity of
the trap only to realize that the method itself did it for me :(

Lava traps have been implemented, this is also completely new for this plugin

Altered the altar generation making it so that in crimson worlds it will create crimson altars instead of demon altars

Also some minor changes that mainly affect the clarity of the feedback to the user.

Regex has been implemented and error generation has been changed so that errors are more dynamic.

Statues are fundamentally changed, they now utilize Terraria's built-in statuelist meaning that even if terraria updates to add statues,
they will also be added to the list. the spawning of specific statues now uses a dictionary, and includes more statues.

Ores have been implemented and refined - Chlorophyte now only spawns in the jungle and only replaces mud blocks
unnatural ores have been removed such as meteorite as that drops from meteors





## Release History ⌚

* 2.0
   
   * Using a switch statement instead of a command for each possible generation, makes things look alot neater :)

   * Maxtries now can be customised in the config json file! this controls how many attempts it has at generating an object.

  * Pots now actually generate their biome variants, no longer do we have the default pot in hell or in the dungeon!

  * Input validation has been implemented - checks are now in place to make sure that strings and 0/negative integers stay away! go away :(

  * The old trap generation used to just generate geysers, we now have a random number implemented so that it will switch between the various 
cave traps!

  * Temple traps have been implemented, this is completely new for this plugin, i went through the effort of checking for lihzahrd bricks in the vicinity of
the trap only to realize that the method itself did it for me :(

   * Lava traps have been implemented, this is also completely new for this plugin

   * Altered the altar generation making it so that in crimson worlds it will create crimson altars instead of demon altars

Also some minor changes that mainly affect the clarity of the feedback to the user.
* 2.0.1
	* Statues are fundamentally changed, they now utilize Terraria's built-in statuelist meaning that even if terraria updates to add statues,
they will also be added to the list. the spawning of specific statues now uses a dictionary, and includes more statues.

	* Ores have been implemented and refined - Chlorophyte now only spawns in the jungle and only replaces mud blocks
unnatural ores have been removed such as meteorite as that drops from meteors
* 2.0.2
   * Structures have been added, these generate structures as opposed to resources

   * Trees work the same way as before, this replenishes trees in the world surface

   * Shrooms have been re-worked, now it spawns shroom variants in their respective biome

   * Dungeon has been added, this still has to be checked for bugs
* 2.0.3
	* Dungeon has been re-worked, it now only spawns when standing on a block and away from a world border!
	
	* Pyramid has been added, this uses the same validation as dungeon, didnt know whether to only make it work when standing on sand
however i left it with any block as to give freedom to the user

	* Minehouse has been added, this is a bit buggy so far, it needs work and chests do not spawn on it

	* The barebones for Temple and Living trees are there, the problem with Temple is that it spawns the shape and the path but not traps, or objects in the temple.
Living tree also needs to be looked at when i get time.



	


## Contact Info

Discord : Matheis#0489


