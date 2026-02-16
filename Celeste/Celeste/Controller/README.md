## Completed
Added Controller Interface and implemented concrete classes for the Keyboard and Mouse controllers. Began work on the controller loader that loads the controllers during the LoadContent() class in Game1.

## To-Do
-Need Command Interface and Concrete classes to implement different commands. 
-Need to finish Controller Loader
-Need to map keys to different actions, ie. movement and switching between sprite scenes (Sprint 2 requirement)

## Explanation
Keyboard controller and mouse controller and implemented by using Dictionarys (key and position of mouse on screen, Command). Every key can be mapped for different functions, and mouse functions can also be mapped based on position on the screen and whether the input is left or right click. 