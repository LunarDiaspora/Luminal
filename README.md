# Luminal
*A simple game framework, made for Supernova.*

## What can you do with this?
Luminal is a tiny toolkit for drawing things to the screen, grabbing input and so on.
Audio engines are modular, which means that you can swap them out at will.

## The configuration
In the folder where Luminal.dll resides in the host application, make a file called `Luminal.json`.
Example config:
```json
{
	"AudioEngine": "LuminalFMODCoreEngine"
}
```
Luminal will then attempt to load the audio engine specified by that key.

## The logger
To make a custom logger, write a class that implements ILogger, then call `Log.SetLogger(ILogger yourLogger);` after constructing Engine.