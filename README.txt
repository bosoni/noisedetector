noisedetector
(c) mjt 2022

MIT license.

Compiled .exe can be found at 
./bin/x64/Debug/noisedetector.exe


Double click that on windows.

On linux, install mono, then
$ mono noisedetector.exe


Two windows opens (one 1x1 size, other where one can see some infos).

You will see numbers (audio data's average volume from mic). Make some noise and then average value goes up. Take some volume value and use it in  noise_config.txt  as volume peak value. Change program name and parameters etc. Then close noisedetector and start it again.



Uses FNA library ( https://fna-xna.github.io/ )

