namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Toys;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using static API.API;

    public abstract class BaseLobbyRoom
    {
        protected virtual RoomType RoomType => RoomType.Unknown;

        public Room ThisRoom => Room.Get(RoomType);

        /// <summary>
        /// Gets "the" gate to/inside this room.
        /// </summary>
        public Door Gate => ThisRoom.Doors.FirstOrDefault(door => door.IsGate);

        /// <summary>
        /// Gets all gates in this room.
        /// </summary>
        public List<Door> Gates => ThisRoom.Doors.Where(door => door.IsGate).ToList();

        /// <summary>
        /// Gets "the" entrance door. Use only if you're sure there is only one (or you don't care).
        /// </summary>
        public Door Entrance => ThisRoom.Doors.FirstOrDefault(door => door.Rooms.Count > 1);

        /// <summary>
        /// Gets all entrance doors.
        /// </summary>
        public List<Door> Entrances => ThisRoom.Doors.Where(door => door.Rooms.Count > 1).ToList();

        public List<Vector3> SpawnPoints { get; } = new();

        /// <summary>
        /// Called before everyone spawns in.
        /// Sets interactable doors to all doors interior to the room.
        /// </summary>
        public virtual void SetUpRoom()
        {
            if (ThisRoom != null)
            {
                foreach (Door door in ThisRoom.Doors)
                {
                    if (door.Rooms.Count == 1 && !door.IsElevator) AllowedInteractableDoors.Add(door);
                    if (door.IsGate) door.IsOpen = true;
                }
            }

            SetupSpawnPoints();
        }

        /// <summary>
        /// Adds spawn points to this room
        /// </summary>
        public abstract void SetupSpawnPoints();

        /// <summary>
        /// When a player spawns in
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnPlayerSpawn(Player player)
        {
            float random = 0.15f;
            player.Position = SpawnPoints.RandomItem() 
                + Vector3.up
                + Vector3.left * Random.Range(-random, random)
                + Vector3.right * Random.Range(-random, random);
        }

        /// <summary>
        /// Clean up any stuff done to the room.
        /// Called when everyone is made Spectator before the round starts.
        /// </summary>
        public virtual void OnRoundPrepare()
        {
            if (ThisRoom != null)
            {
                foreach (Door door in ThisRoom.Doors)
                {
                    if (!door.IsElevator)
                        door.IsOpen = false;
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------

    }
}
