# Stormworks Lua Extract

## Features

* Automatically extracts all Lua scripts from all controllers on **saved** vehicles.
* Writes Lua scripts to local .lua files that you can edit in your favorite editor outside the game.
* Automatically updates vehicle save files for rapid prototyping.
* Automatically adds/removes scripts as they're added/deleted from vehicles in-game. <sup>once you save the vehicle</sup>
* Automatically **minifies** the Lua code if it's larger than the maximum amount of allowed characters. Typically it can cut down a 6000 character script to about 2000.
	* This allows you to freely write longer variable names and comments.  
	All comments are removed from the final script.

YouTube:  
[![YouTube Video](https://img.youtube.com/vi/9sFofudtIb0/0.jpg)](https://www.youtube.com/watch?v=9sFofudtIb0)

This application extracts the lua scripts from all vehicles' microcontrollers, and puts them into .lua files on your disk.

When you edit these .lua files, they are automatically written back into the vehicle's save file. This allows you to edit and test your scripts more quickly.

## Warning
Still in beta, in the worst case, it may corrupt or delete a saved vehicle.

Inbuilt in the code is a backup system. Before overwriting a local extracted .lua file, OR a vehicle's save file, it will write the original content to a new file in the "Backup" directory next to the application's .exe. If something goes wrong, try looking in there for a recent backup.

## Running it
Extract the .zip and run `StormworksLuaExtract.exe`. It's in there somewhere.

Once it starts up, it will create a folder called "Workspace". In here, it will create .lua files extracted from the vehicles.
This only works for vehicles you have **saved**, so make sure you saved your vehicle in Stormworks first.

You can start editing these files, when you save, it should automatically update the vehicle's XML file.

In game, you'll have to re-load the vehicle in the editor to load its new script.

As of v0.0.3, any changes you make in the editor are also reflected to the local .lua files. This includes adding, deleting and modifying Lua scripts.

[![Build Status](https://travis-ci.com/Rene-Sackers/StormworksLuaExtract.svg?branch=master)](https://travis-ci.com/Rene-Sackers/StormworksLuaExtract)