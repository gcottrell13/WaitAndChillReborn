﻿namespace WaitAndChillReborn
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Player;
    using GameCore;
    using global::WaitAndChillReborn.API;
    using MEC;
    using PlayerRoles;
    using static API.API;

    internal static class PlayerEventHandlers
    {

        public static void OnCoinFlip(FlippingCoinEventArgs @event)
        {
            if (!IsLobby)
                return;
            if (ReadyCheckHandle.IsRunning == false && Config.UseReadyCheck)
            {
                ReadyCheckHandle = Timing.RunCoroutine(ReadyCheckHandler.ReadyCheck());
            }
        }

        public static void OnNoclip(TogglingNoClipEventArgs ev)
        {
            if (!IsLobby)
                return;
            if (!Config.UseReadyCheck)
                return;
            FindPlayerRoom(ev.Player).OnToggleNoClip(ev.Player, ev.IsEnabled);
        }

        public static void OnPlayerLeft(LeftEventArgs @event)
        {
            RemoveSpawnedPlayer(@event.Player);
        }

        public static void OnSpawnRagdoll(SpawningRagdollEventArgs ev)
        {
            if (!IsLobby)
                return;
            if (!Player.List.Any(player => player.Role.Type == PlayerRoles.RoleTypeId.Scp3114))
            {
                ev.IsAllowed = false;
            }
            else
            {
                if (Config.Ragdoll3114Limit == -1 || spawnedRagdollsFor3114 < Config.Ragdoll3114Limit)
                {
                    spawnedRagdollsFor3114++;
                }
                else
                {
                    ev.IsAllowed = false;
                }
            }
        }

        public static void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!IsLobby)
                return;
            if (AllowedInteractableDoors.Contains(ev.Door) == false)
            {
                ev.IsAllowed = false;
            }
            else if (!ev.Door.IsOpen && ev.Door.KeycardPermissions != KeycardPermissions.None)
            {
                ev.Door.IsOpen = true;
            }
        }

        public static void OnVerified(VerifiedEventArgs ev)
        {
            if (!IsLobby)
                return;

            if (RoundStart.singleton.NetworkTimer > 1 || RoundStart.singleton.NetworkTimer == -2)
            {
                Timing.CallDelayed(
                    Config.SpawnDelay,
                    () =>
                    {
                        ev.Player.Role.Set(RolesToChoose.GetNext());

                        if (Config.TurnedPlayers)
                        {
                            Scp096Role.TurnedPlayers.Add(ev.Player);
                            Scp173Role.TurnedPlayers.Add(ev.Player);
                        }
                    });
            }
        }

        public static void OnSpawned(SpawnedEventArgs ev)
        {
            if (!IsLobby)
                return;

            if (RoundStart.singleton.NetworkTimer <= 1 && RoundStart.singleton.NetworkTimer != -2)
                return;

            if (LobbyAvailableRooms.Count == 0)
                return;

            _givePlayerEffectsAndItems(ev.Player, Config.SpawnDelay);
            Exiled.CustomItems.API.Extensions.ResetInventory(ev.Player, Config.Inventory);
            LobbyAvailableRooms.RandomItem().OnPlayerSpawn(ev.Player);
        }

        public static void OnDying(DyingEventArgs ev)
        {
            if (IsLobby)
            {
                ev.Player.ClearInventory();
            }
        }

        public static void OnDied(DiedEventArgs ev)
        {
            if (!IsLobby || (RoundStart.singleton.NetworkTimer <= 1 && RoundStart.singleton.NetworkTimer != -2))
                return;

            if (LobbyAvailableRooms.Count == 0)
                return;

            Timing.CallDelayed(Config.SpawnDelay, () =>
            {
                ItemPool<RoleTypeId> rolePool = RolesToChoose;

                if (Config.UniqueScps)
                {
                    RoleTypeId role = rolePool.GetNext(role =>
                    {
                        if (PlayerRolesUtils.GetTeam(role) == Team.SCPs && Player.List.Any(player => player.Role.Type == role))
                        {
                            return false;
                        }
                        return true;
                    });
                    ev.Player.Role.Set(role);
                }
                else
                {
                    RoleTypeId role = rolePool.GetNext();
                    ev.Player.Role.Set(role);
                }
            });
        }

        public static void _givePlayerEffectsAndItems(Player player, float delay)
        {
            Timing.CallDelayed(
                delay,
                () =>
                {
                    foreach (KeyValuePair<EffectType, byte> effect in Config.LobbyEffects)
                    {
                        player.EnableEffect(effect.Key);
                        player.ChangeEffectIntensity(effect.Key, effect.Value);
                    }

                    if (Config.LobbyGlobalChat)
                        player.VoiceChannel = VoiceChat.VoiceChatChannel.Spectator;

                    foreach (KeyValuePair<AmmoType, ushort> ammo in Config.Ammo)
                        player.Ammo[ammo.Key.GetItemType()] = ammo.Value;
                });
        }
    }
}
