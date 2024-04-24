---------------------
DisableScript V 1.1
by Standardverlag
Readme.txt
---------------------

Thank you for purchasing DisableScript, a simple Unity3D tool which will let you 
disable or enable scripts to be compiled or not, from inside the editor.

---------------------
HOW TO INSTALL:
- Download from the asset store
- Click "Import"
- When opening a new project: just select "DisableScript" from your package list

---------------------
HOW TO USE:
- Select a script in the project window
- Right click and select "Compile Script Settings/Disable Script" to turn the script off
- Right click and select "Compile Script Settings/Enable Script" to turn the script back on

TECHNICAL INFORMATION:
DisableScript does add a new first and last line to your script in order to disable it.
For CS and JS files it does use #if false and for BOO it does use comments.
If you manually do change these lines, you can't automatically enable the script.
Just delete the first and last line in these cases.
  
---------------------
Version History:
 
 Version 1.1: - Added ability to select and enable/disable multiple scripts
 
 Version 1.0: - Initial  

---------------------
Support:

http://unity.standardverlag.de
Mail: unity@standardverlag.de

Happy scripting! 
