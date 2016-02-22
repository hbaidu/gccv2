# GPS Cycle Computer #

---

**Features**
  * GPS tracking application for Windows Mobile (WM5.0-WM6.5) devices
  * Tested on HTC Diamond, Touch Diamond 2, Samsung Omnia2, Asus P320...
  * Logs GPS data into binary files with `.gcc` extension.
  * `*.gcc` files can be loaded back for viewing, or exported into `.kml` or `.gpx` format, to view in e.g. GoogleEarth.
  * Direct download of Open Street Map tiles, or offline download to avoid any data connection
  * Map display with automatic zoom of maps, if current zoom level is not downloaded
  * Waypoint support (GPX and GCC file format)
  * Cycle computer data screen (main display)
  * Direct input of destination position for e. g. geocaching
  * To save battery power, GPS can be switched on/off at pre-defined intervals (5 sec ... 10 min)
  * Does not required any other GPS software to be installed, i.e. works directly with Windows GPS driver.
  * Automatic power off, on low battery
  * Live loggin option, using www.crossingways.com server

**Main display**
  * trip distance, distance to start/end of track
  * trip time (including or excluding "stop time", i.e. when speed is zero)
  * current / average / max speed
  * current position relative to starting point (as x/y)
  * current height
  * battery usage
  * display units are miles / mph, km / km/h or nautical miles / knots

**Map display**
  * Current position
  * Track to follow
  * Current tracklog
  * Waypoints
  * Distance between current potision and track to follow

**Graph display**
  * Altitude or speed graph of tracklog or track to follow

---

## Installation ##

Copy the CAB file into your Windows Mobile device and run to install. In most cases it's no problem to install on Storage Card. If you have problems try to install into Internal Storage or main memory.

The output `*.gcc` files are created in the "input/output files location" (default: same folder, as the application).
The output `*.kml/*.gpx` files also created in the same folder. The `*.kml/*.gpx`
file names match the input `*.gcc` name.

---


## Controls on the "Main" tab ##

Start / Stop buttons - to start/stop the log. Log file is automatically created,

with name "year"+"month"+"day"+hour"+"min"+"sec"+ .gcc in the application directory.



BkLight Off - switch off backlight (and continue logging). Do not turn off the

device, as this application will be switched off as well. I.e. on Diamond, do not

press hardware "power" button to switch it off (this is set by default), but

click "BkLight Off" instead. To switch backlight on again, press harware "power".



GPS status display (top line of the window) :

- S - number after "S" is number of satellites found

- Snr - number after SNR is max Signal-To-Noise ratio, i.e. the signal quality. The higher, the better.

- T - number after T is difference between current UTC time of your phone and UTC time of GPS sample

> (from satellite). Might be negative - then you phone time is behind. What is important, is when the

> number is increasing, then the GPS cannot get hold on a fresh sample (i.e. searching)

- Dh - number after Dh is "DHOP" == Horizontal Dilution Of Precision. 1 is very good lat/long precision,

> 50 is very bad lat/long precision.

- Green box at the top right corner - sample OK and recorded.

- Red box at the top right corner - sample is bad/old/invalid, etc, GPS still searching.

---


## Options ##

For some Options you must stop logging and switch off and on GPS to have an effect.



- GPS activity : choose how often you would like to run GPS (always on, or

> switch it on/off at given intervals)



- Units : select units for the display and graphs.



- Exclude stop time : if activate, the points with zero speed are removed from

> "trip time" and "average speed" calculations. This is useful to see the "net time"

> when you e.g. cycling and make breaks during the trip, without the need to switch off the logger.



- Stop GPS if battery < 20% : as a safety feature, to avoid completely draining

> the battery.

---


## Loading the **.gcc files back for viewing ##**

Click "File/Export" to load an existing file.


"Units" and "Exclude stop time" can be changed, display will be automatically updated.


Click "Save .kml" - to save into KML the currently loaded file or the current

log (after you pressed Stop). The .kml file has the same name as the corresponding gcc file.


Click "Save .gpx" - to save into GPX the currently loaded file or the current

log (after you pressed Stop). The .gpx file has the same name as the corresponding gcc file.


KML files can be view with Google Earth (very nice!), and you might view them

with the Google Maps Mobile according to Google help (the one installed on Diamond?)

- but I was not able to do make it work on Diamond.

---


## Track to Follow ##

You can load a .gpx .kml or .gcc file as "Track2Follow". Then tis track is displayed on map in different color and you can follow it. A navigation arrow and navigation commands in text and voice guides you along the route loades as Track2Follow.

---


## Custom button skins and custom back / fore color ##
Besides the two Day and Night Schemes, you can set your own background / text color and images for all buttons. All what is required is

to edit .jpeg files which are supplied with the source code (look into GpsCycleComputerSource.zip)

and copy the new images into the folder with GpsCycleComputer.exe on your

WinMobile, then re-start GpsCycleComputer.  The new images will be loaded at the startup.

If you want to change just a single button, just copy the files for that button, all files are not required.



For example, if you want the change “Graph” button for menu, edit files “graph.jpg” (normal)

and “graph\_p.jpg” (pressed - there are two images, as button has two states). Note you must

not change the image sizes!



The background color and foreground color (text color) can be changed in Options - Main screen - Select fore/back/mapLabel-color.


---


## Source code ##

A complete source code (in C#) is provided. For GCC file format see file Form1.cs, function

"buttonLoad\_Click" which loads a gcc file. Basically after a header with general data,

the data is written as 5 short int (short int = 16 bits = 2 bytes) which are :

x, y, z (in metres, relative to starting point), speed (in kmh\*10) and time (sec, relative

to start). Also there are a few special records (also as 5 short ints) to store some

control info, like battery status.


Feel free to change anything you like, but please send me you comments/bug fixes.

---


## Documentation and Support ##
Documentation is here:
http://gccv2.googlecode.com/files/Readme.htm

Support is provided in the project Forum:
http://forum.xda-developers.com/showthread.php?t=424423

