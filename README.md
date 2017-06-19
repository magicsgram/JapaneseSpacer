# JapaneseSpacer
Japanese written texts don't contain spaces. Japanese Spacer adds spaces in between words. It also adds Furigana to help reading.

THIS IS FOR THE BEGINNERS' REFERENCE ONLY! I DO NOT GUARANTEE THE ACCURACY OF THE GLOBALIZATION LIBRARY SHIPPED WITH WIN10.

# Requirements
  - Windows 10 - 32/64 bit
  - .net framework 4.5

# Usage
Download the latest binary from https://github.com/magicsgram/JapaneseSpacer/raw/master/ReleaseBins/JapaneseSpacer.7z
1. Extract the file into a new folder
2. Run kylecapturetranslator.exe
3. Type or copy-paste japanese text onto the "text" area to the left.
4. The spaced(+furigana) result should automatically show up to the right.

# Screenshot
![alt text](https://github.com/magicsgram/JapaneseSpacer/raw/master/ScreenShots/1.3-1.PNG "Screenshot")

# Release Node
## Release 1.0
  - Initial release
## Release 1.1
  - Fixed behavior of adding furigana to katakana letters
## Release 1.2
  - Fixed behavior of adding furigana to non-letters
## Release 1.3
  - Allowed users to choose the font, defaulted to <Yu Gothic UI> shipped with Windows 10
  - Perform more rigorous sentence splitting into multiple chunks, because JapanesePhoneticAnalyzer api does not accept a string w/ length greater than 100
