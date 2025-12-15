## Overview
**9BitPaletteConverter** is a C# console application designed to convert image colours into the 24-bit RGB representation of the 9-bit palette format used by the Sega Mega Drive. It supports multiple image formats such as `.jpg`, `.jpeg`, `.png`, `.gif` and `.bmp`.

### Theory
The Sega Mega Drive's colour format is represented by a 12-bit mask in the form:
```
BBB0 GGG0 RRR0
```
Where the fourth bit in each colour channel is always set to `0`. This allows to represent colours more efficiently using a 3-digit format, with possible values being:

- 0 (0000)
- 2 (0010)
- 4 (0100)
- 6 (0110)
- 8 (1000)
- A (1010)
- C (1100)
- E (1110)

The Mega Drive ignores the fourth, eighth, and twelfth bits (which are zero), leaving just 9 bits, i.e. 3 bits for each colour channel (Red, Green, and Blue).

The maximum channel value, `111`, represents full intensity. When mapped to 24-bit RGB (8 bits per channel), this corresponds to a value of 255 (#FF). Since the 3-bit channel provides 8 discrete intensity levels, there are 7 steps between minimum and maximum, giving an approximate step size of:
```
255 / 7 ≈ 36.4285
```
Thus, when converted and rounded to the nearest integer, the possible palette values are:
- 0
- 36
- 73
- 109
- 146
- 182
- 219
- 255

#### Analogue Nature of the Mega Drive
While the Sega Mega Drive defines its colours digitally, the console outputs video as an analogue signal. The final colours displayed therefore depend on the characteristics of the DAC, video encoder, cabling, and the display itself. As a result, the image can appear slightly washed out or dimmed. For example, the colours on real hardware may look similar to those produced by the **BlastEm** emulator, which attempts to mimic these analogue imperfections.

## Requirements
- .NET 8 Runtime or ASP.NET Core Runtime

## How To Build
```
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true /p:PublishTrimmed=false
```

## License
This project is open source and available under the MIT License.
