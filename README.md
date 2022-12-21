# PSMS
Paul Smith Must Survive

Currently learning Unity and C#, using this project as a vehicle. It's a top-down twin-stick shooter (well, WASD and mouse atm).

The object of the game is to help Paul Smith survive until nightfall. If an enemy touches him, he dies instantly. 

----

Planned features:

* WASD movement with mouse controlling aiming position [IMPLEMENTED]
* Game controller support (twin stick)
* A range of weapon powerups [currently only one powerup exists, which boosts firing rate for 5 seconds]
* Dynamic generation of enemy wave data from JSON file [IMPLEMENTED]
* Variety of different enemy types [Currently 3 types exist, behaviour needs to be added]
* Persistent high scores
* Different environments
* Particle effects [Currently the bullets have particles and lighting, need to add hurt effects/explosions
* Boss fights
* Some sort of epic UI

----

JSON wave data formatting
-------------------------
The format for the wave data JSON file is very simple - you specify waves and subwaves. Each subwave contains the number of slow, medium and fast enemies should be spawned in that subwave. They should also include the delay in seconds until the next subwave spawns in. the final subwave in each wave should have a delay of 0.

For example:

    {"waves":[
	    {"subwaves":[
		    {"slow":0,"medium":3,"fast":0,"delay":10},
		    {"slow":0,"medium":4,"fast":0,"delay":10},
		    {"slow":0,"medium":5,"fast":2,"delay":0}]
	    },
	    {"subwaves":[
		    {"slow":0,"medium":3,"fast":1,"delay":10},
		    {"slow":0,"medium":3,"fast":2,"delay":10},
		    {"slow":0,"medium":3,"fast":3,"delay":0}]
	    }]
    }
    
Create a file called "waves.json" and drop it into your persistent data folder (~/.config/unity3d/Triplesix Studios/Paul Smith Must Survive/ on linux, %userprofile%\AppData\LocalLow\Triplesix Studios\Paul Smith Must Survive\ on Windows). The game will use this file for wave data if it exists, otherwise it will fall back on the build in wave data. In a later update I'll expand this to be a mod-loading function.
