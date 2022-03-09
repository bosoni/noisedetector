noisedetector
(c) mjt 2022

MIT license.

Compiled .exe can be found at 
./bin/x64/Debug/noisedetector.exe

Double click that on windows.
On linux, install mono, then
$ mono noisedetector.exe

2 windows opens (minimize/hide black window, it isnt needed).

You will see numbers (idle mic datas volume), make some noise and then avg value goes up. Take some volume value and use it in  noise_config.txt  as volume peak value. Change program name and parameters etc. Then close those windows and run noisedetector.exe again.


Why?
Well, one can start ffmpeg video or take image on every loud noise. Or start some audio recording software when sleeping and noisedetector starts when snoring (btw, this captures mic so might need other audio source for that).


Uses FNA library ( https://fna-xna.github.io/ )

