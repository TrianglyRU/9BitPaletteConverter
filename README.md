# 9BitPaletteConverter

## Overview
9BitPaletteConverter is a C# console application that converts the colours of an image to a linearly scaled palette used by the Sega Mega Drive. The palette values are restricted to the following set:

- 0
- 36
- 73
- 109
- 146
- 182
- 219
- 255

## Features
- Converts image colours to the Mega Drive-compatible palette.
- Supports popular image formats such as `.jpg`, `.jpeg`, `.png`, and `.bmp`.
- Processes entire directories of images.

## Requirements
- .NET Framework or .NET Core SDK installed.
- A folder containing images to be processed.

## Notes
- The input folder must be named `sprites`. If the folder name is different, the application will prompt you to try again.
- The output folder (`sprites_modified`) will be created automatically in the same directory as the original `sprites` folder.

## License
This project is open source and available under the MIT License.