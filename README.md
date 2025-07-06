# TripView
TripView is a program to visualize LeafSpy log information. You can see your route taken on a map and display charts on the status of the vehicle during the trip.
![Main Window nothing loaded](.github/images/startup.png "Main Window (No data loaded)")
To begin, you must load a LeafSpy Log file (these are located in <i>com.Turbo3.Leaf_Spy_Pro/files/LOG_FILES/</i> on your (android) device).
Select File -> Open to load the file into TripView. If you do not have a log file, but wish to see TripView in action, please see the sample log file in the examples folder.

# Privacy
TripView only needs internet access to download map tiles. Your data never leaves your local machine. 
If you wish, you can host your own instance of OpenStreetMaps and point TripViewer to it (check appsettings.json). 
How to setup [OpenStreetMaps](https://github.com/openstreetmap/openstreetmap-website/blob/master/INSTALL.md) is out of scope for this document.

# Supported Operating System
TripView is written using C# on the dotnet core 9 runtime. 
It uses WPF as its UI framework, as such at this time it will only run on Windows. 
Windows 11 is recommended, but it should work fine on windows 10. 
Windows 7 is unsupported but it might work without issue and untested.
Linux/Mac support will require a port to different UI framework (Roadmap).
Android/IOS support is not currently planned at this time.

## Features
* Route visualization
* Charts:
    * Battery Temperature x Time
    * Battery SOC x Time
    * Elevation x Time
    * Pack Volts/Amps x Time
    * Motor RPM / Torque x Time
    * Battery SOH x Time
    * 12v Battery Volts/Amps x Time
    * Speed x Time
    * Tire Pressure x Time
    * ...and more
* Log Event Inspection
* Route export to KML (for loading into Google Earth or other applications)
* Save Map and charts to files for report generation

## Quick Start
Once you have loaded a log file, you will see data populate the window as shown below:
![Main Window data loaded](.github/images/data-loaded.png "Main Window (trip loaded)")
The left panel is the individual data point (event) viewer. It will show you the details of each logged event. You can click on the chart or the map to select an event to view.
The main panel is the map, this will show you your route (if gps information is available). The map currently has three layers (The map itself, event locations and gps accuracy (hidden by default)).
The lower panel shows the currently active chart. The active chart type can be changed by going into the View menu and changing the chart type.
You may export images of your trip by right clicking on the chart or map and selecting save.

## Configuration
Please note currently you must configure the application to understand your leafspy log output, unless you are using my exact settings due to the way leafspy was designed.
To do so, you must update the appsettings.json file section "LeafspyImport". Please make sure these settings match what you have in leafspy otherwise your data will appear to be incorrect.
You may also customize the appearance somewhat by adjusting the settings in this file.

## Vehicle support
This project has only been tested with a Nissan LEAF 2020 SV using LeafSpy Pro on Android 15.
Older model nissan LEAFs or older versions of LeafSpy (older than 2023) may not work correctly, however, I would like to support them if at all possible. 
Please file an issue and include a log file (please replace GPS lat/long with the provided below values, replace with with VINREPLACED).
Change lat/long with "11 22.12345" , "-100 2.12345" if not zero. Please leave zeros. You will also need to include your LeafSpy settings.
Files with the following LeafSpy fields are needed: 
* Speed1
* Speed2
* Inverter 2 Temp
* Inverter 4 Temp
* ACComp01MPa

## Status
This project is in active development, as such features may change. Settings may change or be removed. Settings may not function the same way between versions. The UI will possibly update or be redesigned as time goes on.
Future development roadmap is in the Thoughts.txt file or on Github Issues.

## Contributing
PRs currently not accepted, but issues are welcome. PRs will be accepted in the future once the project is in a stable state.