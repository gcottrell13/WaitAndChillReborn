using Exiled.API.Features;
using Exiled.API.Features.Doors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WaitAndChillReborn
{
    using static API.API;

    internal class Lcz173ReadyCheck : IReadyCheckRoom
    {
        private Room thisroom;
        private Door gate;
        private Door otherdoor;

        private Vector3 behindHere;

        public void SetUpRoom()
        {
            thisroom = Room.Get(Exiled.API.Enums.RoomType.Lcz173);

            Log.Debug($"LCZ 173 room at: {thisroom.Position}");

            foreach (var door in thisroom.Doors)
            {
                if (door.Rooms.Any(room => room.RoomName != MapGeneration.RoomName.Lcz173)) continue; // this is a door to the outside, we don't want to go out there!
                if (door.IsGate)
                {
                    // we don't want to spawn by the gate, but let's open it
                    door.IsOpen = true;
                    gate = door;
                    continue;
                }
                LobbyAvailableSpawnPoints.Add(door.Position + Vector3.up);
                AllowedInteractableDoors.Add(door); // this door is inside this room, this door is OK to open!
                otherdoor = door;
            }

            var dx = otherdoor.Position.x - gate.Position.x;
            var dz = otherdoor.Position.z - gate.Position.z;
            if (Math.Abs(dx) < Math.Abs(dz))
            {
                behindHere = new Vector3(0, 0, -Math.Sign(dz));
            }
            else
            {
                behindHere = new Vector3(-Math.Sign(dx), 0, 0);
            }
        }

        public bool IsPlayerReady(Player player)
        {
            var playerDir = player.Position - gate.Position;
            return playerDir.x * behindHere.x + playerDir.z * behindHere.z > 0;
        }

        public void OnRoundStart()
        {
            return;
        }

        public string Instructions() => WaitAndChillReborn.Singleton.Translation.Lcz173ReadyInstructions;

        public void OnPlayerSpawn(Player player)
        {
        }
    }
}
