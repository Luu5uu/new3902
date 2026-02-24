## Completed
-Added Controller Interface and implemented concrete classes for the Keyboard and Mouse controllers. 
-Added the controller loader that loads the controllers during the LoadContent() class in Game1.
-Added Command inteface and classes in order to handle various inputs
-Adjusted Game1 to meed Sprint2 cycling block and item requirement

## To-Do
-Integrate the PlayerCommand script into the Controller and Command scripts
-Complete mouse controller for eventual use if needed
-Add Sprint 3 functions

## Explanation
Keyboard controller is implemented by using a Dictionary (key, Command). Every key can be mapped for different functions. Command script handles the to-do of pressing keys, while controller handles the registering (controller loader assigns keys to in game inputs.)