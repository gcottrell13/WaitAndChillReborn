namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using static API.API;

    public abstract class BaseLobbyRoom
    {
        protected virtual RoomType RoomType => RoomType.Unknown;

        public Room ThisRoom => Room.Get(RoomType);

        public Door Gate => ThisRoom.Doors.FirstOrDefault(door => door.IsGate);

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
        /// Is the given player in this room?
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool IsPlayerInRoom(Player player) => player.CurrentRoom?.Type == RoomType;

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

        /// <summary>
        /// Instructions on how to become ready
        /// </summary>
        /// <returns></returns>
        public virtual string Instructions() => BaseToggleALTInstructions;

        public static string BaseToggleALTInstructions => WaitAndChillReborn.Singleton.Translation.ToggleNoClipInstructions;

        public virtual void OnToggleNoClip(Player player, bool enabled)
        {
            if (ReadyPlayers.Contains(player))
            {
                ReadyPlayers.Remove(player);
            }
            else
            {
                ReadyPlayers.Add(player);
            }
        }
    }
}
