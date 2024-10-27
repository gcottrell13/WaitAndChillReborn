# WaitAndChillReborn

Remake of this plugin: https://github.com/Michal78900/WaitAndChillReborn

## ⚠ Archived ⚠
- Please use the WaitAndChillReborn.dll from: https://github.com/gcottrell13/ScpSLPlugins

## Features
- Two message lines which are customizable (You can make it a Hint displayed on each Player or a Broadcast, it also works with Unity's Rich Text tags, you can also disable the message and just let users do what they want)
- You can adjust the vertical position of the message lines when they are displayed! (Hints only)
- Choice of randomly setting a role for users to be when they spawn
- Choice of randomly setting a room for users to be spawnned in
- Giving items to a player, while in lobby
- Ready Check system using the NoClip key to toggle readiness. The game will not start unless the required number of players are ready.
	- Useful for a private group
	- Required player ready % is configurable
	- If used on a public server, % should be lower than 100 (maybe 50 to 80)
 
 ## Note
- **{player}** will return one of two options for messages ((0 or x players have connected) or (1 player has connected))
- **{seconds}** will return one of four options for messages (The server is paused, The round has started, 1 second remains, x seconds remain)
 
# List of all possible lobby rooms
- TOWER1
- WC
- GR18
- GATE_A
- GATE_B
- DRIVEWAY  (the area under the bridge next to gate A elevator)
- 079
- 096
- 106
- 173
- 939
