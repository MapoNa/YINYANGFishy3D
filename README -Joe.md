# YINYANGFishy3D
IS71030D: GAMES PROGRAMMING 1 (2024-25)
Developer: Yufei Liu
Student ID: 33705656
Submission Date: December 21, 2024
Declaration: All content in this version was independently completed by Yufei Liu.
Independently Completed Features
Core Features
Player Movement and Control:

Implemented fish movement using Rigidbody.velocity, supporting smooth turning.
Added jump and dive mechanics, with jump force dynamically influenced by dive depth.
Dynamically adjusted movement speed under extreme Yin-Yang values.
Camera Smooth Follow:

Designed a dynamic camera follow system with synchronized rotation and offset adjustment.
Player Interaction:

Enabled interaction with seaweed to adjust Yin-Yang values and scale, ignoring interactions with regrowing seaweed.
Designed interaction logic for collectible items, prioritizing the closest item and maintaining a collection count.
Physics and Visual Enhancements
Yin-Yang Mechanic:

Dynamically adjusted Yin-Yang values based on lighting conditions.
Added red flashing effect to the fish material as a warning when Yin-Yang values reach critical levels.
Particle Effects:

Triggered particle effects for leaving and entering water based on the fish's vertical position.
Fish Model and Animation:

Created swimming animations for the fish model.
Scaling Optimization:

Adjusted only the parent object's scale during size changes to avoid affecting child objects.
Level Design
Gameplay Mechanics:

Designed levels based on bomb-wall mechanics, item collection, and cork unlocking.
Implemented door interaction using red ball mechanics with dynamic pull force and range adjustments.
Interaction Optimization:

Fixed collision jitter issues during player-object interactions.
Prevented the fish model from interfering with shadow detection logic.
