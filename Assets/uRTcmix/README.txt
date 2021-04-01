This is the uRTcmix (RTcmix in Unity) package to allow the use of the
RTcmix digital signal-processing and sound synthesis language in the
Unity game engine.  See http://sites.music.columbia.edu/brad/uRTcmix/
for more information, examples and tutorials.

Drag the "RTcmixmain" game object (prefab) into your game object
list/hierarchy.  This has an associated script, "rtcmixmain.cs",
that contains all of the RTcmix API functions that can be used
in Unity.

--------------
NOTE:  You will probably get an error when you add the RTcmixmain
object to your object list/hierarchy:

   "Unsafe code requires the `unsafe' command line option to be specified.
   Enable "Allow 'unsafe' code" in Player Settings to fix this error."

Go to Edit->Project Settings->Player and then open the "Other Settings"
tab (if it isn't already).  Scroll down and check the box next to
the "Allow 'unsafe' Code" setting.
--------------

The "beep" game object (prefab) will sound a G-above-middle-C for 8.7
seconds when the Unity engine is started (if "beep" is instantiated).  It
uses the associated simple "beep.cs" script to do this.

The "uRTcmixScriptTemplate.cs" script contains all of the uRTcmix
functions that may be called from Unity. Mmost are commented-out; there
as examples of how they may be used.

Brad Garton
October, 2019
