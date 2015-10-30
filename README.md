______              ______          _    
| ___ \             | ___ \        | |   
| |_/ /__ _ _ __ ___| |_/ /_ _  ___| | __
|    // _` | '__/ __|  __/ _` |/ __| |/ /
| |\ \ (_| | | | (__| | | (_| | (__|   < 
\_| \_\__,_|_|  \___\_|  \__,_|\___|_|\_\
                                         
                                         

Introduction
------------------------
RarcPack is a library that allows the creation of RARC archives, commonly used in Nintendo's GameCube games, without the need for an external program.
Before this library was created, it was necessary to use an external RARC packing program to create an archive.

Usage
------------------------
The library utilizes the classes VirtualFolder and FileData to approximate the final structure of the archive.

To use the library, first build the structure of the archive, with VirtualFolder for directories and FileData for files. Then, pass the root of the structure
to Pack(VirtualFolder root, EndianBinaryWriter writer) in an instance of RARCPacker. This will output a RARC archive at the filepath specified in writer.

Requirements
------------------------
This library relies on GameFormatReader.dll to function. It can be found here:

https://github.com/lioncash/GameFormatReader

Contributors
------------------------
Sage of Mirrors/Gamma (Author)
thakis (SZS/RARC research)
LordNed (Moral support)

Support
------------------------
If there are any issues, contact Sage of Mirrors on GitHub (https://github.com/Sage-of-Mirrors) or Gamma on jul (http://jul.rustedlogic.net/profile.php?id=2052) or #zelda on irc.badnik.net.

Changelog
------------------------
v1.0 - Release
