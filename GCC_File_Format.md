# GCC file format: #

There is one header with 26 bytes
and then any number of records each with 10 bytes
(except waypoints which have additional 2 bytes for each character of the name and special records >31 which have an unicode string appended).

## Header (26 bytes): ##
  * 3 characters (bytes): "GCC"
  * 1 byte: version number currently 1 (binary)
  * 6 bytes: start time in format: year-2000, month, day, hour, minute, second
  * 8 bytes double: reference latitude (start point y)
  * 8 bytes double: reference longitude (start point x)

## normal Records (5 short ints): ##
  * x in m from reference point
  * y in m from reference point
  * z in m from sea level
  * s speed in 0.1 km/h
  * t time in seconds from start (unsigned short int)

## special Records: ##

**origin shift (0):**
  * (x) x shift in m
  * (y) y shift in m
  * (z) 0 (origin shift)
  * (s) -1 (special record)
  * (t) 0xFFFF (special record)


**battery (1):**
  * (x) battery in %
  * (y)
  * (z) 1 (battery)
  * (s) -1 (special record)
  * (t) 0xFFFF (special record)


**GPS option (2):**
  * (x) gps\_poll\_sec
  * (y)
  * (z) 2 (gps option)
  * (s) -1 (special record)
  * (t) 0xFFFF (special record)


**waypoint (3):**
  * (x) number of characters
  * (y)
  * (z) 3 (waypoint)
  * (s) -1 (special record)
  * (t) 0xFFFF (special record)
  * waypoint name (each character encoded as 2-bytes)


**heart rate (4):**
  * (x) heart rate in bpm
  * (y)
  * (z) 4 (heart rate)
  * (s) -1 (special record)
  * (t) 0xFFFF (special record)


special record numbers > 31 have unicode string appended

**name (32):**
  * (x) number of characters
  * (y)
  * (z) 32 (name)
  * (s) -1 (special record)
  * (t) 0xFFFF (special record)
  * track name (unicode string preceeded by length)


**description (33):**
  * (x) number of characters
  * (y)
  * (z) 33 (description)
  * (s) -1 (special record)
  * (t) 0xFFFF (special record)
  * track description (unicode string preceeded by length)