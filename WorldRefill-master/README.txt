World Refill Plugin by k0rd, IcyPhoenix and Enerdy (Updated by Illuminousity/Matheis)

/* REFERENCES */

References/Dependencies can be found in the References folder of the Directory, These are up to date as of 27/05/2020


/* BUILD */

You can find the compiled dll in WorldRefill>bin>release (Worldrefill.dll)



/* CHANGELOG */

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

/* WIP */

The rest of the options need to be implemented, this should take a couple of days max hopefully if i do not run into any bugs.
I also plan to add some more new features, like the ability to regenerate a whole new complete temple! and others

