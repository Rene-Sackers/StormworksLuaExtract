This application allows you to edit the Stormworks microcontroller Lua objects in an external editor by extracting the scripts, and creating separate .lua files.

When you change a .lua file, it'll then back-up your microcontroller .xml file and overwrite it with the updated script content.

## Warning
Still in beta, in the worst case, it may corrupt or delete a saved microcontroller.

When you save a Lua file, it will first create a backup of the microcontroller xml in the folder "Backups" next to the application's executable to at least try and prevent loss of code/controller stuff.

## Running it
Extract the .zip and run `StormworksLuaExtract.exe`. It's in there somewhere.

Once it starts up, it will create a folder called "Workspace". In here, it will create .lua files from the controllers in the microcontroller xml files.

You can start editing these files, when you save, it should automatically update the microcontroller XML file.

In game, you'll have to re-load the microcontroller to get the new scripts.