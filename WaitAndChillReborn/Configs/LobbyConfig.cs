namespace WaitAndChillReborn.Configs
{
    using Exiled.API.Enums;
    using System.Collections.Generic;
    using System.ComponentModel;
    using PlayerRoles;
    using UnityEngine;

    public sealed class LobbyConfig
    {
        [Description($"""
            List of lobby rooms where players can spawn.
            Options:
                - '{CD01Room.Name}'
                - '{Lcz173Room.Name}'
                - '{GateARoom.Name}'
                - '{GateBRoom.Name}'
                - '{Scp049Room.Name}'
                - '{Scp079Room.Name}'
                - '{Scp106Room.Name}'
                - '{Scp939Room.Name}'
                - '{DrivewayRoom.Name}'
                - '{TowerRoom.Name}'
                - '{WcRoom.Name}'
                - '{GlassRoom.Name}'
                - '{EscapeRoom.Name}'
                - '{PC15Room.Name}'
            """)]
        public List<string> LobbyRoom { get; private set; } = new()
        {
            CD01Room.Name,
            Lcz173Room.Name,
            GateARoom.Name,
            GateBRoom.Name,
            Scp049Room.Name,
            Scp079Room.Name,
            Scp106Room.Name,
            Scp939Room.Name,
            DrivewayRoom.Name,
            TowerRoom.Name,
            WcRoom.Name,
            GlassRoom.Name,
            EscapeRoom.Name,
            PC15Room.Name,
        };

        [Description("List of static positions where player can spawn:")]
        public List<Vector3> StaticLobbyPositions { get; private set; } = new()
        {
            new Vector3(-1f, -1f, -1f),
        };

        [Description("Whether plugin should use all of lobby rooms on the list instead only one. Player when they join will be teleported to the random lobby room.")]
        public bool MultipleRooms { get; private set; } = false;

        [Description("The time (in seconds) between player joining on the server and him changing role while in lobby (change this number if some players aren't spawned / are spawned as a None class.")]
        public float SpawnDelay { get; private set; } = 0.25f;

        [Description("List of roles which players can spawn as:")]
        public List<RoleTypeId> RolesToChoose { get; private set; } = new()
        {
            RoleTypeId.Tutorial,
        };

        [Description("Use the Ready Check system to start the lobby. Useful for groups of friends.")]
        public bool UseReadyCheck { get; private set; } = true;

        [Description("""
            % of players that need to be ready in order to start the round.
            Suggested values:
            80 - 80% of players must be ready
            100 - Everyone must be ready
            """)]
        public int ReadyCheckPercent { get; private set; } = 0;

        [Description("Limit how many ragdolls can be spawned during the lobby for SCP 3114. -1 for unlimited.")]
        public int Ragdoll3114Limit { get; private set; } = -1;

        [Description("Should the lobby only allow one of each SCP Role at a time")]
        public bool UniqueScps { get; private set; } = true;

        [Description("Should everyone be added to global chat during the lobby")]
        public bool LobbyGlobalChat { get; private set; } = true;

        [Description("List of items given to a player while in lobby: (supports CustomItems)")]
        public List<string> Inventory { get; private set; } = new()
        {
            ItemType.Coin.ToString(),
        };

        [Description("List of ammo given to a player while in lobby:")]
        public Dictionary<AmmoType, ushort> Ammo { get; private set; } = new()
        {
            { AmmoType.Nato556, 0 },
            { AmmoType.Nato762, 0 },
            { AmmoType.Nato9, 0 },
            { AmmoType.Ammo12Gauge, 0 },
            { AmmoType.Ammo44Cal, 0 },
        };

        [Description("Whether players should NOT be able to trigger SCP-096 and stop moving SCP-173, while in lobby.")]
        public bool TurnedPlayers { get; private set; } = true;

        [Description("Effects that will be enabled, while in lobby. The number is the effect intensity.")]
        public Dictionary<EffectType, byte> LobbyEffects { get; private set; } = new()
        {
            { EffectType.MovementBoost, 50 },
        };
    }
}
