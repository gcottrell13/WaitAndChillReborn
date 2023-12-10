namespace WaitAndChillReborn.API
{
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using MEC;
    using System.Collections.Generic;
    using UnityEngine;

    public static class API
    {
        public static Vector3 LobbyChoosedSpawnPoint;

        public static List<Vector3> LobbyAvailableSpawnPoints = new();

        public static CoroutineHandle LobbyTimer;

        public static CoroutineHandle ReadyCheckHandle;

        // should be null if not using ReadyCheck
        public static Room ReadyCheckLockedDownRoom;

        public static int ReadyPlayers;

        // For ReadyCheck to not flicker the message between waiting and ready
        public static HashSet<uint> SpawnedInPlayers = new();

        public static bool IsLobby => !Round.IsStarted && !Round.IsEnded;

        public static HashSet<Door> AllowedInteractableDoors = new();


        public static void AddSpawnedPlayer(Player player) => SpawnedInPlayers.Add(player.NetId);
        public static bool HasSpawnedPlayer(Player player) => SpawnedInPlayers.Contains(player.NetId);
        public static bool RemoveSpawnedPlayer(Player player) => SpawnedInPlayers.Remove(player.NetId);
    }
}
