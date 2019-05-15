This application allows you to edit the Stormworks microcontroller Lua objects in an external editor by extracting the scripts, and creating separate .lua files.

When you change a .lua file, it'll then back-up your microcontroller .xml file and overwrite it with the updated script content.

## Warning
This is very beta, more of a proof of concept. The code is garbage, but it seems to work.

When you save a Lua file, it will first create a backup of the microcontroller xml file to at least try and prevent loss of code/controller stuff.

## Requirements
You need to have the [.NET Core 2.2 runtime](https://dotnet.microsoft.com/download/thank-you/dotnet-runtime-2.2.5-windows-hosting-bundle-installer) installed.

## Running it
Once .NET is installed, and you extracted the .zip from the releases, open a command prompt in the directory containing `StormworksLuaExtract.dll` and run the following command:

```
dotnet StormworksLuaExtract.dll
```

Once it starts up, it will create a folder called "Workspace". In here, it will create .lua files from the controllers in the microcontroller xml files.

You can start editing these files, when you save, it should automatically update the microcontroller XML file.

In game, you'll have to re-load the microcontroller to get the new scripts.