# Luminal
*Simplicity is the key to power.*

## What can you do with this?
Luminal is a game engine, in C#, for C#.
It has support for hardware-accelerated 2D and 3D graphics, audio, handling input, and most things you'd expect.

## Progress at the moment
This engine is extremely work-in-progress, and **is not stable**. Please do not yell at us when it crashes, until release.

## The logger
To make a custom logger, write a class that implements ILogger, then call `Log.SetLogger(ILogger yourLogger);` after constructing Engine.

## Operating system support
**Luminal is Windows only.**  
I will not change this. Ever. It would be far too much dev time for a platform I don't even use.

## Roadmap
- [x] Basic 2D graphics
- [x] Window
- [x] Audio
- [x] Keyboard/mouse input
- [x] Flexible logging
- [X] IMGUI support
- [X] Expose more of the engine to the end user
- [X] Extremely basic 3D support
- [X] More advanced 3D support

## Credits
Luminal is a [Lunar Diaspora](https://github.com/LunarDiaspora) project.  
All code by [Rin](https://github.com/ry00001).  
Uses [SDL2](https://libsdl.org), using SDL2-CS.  
Powered by [SDL_gpu](https://github.com/grimfang4/sdl-gpu) and [OpenTK](https://github.com/opentk).
Special thanks to the .NET team, and especially the .NET 5.0 developers.