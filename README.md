[![Build Status](https://travis-ci.com/Rene-Sackers/StormworksLuaExtract.svg?branch=master)](https://travis-ci.com/Rene-Sackers/StormworksLuaExtract)

This application extracts the lua scripts from all vehicles' microcontrollers, and puts them info .lua files on your disk.

When you edit these .lua files, they are automatically written back into the vehicle's save file. This allows you to edit and test your scripts more quickly.

![Screenshot](https://i.imgur.com/UrL2lY3.gif)

## Warning
Still in beta, in the worst case, it may corrupt or delete a saved vehicle.

Inbuilt in the code is a backup system. Before overwriting a local extracted .lua file, OR a vehicle's save file, it will write the original content to a new file in the "Backup" directory next to the application's .exe

## Running it
Extract the .zip and run `StormworksLuaExtract.exe`. It's in there somewhere.

Once it starts up, it will create a folder called "Workspace". In here, it will create .lua files extracted from the vehicles.

You can start editing these files, when you save, it should automatically update the vehicle's XML file.

In game, you'll have to re-load the vehicle to load its new script.