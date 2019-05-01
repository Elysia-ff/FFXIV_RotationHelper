# FFXIV_RotationHelper
Helps to practice DPS rotation.

Preview : https://youtu.be/vLDzNC5nNPo

### Integrating ::

Plugins -> Plugin Listing -> Browse... -> Select FFXIV_RotationHelper.dll -> Add/Enabled Plugin


# Trouble Shooting
#### Could not load type 'Newtonsoft.Json'

Download Newtonsoft.Json.dll and place it into the folder that includes ACT.exe.

#### Not Found Player info

Disable FFXIV_ACT_Plugin.dll and Re-Enable it.

# TODO

1. ~~Remove AssemblyResolver. (Embed .dll in .exe)~~ (Newtonsoft.Json not embeded yet)

2. Update ActionTable.csv for patch 5.x. (have to support both 4.x and 5.x for a while)

3. ~~Remove RotationWindow from Alt+Tab menu.~~

4. Add feature to save ffxivrotations URL with memo.

5. Update pet data when pet has left the battlefield.

6. Find best practice to convert js to c#.

7. Load the log file without re-enable ffxiv_act_plugin.dll
