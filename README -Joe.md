# YINYANGFishy3D
Developer: Joe
Student ID: 
Submission Date: December 30, 2024
Declaration: All content in this version was independently completed by Joe.

Independently Completed Features
Core Features
Enemy Fish AI:

1.Developed a simple AI system for enemy fish, driven by Rigidbody components.

	Implemented random movement and rotation, with direction changes occurring every few seconds.

	Ensured that enemy fish behavior appeared natural and unpredictable.

2.Enemy Fish Spawner:

	Set a maximum spawn limit for enemy fish.

	Continuously monitored the fish count and spawned enemy fish prefabs at 5-second intervals.

	Randomized fish colors (yellow, blue, red) and adjusted local scales to enhance diversity.

3.Boss Fish AI:

	Created a sophisticated AI for the boss fish, including idle, chasing, and attack states.

	Utilized Rigidbody for natural movement and state transitions based on player proximity.

4.Boss Fish Animation:

	Designed animations for idle, chasing, and attack behaviors using Unity's animation component.

	Triggered specific animations based on the boss fish's state.

	Implemented animation events to control the timing of attack collisions.

5.Boss Interaction:

	Added collision detection for boss attacks on pillars using OnCollisionEnter.

	Ensured that boss attacks had appropriate impact on the environment.

6.Interactive Items:

	Physics Rope: 
	Created using Unity’s Hinge Joint component for realistic simulation.

    	Bomb: 
	Utilized Physics.OverlapSphereNonAlloc to detect nearby items.

	Distinguished between players and bosses, executing specific functions upon detection.

	Controlled sound effects using animation events for a polished experience.

	Key & Lock:
	Assigned unique IDs to keys and locks.

	Implemented collision detection to verify matching IDs and destroy locks accordingly.

7.Eating Fish Mechanic:

	Enabled player to eat smaller fish by using Physics.OverlapSphereNonAlloc to detect nearby fish.

	Increased the player's YinYang value upon successful consumption.

8.Player Death Logic:

	Disabled the player controller upon death, followed by a 3-second respawn delay.

	Ensured the player respawned at the designated birth place.

9.Dead UI Animation:

	Created an animation for the death UI to visually indicate player death.

	Triggered this UI and animation upon player death.

10.Winning Logic:

	Added logic to play a sound effect and display the win UI upon reaching the final position.

	Provided a satisfying conclusion to the player's journey.

11.Pause Menu:

	Implemented a pause menu to control game time using Time.timeScale.
	
	Allowed the player to pause and resume the game using the ESC key or pause UI.

12.Main Menu:

	Developed the main menu scene with a start game function.
	
	Created a scene load manager to handle scene transitions seamlessly.

13.Post-processing Enhancements:

	Improved visual quality with post-processing effects including bloom, depth of field, and color grading.

	Enhanced the overall aesthetic and atmosphere of the game.

14.Sound Effects:

	Added sound effects for eating, bomb, and left-click interactions.

	Enhanced the auditory experience to make the game more immersive.

Reference videos:
https://www.youtube.com/watch?v=ZlYo_-gyJzA
https://www.youtube.com/watch?v=OuZrhykVytg&t=167s
https://www.youtube.com/watch?v=nbxiqHCsYFg
https://www.youtube.com/watch?v=Zzklpdvue3A
https://www.youtube.com/watch?v=MElbAwhMvTc
https://www.youtube.com/watch?v=Z2hD0WxCFak
