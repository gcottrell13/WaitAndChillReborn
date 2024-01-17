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
        public static List<BaseLobbyRoom> LobbyAvailableRooms = new();

        public static CoroutineHandle LobbyTimer;

        public static bool IsLobby => !Round.IsStarted && !Round.IsEnded;

        public static HashSet<Door> AllowedInteractableDoors = new();

        public static readonly HashSet<Pickup> LockedPickups = new();

        public static int spawnedRagdollsFor3114 = 0;

        #region Ready Check

        public static CoroutineHandle ReadyCheckHandle;

        public static bool IsReadyToStartGame;

        public static HashSet<Player> ReadyPlayers = new();

        // spectator is included here for all the people that are momentarily dead
        public static List<Player> validPlayers => Player.List.Where(p => p.IsAlive || p.Role == RoleTypeId.Spectator).ToList();

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
            IsReadyToStartGame = false;
            ReadyPlayers = new();
            Round.KillsByScp = 0;
            spawnedRagdollsFor3114 = 0;
            _roles = null;
        }

        public static Translation Translation => WaitAndChillReborn.Singleton.Translation;
        public static LobbyConfig Config => WaitAndChillReborn.Singleton.Config.LobbyConfig;
    }
}
