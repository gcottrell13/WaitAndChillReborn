namespace WaitAndChillReborn
{
    using System.Collections.Generic;
    using Configs;
    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Roles;
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
    using Server = Exiled.API.Features.Server;
    using Exiled.Events.EventArgs.Server;
    using Exiled.API.Features;

    internal static class EventHandlers
    {
        internal static void RegisterEvents()
        {
            ServerEvent.WaitingForPlayers += OnWaitingForPlayers;

            PlayerEvent.Verified += OnVerified;
            PlayerEvent.Spawned += OnSpawned;
            PlayerEvent.Dying += OnDying;
            PlayerEvent.Died += OnDied;

            MapEvent.PlacingBlood += OnDeniableEvent;
            PlayerEvent.SpawningRagdoll += OnDeniableEvent;
            PlayerEvent.IntercomSpeaking += OnDeniableEvent;
            PlayerEvent.DroppingItem += OnDeniableEvent;
            PlayerEvent.DroppingAmmo += OnDeniableEvent;
            PlayerEvent.InteractingDoor += OnDeniableEvent;
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
            PlayerEvent.Spawned -= OnSpawned;
            PlayerEvent.Dying -= OnDying;
            PlayerEvent.Died -= OnDied;

            MapEvent.PlacingBlood -= OnDeniableEvent;
            PlayerEvent.SpawningRagdoll -= OnDeniableEvent;
            PlayerEvent.IntercomSpeaking -= OnDeniableEvent;
            PlayerEvent.DroppingItem -= OnDeniableEvent;
            PlayerEvent.DroppingAmmo -= OnDeniableEvent;
            PlayerEvent.InteractingDoor -= OnDeniableEvent;
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
            if (IsLobby && ReadyCheckHandle.IsRunning == false && WaitAndChillReborn.Singleton.Config.LobbyConfig.UseReadyCheck)
            {
                ReadyCheckHandle = Timing.RunCoroutine(Methods.ReadyCheck());
            }
        }

        private static void OnChoosingStartTeamQueue(ChoosingStartTeamQueueEventArgs arg)
        {
            foreach (var q in arg.TeamRespawnQueue)
            {
                Log.Debug(System.Enum.GetName(typeof(PlayerRoles.Team), q));
            }
            
        }

        private static void OnWaitingForPlayers()
        {
            Log.Warn("Waiting players");
            if (!WaitAndChillReborn.Singleton.Config.DisplayWaitingForPlayersScreen)
                GameObject.Find("StartRound").transform.localScale = Vector3.zero;

            if (LobbyTimer.IsRunning)
                Timing.KillCoroutines(LobbyTimer);

            //if (Server.FriendlyFire)
            //    FriendlyFireConfig.PauseDetector = true;

            if (WaitAndChillReborn.Singleton.Config.DisplayWaitMessage)
                LobbyTimer = Timing.RunCoroutine(Methods.LobbyTimer());

            if (WaitAndChillReborn.Singleton.Config.LobbyConfig.UseReadyCheck)
                ReadyCheckHandle = Timing.RunCoroutine(Methods.ReadyCheck());

            Log.Warn("Clear turned players");
            Scp173Role.TurnedPlayers.Clear();
            Scp096Role.TurnedPlayers.Clear();

            Log.Warn("Setting up Timing for SetupAvailablePositions");
            // Timing.CallDelayed(0.1f, );

            if (WaitAndChillReborn.Singleton.Config.LobbyConfig.UseReadyCheck)
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
                        ev.Player.Role.Set(Config.RolesToChoose[Random.Range(0, Config.RolesToChoose.Count)]);

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
            if (IsLobby && ev is IDeniableEvent deniableEvent)
                deniableEvent.IsAllowed = false;
        }

        private static void OnDying(DyingEventArgs ev)
        {
            if (IsLobby)
                ev.Player.ClearInventory();
        }

        private static void OnDied(DiedEventArgs ev)
        {
            if (!IsLobby || (RoundStart.singleton.NetworkTimer <= 1 && RoundStart.singleton.NetworkTimer != -2))
                return;
            Timing.CallDelayed(Config.SpawnDelay, () => ev.Player.Role.Set(Config.RolesToChoose[Random.Range(0, Config.RolesToChoose.Count)]));
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
                        true => LobbyAvailableSpawnPoints[Random.Range(0, LobbyAvailableSpawnPoints.Count)],
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

            if (ReadyCheckLockedDownRoom != null)
            {
                foreach (var door in ReadyCheckLockedDownRoom.Doors)
                {
                    door.ChangeLock(DoorLockType.None);
                    door.IsOpen = false;
                }

                var cdSpawn = Room.Get(RoomType.LczClassDSpawn);
                foreach (var door in cdSpawn.Doors)
                {
                    door.IsOpen = false;
                }
            }

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

            LockedPickups.Clear();
        }

        private static readonly HashSet<Pickup> LockedPickups = new();
        private static readonly LobbyConfig Config = WaitAndChillReborn.Singleton.Config.LobbyConfig;
    }
}