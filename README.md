# Luminal
*Simplicity is the key to power.*

## What can you do with this?
Luminal is a game engine, in C#, for C#.
It has support for hardware-accelerated 2D graphics, audio, handling input, and most things you'd expect.

## Progress at the moment
This engine is extremely work-in-progress, and **is not stable**. Please do not yell at us when it crashes, until release.

## The configuration
In the folder where Luminal.dll resides in the host application, make a file called `Luminal.json`.
*Luminal applications need to do this.*
Example config:
```json
{
	"AudioEngine": "LuminalFMODCoreEngine"
}
```
Luminal will then attempt to load the audio engine specified by that key.

## The logger
To make a custom logger, write a class that implements ILogger, then call `Log.SetLogger(ILogger yourLogger);` after constructing Engine.

## Roadmap
- [x] Basic 2D graphics
- [x] Window
- [x] Audio
- [x] Keyboard/mouse input
- [x] Flexible logging
- [ ] IMGUI support
- [ ] Expose more of the engine to the end user
- [ ] Finish LGUI
- [ ] Extremely basic 3D support

## Credits
Luminal is a [Lunar Diaspora](https://github.com/LunarDiaspora) project.  
All code by [Rin](https://github.com/ry00001).  
Uses [SDL2](https://libsdl.org), using SDL2-CS.  
Powered by [SDL_gpu](https://github.com/grimfang4/sdl-gpu) and [OpenTK](https://github.com/opentk).
Special thanks to the .NET team, and especially the .NET 5.0 developers.