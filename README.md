# World Refill - TShock Plugin 🛠️

> World Refill Plugin by k0rd, IcyPhoenix and Enerdy (Updated by Illuminousity/Matheis)

## Introduction 🗺

This plugin has been vastly improved from the original and comes with a number of performance improvements
/ more checks in place to make sure generation isnt unnatural. The aim of this plugin is to keep a perfectly
natural world whilst having all the resources you could want without ruining it, however this is up to the user to decide.

I cannot stress this enough, please do not give this command to users, at current this plugin does not work with tshock regions and performing the /gen world command
could destroy your spawn etc.

Please use with care!

Most importantly though, have fun!
 
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
| minehouse | Spawns a minehouse in the world |/gen minehouse |
| hellevator | Spawns a hellevator in the world |/gen hellevator |
| island | Spawns a floating island in the world |/gen island (type of island) |
| world | Resets the World to its original state! | /gen world true [seed] |


## Permissions 🚫

* Generation Command (/gen) : "worldrefill.generate"
* To Reload the config use the /reload command, it is hooked onto there.


 
## References 💾
References/Dependencies can be found in the References folder of the Directory
These are up to date as of 12/06/2020

## Build 🧱
You can find the compiled dll in WorldRefill>bin>release (Worldrefill.dll)


## Release History ⌚

* 2.0

   * Reloading of the config will now take place when using /reload instead of using /genreload
   
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

* 2.0.4
	* Dungeon and Pyramid now use a more accurate and refined method to calculate whether the player is on the surface of the world!

	* Hellevator has been added, this has been re-worked, there is now a fixed width and it now accurately calculates the position of hell.

* 2.1
	* We have gone asynchronous! This comes with huge performance improvements and less lag!

	* World has been added, this is still in a developmental state, spamming this command will lead to a crash this is due to terraria trying to re-generate the world multiple times

	* World resets the world to the seed of the world.

	* A limit to amount of 32767 has been implemented, this is because of world limitations and how much "stuff" a world can hold.

	* Code has been split up, generation functions have been moved to Regen.cs and Validations that were used have been moved to Validation.cs and the Creation and Reading
of the config has been moved to Config.cs.

* 2.1.1
	* Removal of Test debug feature, this was accidentally left in!	
	
	* Config has been moved to its own folder for less complicated access.

	* Fixed issue where spamming /gen world true would cause a crash

	* Floating islands have been implemented! the normal variant still needs some work and tile validation still has to be implemented.

	* Fixed issue where using extremely high numbers for the generation would cause a crash!

	* Fixed issue where spawning pyramid would get you stuck.

* 2.1.2
	* Floating Islands Tile Validation has been added, this is based on natural generation so you would need to be around the same Y-level as other islands

	* Skylake has been added to islands!

	* Grass now grows on the floating island! (In order to grow trees do /gen trees)

	* World can now take a seed input, leave seed empty to generate world with the same seed, this also applied to special seeds such as "not the bees!"

	* onSurface has now been fixed, you must be standing on blocks with both feet!

	* Added a Log so that you can keep track of every single generation

	* A sound now plays when a generation is completed!

	* InfChests3 Support has been added! on generation it will now automatically send the chests generated to the db!

* 2.1.3
	* Fixed Issue Where InfChests3 support would not work for SQLite.
	
	* Removed Debug information that i accidentally left in :P

	

	


## Currently Working On!

* Chests, everything Chests, full infchests support!




## To Implement



* Temple (this one will be difficult)

* Living Trees (will also be difficult)

* Chests (will also be difficult lol)

* Anything else that has been added to 1.4/ Suggestions from external testers?

* World Regenerations do not affect region protected structures.


## Contact Info

Discord : Matheis#0489


