# PaletteSwapper
Swaps certain colors based on reference lookup texture

## How to install
This is an Editor script for Unity. 
The file has to be in Assets/Editor for it to work.
You can open the tool, by going to Tools/PaletteSwapper.

## How it works
Specify a name and location for the new texture to be saved to.

The original texture is the base texture, that will be used to generate the new texture.
The reference texture should contain all the colors you want changed on the original texture (can only be 1 pixel tall).

The tolerance specifies how close the colors have to be, to be swapped. 
Use swapper specifies whether you want colors changed based on a lookup texture or by tinting.
If using swapper, your swap texture should be the same size as the reference texture. When the script finds a match in the reference texture, it will replace that pixel with the one at that same position in the swap texture.

## Recommendations
It's a simple script and should probably only be used with small palettes. 

## Bugs
None. I'm a god. JK, idfk, but feel free to reach out to me on twitter, if you find any: https://twitter.com/hjaltetagmose.
