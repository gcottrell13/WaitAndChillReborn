namespace WaitAndChillReborn
{
    using System.Collections.Generic;
    using Configs;
    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.EventArgs.Player;
    using GameCore;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using MEC;
    using Mirror;
    using UnityEngine;
    using static API.API;
    using Log = Exiled.API.Features.Log;
    using Object = UnityEngine.Object;
    using PlayerEvent = Exiled.Events.Handlers.Player;
    using ServerEvent = Exiled.Events.Handlers.Server;
    using MapEvent = Exiled.Events.Handlers.Map;
    using Player = Exiled.API.Features.Player;
    using Scp106Event = Exiled.Events.Handlers.Scp106;
    using Exiled.Events.EventArgs.Server;
    using Exiled.API.Features;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using global::WaitAndChillReborn.API;
    using PlayerRoles;
    using Exiled.API.Features.Roles;

    internal static class EventHandlers
    {

        private static ItemPool<RoleTypeId> _roles;
        internal static ItemPool<RoleTypeId> RolesToChoose
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

        internal static void RegisterEvents()
        {
            ServerEvent.WaitingForPlayers += OnWaitingForPlayers;

            PlayerEvent.Verified += OnVerified;
            PlayerEvent.Left += OnPlayerLeft;
            PlayerEvent.Spawned += OnSpawned;
            PlayerEvent.Dying += OnDying;
            PlayerEvent.Died += OnDied;
            PlayerEvent.Hurting += OnHurt;

            MapEvent.PlacingBlood += OnDeniableEvent;
            PlayerEvent.SpawningRagdoll += OnSpawnRagdoll;
            PlayerEvent.IntercomSpeaking += OnDeniableEvent;
            PlayerEvent.DroppingItem += OnDeniableEvent;
            PlayerEvent.DroppingAmmo += OnDeniableEvent;
            PlayerEvent.InteractingDoor += OnInteractingDoor;
            PlayerEvent.InteractingElevator += OnDeniableEvent;
            PlayerEvent.InteractingLocker += OnDeniableEvent;
            PlayerEvent.FlippingCoin += OnCoinFlip;
            MapEvent.ChangingIntoGrenade += OnDeniableEvent;

            // Scp106Event.CreatingPortal += OnDeniableEvent;
            Scp106Event.Teleporting += OnDeniableEvent;

            ServerEvent.RoundStarted += OnRoundStarted;
            ServerEvent.ChoosingStartTeamQueue += OnChoosingStartTeamQueue;
        }

        internal static void UnRegisterEvents()
        {
            ServerEvent.WaitingForPlayers -= OnWaitingForPlayers;

            PlayerEvent.Verified -= OnVerified;
            PlayerEvent.Left -= OnPlayerLeft;
            PlayerEvent.Spawned -= OnSpawned;
            PlayerEvent.Dying -= OnDying;
            PlayerEvent.Died -= OnDied;
            PlayerEvent.Hurting -= OnHurt;

            MapEvent.PlacingBlood -= OnDeniableEvent;
            PlayerEvent.SpawningRagdoll -= OnSpawnRagdoll;
            PlayerEvent.IntercomSpeaking -= OnDeniableEvent;
            PlayerEvent.DroppingItem -= OnDeniableEvent;
            PlayerEvent.DroppingAmmo -= OnDeniableEvent;
            PlayerEvent.InteractingDoor -= OnInteractingDoor;
            PlayerEvent.InteractingElevator -= OnDeniableEvent;
            PlayerEvent.InteractingLocker -= OnDeniableEvent;
            PlayerEvent.FlippingCoin -= OnCoinFlip;
            MapEvent.ChangingIntoGrenade -= OnDeniableEvent;

            // Scp106Event.CreatingPortal -= OnDeniableEvent;
            Scp106Event.Teleporting -= OnDeniableEvent;

            ServerEvent.RoundStarted -= OnRoundStarted;
            ServerEvent.ChoosingStartTeamQueue -= OnChoosingStartTeamQueue;
        }

        private static void OnCoinFlip(FlippingCoinEventArgs @event)
        {
            if (!IsLobby)
                return;
            if (ReadyCheckHandle.IsRunning == false && Config.UseReadyCheck)
            {
                ReadyCheckHandle = Timing.RunCoroutine(Methods.ReadyCheck());
            }
        }

        private static void OnChoosingStartTeamQueue(ChoosingStartTeamQueueEventArgs arg)
        {
            if (!IsLobby)
                return;
            //foreach (var q in arg.TeamRespawnQueue)
            //{
            //    Log.Debug(System.Enum.GetName(typeof(PlayerRoles.Team), q));
            //}

        }

        private static void OnHurt(HurtingEventArgs @event)
        {
            if (!IsLobby)
                return;
            void shouldSendHint(bool condition, [CallerArgumentExpression("condition")] string message = null)
            {
                if (@event.Attacker?.Role.Team == PlayerRoles.Team.SCPs && condition)
                {
                    @event.Attacker.Broadcast(new()
                    {
                        Content= $"SCPs can no longer hurt lobby players.\nReason: {message}",
                        Duration=10,
                        Show=true,
                    }, true);
                    @event.IsAllowed = false;
                }
            }

            shouldSendHint(Round.KillsByScp >= 10);
        }

        private static void OnPlayerLeft(LeftEventArgs @event)
        {
            RemoveSpawnedPlayer(@event.Player);
        }


        private static int spawnedRagdollsFor3114 = 0;
        private static void OnSpawnRagdoll(SpawningRagdollEventArgs @event)
        {
            if (!IsLobby)
                return;
            if (!Player.List.Any(player => player.Role.Type == PlayerRoles.RoleTypeId.Scp3114))
            {
                @event.IsAllowed = false;
            }
            else
            {
                if (Config.Ragdoll3114Limit == -1 || spawnedRagdollsFor3114 < Config.Ragdoll3114Limit)
                {
                    spawnedRagdollsFor3114++;
                }
                else
                {
                    @event.IsAllowed = false;
                }
            }
        }

        private static void OnWaitingForPlayers()
        {
            Log.Warn("Waiting players");
            Methods.PrepareForNewLobby();

            if (!WaitAndChillReborn.Singleton.Config.DisplayWaitingForPlayersScreen)
                GameObject.Find("StartRound").transform.localScale = Vector3.zero;

            if (LobbyTimer.IsRunning)
                Timing.KillCoroutines(LobbyTimer);

            if (ReadyCheckHandle.IsRunning)
                Timing.KillCoroutines(ReadyCheckHandle);

            //if (Server.FriendlyFire)
            //    FriendlyFireConfig.PauseDetector = true;

            if (WaitAndChillReborn.Singleton.Config.DisplayWaitMessage)
                LobbyTimer = Timing.RunCoroutine(Methods.LobbyTimer());

            if (Config.UseReadyCheck)
                ReadyCheckHandle = Timing.RunCoroutine(Methods.ReadyCheck());

            Log.Warn("Clear turned players");
            Scp173Role.TurnedPlayers.Clear();
            Scp096Role.TurnedPlayers.Clear();

            if (Config.UseReadyCheck)
            {
                Methods.SetupReadyCheckPositions();
            }
            else
            {
                Methods.SetupAvailablePositions();
            }

            Timing.CallDelayed(
                1f,
                () =>
                {
                    LockedPickups.Clear();

                    foreach (Pickup pickup in Pickup.List)
                    {
                        try
                        {
                            if (!pickup.IsLocked)
                            {
                                PickupSyncInfo info = pickup.Base.NetworkInfo;
                                info.Locked = true;
                                pickup.Base.NetworkInfo = info;

                                pickup.Base.GetComponent<Rigidbody>().isKinematic = true;
                                LockedPickups.Add(pickup);
                            }
                        }
                        catch (System.Exception)
                        {
                            // ignored
                        }
                    }
                });
        }

        private static void OnInteractingDoor(InteractingDoorEventArgs @event)
        {
            if (!IsLobby)
                return;
            if (AllowedInteractableDoors.Contains(@event.Door) == false)
            {
                var rooms = string.Join(", ", @event.Door.Rooms.Select(room => room.Name).ToList());
                Log.Debug($"Door accessed between: {rooms}");
                @event.IsAllowed = false;
            }
        }

        private static void OnVerified(VerifiedEventArgs ev)
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

        private static void OnSpawned(SpawnedEventArgs ev)
        {
            if (!IsLobby)
                return;

            if (RoundStart.singleton.NetworkTimer <= 1 && RoundStart.singleton.NetworkTimer != -2)
                return;

            _spawnPlayer(ev.Player, 0.3f);
        }

        private static void OnDeniableEvent(IExiledEvent ev)
        {
            if (!IsLobby)
                return;
            if (ev is IDeniableEvent deniableEvent)
                deniableEvent.IsAllowed = false;
        }

        private static void OnDying(DyingEventArgs ev)
        {
            if (IsLobby)
            {
                ev.Player.ClearInventory();
                return;
            }

        }

        private static void OnDied(DiedEventArgs ev)
        {
            if (!IsLobby || (RoundStart.singleton.NetworkTimer <= 1 && RoundStart.singleton.NetworkTimer != -2))
                return;
            Timing.CallDelayed(Config.SpawnDelay, () =>
            {
                var rolePool = RolesToChoose;

                if (Config.UniqueSCPs)
                {
                    var role = rolePool.GetNext(role =>
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
                    var role = rolePool.GetNext();
                    ev.Player.Role.Set(role);
                }

            });
            _spawnPlayer(ev.Player, Config.SpawnDelay * 2.5f);
        }

        private static void _spawnPlayer(Player player, float delay)
        {
            Timing.CallDelayed(
                delay,
                () =>
                {
                    player.Teleport(Config.MultipleRooms switch
                    {
                        true => LobbyAvailableSpawnPoints.GetNext(),
                        false => LobbyChoosedSpawnPoint
                    });

                    foreach (KeyValuePair<EffectType, byte> effect in Config.LobbyEffects)
                    {
                        player.EnableEffect(effect.Key);
                        player.ChangeEffectIntensity(effect.Key, effect.Value);
                    }

                    Timing.CallDelayed(
                        0.3f,
                        () =>
                        {
                            ReadyCheckRoom?.OnPlayerSpawn(player);

                            Exiled.CustomItems.API.Extensions.ResetInventory(player, Config.Inventory);

                            foreach (KeyValuePair<AmmoType, ushort> ammo in Config.Ammo)
                                player.Ammo[ammo.Key.GetItemType()] = ammo.Value;
                        });
                });
        }

        private static void OnRoundStarted()
        {
            Log.Info("Round started");
            foreach (ThrownProjectile throwable in Object.FindObjectsOfType<ThrownProjectile>())
            {
                if (throwable.TryGetComponent(out Rigidbody rb) && rb.velocity.sqrMagnitude <= 1f)
                    continue;

                throwable.transform.position = Vector3.zero;
                Timing.CallDelayed(1f, () => NetworkServer.Destroy(throwable.gameObject));
            }

            foreach (Player player in Player.List)
                player.DisableAllEffects();

            if (Config.TurnedPlayers)
            {
                Scp096Role.TurnedPlayers.Clear();
                Scp173Role.TurnedPlayers.Clear();
            }

            //if (Server.FriendlyFire)
            //    FriendlyFireConfig.PauseDetector = false;

            SpawnedInPlayers.Clear();
            ReadyCheckRoom?.OnRoundStart();

            Methods.Scp079sDoors(false);

            if (LobbyTimer.IsRunning)
                Timing.KillCoroutines(LobbyTimer);

            if (ReadyCheckHandle.IsRunning)
            {
                Log.Warn("Killing Ready Check coroutine");
                Timing.KillCoroutines(ReadyCheckHandle);
            }

            foreach (Pickup pickup in LockedPickups)
            {
                try
                {
                    PickupSyncInfo info = pickup.Base.NetworkInfo;
                    info.Locked = false;
                    pickup.Base.NetworkInfo = info;

                    pickup.Base.GetComponent<Rigidbody>().isKinematic = false;
                }
                catch (System.Exception)
                {
                    // ignored
                }
            }
            Round.KillsByScp = 0;
            spawnedRagdollsFor3114 = 0;
            LockedPickups.Clear();
        }

        private static readonly HashSet<Pickup> LockedPickups = new();
        private static readonly LobbyConfig Config = WaitAndChillReborn.Singleton.Config.LobbyConfig;
    }
}