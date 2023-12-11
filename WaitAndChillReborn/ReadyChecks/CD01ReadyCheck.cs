using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WaitAndChillReborn
{
    using static API.API;

    internal class CD01ReadyCheck : IReadyCheckRoom
    {
        private Room ReadyCheckLockedDownRoom;

        public void SetUpRoom()
        {
            var cdSpawn = Room.Get(RoomType.LczClassDSpawn);
            Log.Debug("UseReadyCheck found class d spawn room");
            foreach (var door in cdSpawn.Doors)
            {
                AllowedInteractableDoors.Add(door);
                if (door.Rooms.Count == 1)
                {
                    LobbyAvailableSpawnPoints.Add(door.Position + Vector3.up);
                    Log.Debug("UseReadyCheck added class d door as lobby spawn point");
                    continue;
                }
                Log.Debug("UseReadyCheck door with more than one room, locking down other room");
                var otherRoom = door.Rooms.First(room => room.RoomName != MapGeneration.RoomName.LczClassDSpawn);
                ReadyCheckLockedDownRoom = otherRoom;
            }
        }

        public bool IsPlayerReady(Player player)
        {
            return player.CurrentRoom.RoomName != MapGeneration.RoomName.LczClassDSpawn;
        }

        public void OnRoundStart()
        {
            foreach (var pickup in ReadyCheckLockedDownRoom.Pickups)
            {
                pickup.Destroy();
            }
            var cdSpawn = Room.Get(RoomType.LczClassDSpawn);
            foreach (var door in cdSpawn.Doors)
            {
                door.IsOpen = false;
            }
            foreach (var door in ReadyCheckLockedDownRoom.Doors)
            {
                door.ChangeLock(DoorLockType.None);
                door.IsOpen = false;
            }
        }

        public string Instructions() => WaitAndChillReborn.Singleton.Translation.ClassDReadyInstructions;
    }
}
