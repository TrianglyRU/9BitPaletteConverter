## Overview
**9BitPaletteConverter** is a C# console application designed to convert image colours into the hexadecimal / RGB representation of the 9-bit palette format used by the Sega Mega Drive.

### Theory
The Sega Mega Drive's colour format is represented by a 12-bit mask in the form:
```
BBB0 GGG0 RRR0
```
Where the fourth bit in each colour channel is always set to `0`. This allows developers to represent colours more efficiently using a 3-digit format, with possible values being:

- 0 (0000)
- 2 (0010)
- 4 (0100)
- 6 (0110)
- 8 (1000)
- A (1010)
- C (1100)
- E (1110)

The Mega Drive ignores the fourth, eighth, and twelfth bits (which are zero), leaving 9 bits—3 bits for each colour channel (Red, Green, and Blue).

Given that `111` is the brightest colour (#FFFFFF in hexadecimal or 255,255,255 in RGB), and since there are 7 unique combinations of 3 bits (excluding `000`), changing one bit roughly equals one step of:
```
255 / 7 ≈ 36.4285
```
Thus, the possible palette values are restricted to the following set (rounded to the nearest integer):

- 0
- 36
- 73
- 109
- 146
- 182
- 219
- 255

#### Analogue Nature of the Mega Drive
It's important to note that the Sega Mega Drive is an analogue console, meaning it doesn't output colours as fixed digital values. Instead, the colours are determined by the voltage and signal strength being sent to the display. As a result, the actual colours produced by the hardware can vary, appearing slightly "washed out" or even dimmed. For example, the colours on real hardware may look similar to those produced by the **BlastEm** emulator, which attempts to mimic these analogue imperfections.

## Features
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
