namespace WaitAndChillReborn.API
{
    using Exiled.API.Features;
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

        public static bool IsLobby => !Round.IsStarted && !Round.IsEnded;
    }
}
