HDR
===

This is a windows console application with an embedded web server used to server an HLS stream from the <a href="https://github.com/computmaxer/roku-hdhomerun">Roku-HDHomerun</a> channel. 

Dependencies
===
<ul>
<li>
<a href="http://www.silicondust.com/support/hdhomerun/downloads/">hdhomerun_config</a> SiliconDust utility for finding devices on network</li>
<li>ffmpeg</li>
</ul>

Parameters
===
<ul>
<li>path of the hdhomerun_config.exe</li>
<li>path of ffmpeg.exe</li>
<li>web port number</li>
</ul>


Usage
===
<ul>
<li>Unzip "realease.zip" to the directory of you choice. Make sure there is enough space to cache the HLS stream. (~500mb free space)</li>
<li>Edit "start.bat" and change parameters accordingly</li>
<li>Run "start.bat"</li>
<li>Point the <a href="https://github.com/computmaxer/roku-hdhomerun">Roku-HDHomerun</a> channel to the HDR host</li>
</ul>

Logos
===
The channel logo names are automatically generated according to the channel name. If the channel name is "FOX" then the application will look for "FOX.png" in the "/static/logos" directory.

Favorites
===
Channel favorites are enabled in the HDHomerunPlus interface website.





