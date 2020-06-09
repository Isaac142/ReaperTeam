Trajectory Guide:

The "TrajectoryPredictor" and "TrajectoryPredictor3D" scripts are the scripts that you bought this asset for.
These scripts requires you to input a velocity value, and take the angle of launch from the rotation of the object these scripts are placed on.

To start, place one of these scripts on the object that will both be rotating according to the angle of launch, 
and also in the position that you want the trajectory line to start in.

--------------------------
--------------------------

Parameters:

The "Velocity" parameter is the velocity at which you will be launching your projectile in the x-axis direction. 
This affects the shape of the trajectory line so be sure that this is an accurate value.

The "Time Between Points" line is how much time (in seconds) it takes to generate a new point after the previous one has been generated. 
Use a higher number for greater accuracy in the line's object collision.

The "Max Number Of Points" parameter controls how many points can be generated at one time by this script. 
This, combined with the "Time Between Points" parameter, controls how long the line is.
Increase this value to allow for a longer line, and decrease it to make a shorter line with greater performance potential.

The "Texture Object" is the object (preferably quad) that will be placed on the points of the line to make a dotted  line (or another kind of line you want). 
Leave blank if you would rather have the "Line Renderer" create your trajectory.

The "Crosshair" is the object that will be at the end of the line, effectively showing the impact location of your projectile. The crosshair is a GameObject whose y-axis is aligned
with the normal of the object hit by the Trajectory line.

The "Texture Object Divisor" parameter is an integer that dictates which points should have a "Texture Object" object on them. 
If this number is 2, then every other point will have a "Texture Object". If this number is three, then every third point will have a "Texture Object".
The number is 4, every fourth point, etc.

*The "ZDepth" Parameter is the z coord in which the line will reside. THE LINE ONLY PREDICTS 2D LINES ON THE X AND Y AXIS!

The "Gravity" parameter is the acceleration of gravity that will pull the object. By default it is set to the gravitational acceleration of earth.

The "Layers To Hit" parameter tells the trajectory line which layers are an obstacle in through which it can not pass.

**The "X Axis Forward" parameter sets the x axis as the direction you will be launching your projectile; the program also expects to be rotated around the
"z" and "y" axes (This is how it collects angle information). If this is turned off the z direction will be the direction in which the projectile should be fired;
the program also expects to be rotated around the "x" and "y" axes (This is how it collects angle information).



* Means it's in the 2D(TrajectoryPredictor) script only.
** Means it's in the 3D(TrajectoryPredictor3D) script only.

--------------------------
--------------------------

Extra Scripts:

*This asset package also comes with a basic "Launch" script so you can test the "Trajectory Predictor" right out of the box.
It has 2 simple parameters:
		
	The "Velocity" parameter is the velocity that the object will be launched at. 
	For accuracy sake, this number should be the same as your "Trajectory Predictor" velocity parameter.
	
	The "Rate Of Rotation" parameter is how many degrees the object will be rotated per frame.

	To increase the object's rotation press the left arrow key. To decrease the rotation press the right arrow key.
	To launch the object press the space bar.

The "GravityChanger" script allows you to change the gravity of an object in the X, Y, and Z directions:
	
	The "Acceleration Of Gravity" parameter let's you choose the acceleration of gravity in the X, Y, and Z directions.
	
The "Reloader" script reinstaniates an object:
	
	The "Projectile" parameter is the object that will be instantiated.
	
	The "Reload Time" is how long (in seconds) it takes to reload a "Projectile".

	The "Reload Pos Object" is where the object will be instantiated.

*The script that the "SetVelocity" and "Reloader" scripts are mainly derived from

--------------------------
--------------------------

Scenes:
	
The "LayerMask-Scene" simply demonstrates the ability to allow the trajectory arc to pass through certain objects,
or stop after hitting an object. Press the left and right arrow keys to rotate the cannon and press the space bar to fire.

The "Long-Range-Scene" demonstrates the accuracy of the "Trajectory Predictor" script. It also shows how useful it can be to
put the script on an object that rotates to the same degree as the projectile, but not on the projectile itself.
Press the left and right arrow keys to rotate the cannon and press the space bar to fire.

The "Changing-Gravity-Scene" demonstrates how you can change the gravity parameter to get desired results. This allows you
to "roll" your own physics! Press the left and right arrow keys to rotate the cannon and press the space bar to fire.

The "Multiple-Cannon-Scene" allows you to control several cannons at once and showcases the Line Renderer as the trajectory arc.
Press the left and right arrow keys to rotate the cannon and press the space bar to fire.

The "Horizontal-Gravity-Scene" demonstrates how you can have gravity force an object horizontally as well as vertically.
Press the left and right arrow keys to rotate the cannon and press the space bar to fire.

The "3D-Scene" demonstrates the 3D capabilities of Trajectory.
Press the left, right, up, and down arrow keys to rotate the cannon and press the space bar to fire.

--------------------------
--------------------------

Contact:
	
If you have any questions, feedback, or problems with Trajectory feel free to contact me at swankyassets@gmail.com
	
--------------------------
--------------------------

Notes:
	
	If you have the inspector on an object with the "Trajectory Predictor" script on it while in play mode Unity will sometimes
	go very slowly. When you don't have the inspector on it it moves very smoothly. Use caution when using high-accuracy arcs!
	
	There is a variable in the "Trajectory Predictor" script named "hasFired" that will freeze the rotation of the trajectory arc.
	This is an internal variable so you must access it with code.
	
	The the system that calculates whether or not the line is intersecting an object is calculated EVERY POINT ON THE LINE.
	If the line is going through an object that it is supposed to hit either decrease the "Time Between Points" parameter,
	or make the object thicker.
	
	There is an inconsistent bug that happens when the "Texture Object" prefab has a "Mesh Collider" that is on or off.
	I believe this is a Unity error and not of my own making, but I may be wrong. 
	I recommend removing colliders from the "Texture Object" prefab.