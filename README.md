# WaitAndChillReborn

Remade version of this plugin: https://github.com/TruthfullyHonest/WaitAndChill

## Features
- Two message lines which are customizable (You can make it a Hint displayed on each Player or a Broadcast, it also works with Unity's Rich Text tags, you can also disable the message and just let users do what they want)
- You can adjust the vertical position of the message lines when they are displayed! (Hints only)
- Choice of randomly setting a role for users to be when they spawn
 
 ## Note
- %player will return one of two options for messages ((0 or x players have connected) or (1 player has connected))
- %seconds will return one of four options for messages (The server is paused, The round has started, 1 second remains, x seconds remain)
 
# Config
```yml
wait_and_chill_reborn:
  is_enabled: true
  # Determines if any kind of message at all will be displayed
  display_wait_message: true
  # List of roles that players can spawn:
  roles_to_choose:
  - Tutorial
  # Allow dealing damage to other players, while in lobby:
  alow_damage: false
  # Disallow players triggering SCP-096 and stopping from moving SCP-173, while in lobby:
  turned_players: true
  # Give players an effect of SCP-207, while in lobby: (set 0 to disable)
  cola_multiplier: 4
  # Use hints instead of broadcasts for text stuff:
  use_hints: true
  # Determines the position of the Hint on the users screen (32 = Top, 0 = Middle, -15 = Below)
  hint_vert_pos: 25
  # Text traslations:
  top_message: <size=40><color=yellow><b>The game will be starting soon, %seconds</b></color></size>
  bottom_message: <size=30><i>%players</i></size>
  server_is_paused: Server is paused
  round_is_being_started: Round is being started
  one_second_remain: second remain
  x_seconds_remains: seconds remains
  one_player_connected: player has connected
  x_players_connected: players have connected
```
