using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using MEC;
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

            var cdDoors = new List<Door>();

            Log.Debug("UseReadyCheck found class d spawn room");
            foreach (var door in cdSpawn.Doors)
            {
                AllowedInteractableDoors.Add(door);
                if (door.Rooms.Count == 1)
                {
                    Log.Debug("UseReadyCheck added class d door as lobby spawn point");
                    cdDoors.Add(door);

                    Timing.CallDelayed(
                        UnityEngine.Random.Range(23.5f, 40f),
                        () =>
                        {
                            door.IsOpen = true;
                        });


                    continue;
                }
                Log.Debug("UseReadyCheck door with more than one room, locking down other room");
                var otherRoom = door.Rooms.First(room => room.RoomName != MapGeneration.RoomName.LczClassDSpawn);
                ReadyCheckLockedDownRoom = otherRoom;
            }

            var doorsAndSpawn = cdDoors.Select(door =>
            {
                var farthestDoor = cdDoors.OrderByDescending(door2 => (door2.Position - door.Position).magnitude).First();
                var midpoint = (farthestDoor.Position + door.Position) / 2;
                Log.Debug($"{midpoint} -> {door.Position}");
                var dx = midpoint.x - door.Position.x;
                var dz = midpoint.z - door.Position.z;
                if (Math.Abs(dx) > Math.Abs(dz))
                {
                    return new Vector3(door.Position.x, door.Position.y, door.Position.z - Math.Sign(dz) * 2) + Vector3.up;
                }
                else
                {
                    return new Vector3(door.Position.x - Math.Sign(dx) * 2, door.Position.y, door.Position.z) + Vector3.up;
                }
            });

            LobbyAvailableSpawnPoints.AddRange(doorsAndSpawn);
        }

        public bool IsPlayerReady(Player player)
        {
            return player.CurrentRoom == ReadyCheckLockedDownRoom;
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

        public void OnPlayerSpawn(Player player)
        {
            var door = Door.GetClosest(player.Position, out var d);
            if (door != null && d < 3)
            {
                door.IsOpen = false;
            }

            var cdSpawn = Room.Get(RoomType.LczClassDSpawn);

            Log.Debug("UseReadyCheck found class d spawn room");
            foreach (var cd in cdSpawn.Doors)
            {
                AllowedInteractableDoors.Add(cd);
            }
        }
    }
}
