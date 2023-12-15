﻿namespace WaitAndChillReborn.Configs
{
    using Exiled.API.Enums;
    using System.Collections.Generic;
    using System.ComponentModel;
    using PlayerRoles;
    using UnityEngine;
    using global::WaitAndChillReborn.API;

    public sealed class LobbyConfig
    {
        [Description("List of lobbys (rooms) where players can spawn: (list of all possible rooms can be found on plugin's GitHub)")]
        public List<string> LobbyRoom { get; private set; } = new()
        {
            "TOWER1",
            "TOWER2",
            "TOWER3",
            "TOWER4",
            "GATE_A",
            "GATE_B",
            "SHELTER",
            "GR18",
            "049",
            "079",
            "106",
            "173",
            "939",
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

        [Description("Use the Ready Check system to start the lobby. The Lobby room will become the room outside of CD Spawn.")]
        public bool UseReadyCheck { get; private set; } = true;

        [Description("Limit how many ragdolls can be spawned during the lobby for SCP 3114. -1 for unlimited.")]
        public int Ragdoll3114Limit { get; private set; } = -1;

        [Description("Should the lobby only allow one of each SCP Role at a time")]
        public bool UniqueSCPs { get; private set; } = true;

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
