## Completed
-Added Controller Interface and implemented concrete classes for the Keyboard and Mouse controllers. 
-Added the controller loader that loads the controllers during the LoadContent() class in Game1.
-Added Command inteface and classes in order to handle various inputs
-Adjusted Game1 to meed Sprint2 cycling block and item requirement

## To-Do
-Continue consolidating player control handling into the controller and command scripts
-Complete mouse controller for eventual use if needed
-Add Sprint 3 functions

## Explanation
Keyboard controller is implemented by using a Dictionary (key, Command). Every key can be mapped for different functions. Command script handles the to-do of pressing keys, while controller handles the registering (controller loader assigns keys to in game inputs.)

Current gameplay bindings follow the Celeste-style keyboard layout:
- Arrow keys are the primary movement / vertical aim inputs
- `Z` is grab / climb
- `X` is dash
- `C` is jump
- `Down` while grounded is crouch / duck
- touching a wall without grab starts a wall slide
- pressing jump while sliding on a wall performs a wall jump
- `WASD` remains available as a directional testing fallback
