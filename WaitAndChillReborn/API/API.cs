namespace WaitAndChillReborn.API
{
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Pickups;
    using global::WaitAndChillReborn.Configs;
    using MEC;
    using PlayerRoles;
    using System.Collections.Generic;
    using System.Linq;

    public static class API
    {
        public static List<BaseReadyCheckRoom> LobbyAvailableRooms = new();

        public static CoroutineHandle LobbyTimer;

        public static bool IsLobby => !Round.IsStarted && !Round.IsEnded;

        public static HashSet<Door> AllowedInteractableDoors = new();

        public static readonly HashSet<Pickup> LockedPickups = new();

        public static int spawnedRagdollsFor3114 = 0;

        #region Ready Check

        public static CoroutineHandle ReadyCheckHandle;

        public static bool IsReadyToStartGame;

        public static HashSet<Player> ReadyPlayers = new();

        // For ReadyCheck to not flicker the message between waiting and ready
        public static HashSet<uint> SpawnedInPlayers = new();

        public static BaseReadyCheckRoom FindPlayerRoom(Player player)
        {
            foreach (var room in LobbyAvailableRooms)
            {
                if (room.IsPlayerInRoom(player)) return room;
            }
            return catchall;
        }

        public static BaseReadyCheckRoom catchall = new CatchallRoom();

        public static void AddSpawnedPlayer(Player player) => SpawnedInPlayers.Add(player.NetId);
        public static bool HasSpawnedPlayer(Player player) => SpawnedInPlayers.Contains(player.NetId);
        public static bool RemoveSpawnedPlayer(Player player) => SpawnedInPlayers.Remove(player.NetId);

        public static List<Player> validPlayers => Player.List.Where(p => p.Role.Type != RoleTypeId.Overwatch).ToList();

        #endregion


        private static ItemPool<RoleTypeId> _roles;
        public static ItemPool<RoleTypeId> RolesToChoose
        {
            get
            {
                if (_roles == null)
                {
                    _roles = Config.RolesToChoose.ToPool();
                    _roles.ShuffleList();
                }
                return _roles;
            }
        }

        public static void Reset()
        {
            LobbyAvailableRooms.Clear();
            AllowedInteractableDoors.Clear();
            SpawnedInPlayers.Clear();
            catchall = new CatchallRoom();
            IsReadyToStartGame = false;
            ReadyPlayers = new();
            Round.KillsByScp = 0;
            spawnedRagdollsFor3114 = 0;
            _roles = null;
        }

        public static readonly Translation Translation = WaitAndChillReborn.Singleton.Translation;
        public static readonly LobbyConfig Config = WaitAndChillReborn.Singleton.Config.LobbyConfig;
    }
}
