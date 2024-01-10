namespace WaitAndChillReborn.API
{
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Pickups;
    using MEC;
    using System.Collections.Generic;
    using UnityEngine;

    public static class API
    {
        public static Vector3 LobbyChoosedSpawnPoint;

        public static ItemPool<Vector3> LobbyAvailableSpawnPoints = new();

        public static bool SpawnedPinataThisRound = false;

        public static Dictionary<Player, bool> GivenCandyToPlayer = new ();

        public static CoroutineHandle LobbyTimer;

        public static CoroutineHandle ReadyCheckHandle;

        public static int ReadyPlayers;

        // For ReadyCheck to not flicker the message between waiting and ready
        public static HashSet<uint> SpawnedInPlayers = new();

        public static bool IsLobby => !Round.IsStarted && !Round.IsEnded;

        public static HashSet<Door> AllowedInteractableDoors = new();

        public static IReadyCheckRoom ReadyCheckRoom;


        public static void AddSpawnedPlayer(Player player) => SpawnedInPlayers.Add(player.NetId);
        public static bool HasSpawnedPlayer(Player player) => SpawnedInPlayers.Contains(player.NetId);
        public static bool RemoveSpawnedPlayer(Player player) => SpawnedInPlayers.Remove(player.NetId);

        public static void SpawnCoinInFrontOfDoor(Door door)
        {
            var pos = door.Position + Vector3.up + door.Rotation * Vector3.up;
            var pickup = Pickup.CreateAndSpawn(ItemType.Coin, pos, door.Rotation);
            pickup.Transform.localScale = 4 * Vector3.one;
        }
    }
}
