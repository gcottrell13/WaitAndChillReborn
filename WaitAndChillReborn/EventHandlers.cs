namespace WaitAndChillReborn
{
    using Exiled.API.Features.Pickups;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using MEC;
    using Mirror;
    using UnityEngine;
    using static API.API;
    using Log = Exiled.API.Features.Log;
    using Object = UnityEngine.Object;
    using Player = Exiled.API.Features.Player;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using System.Collections.Generic;
    using System.Linq;

    internal static class EventHandlers
    {

        public static void ForceStart()
        {
            foreach (Player player in Player.List)
            {
                if (player.IsAlive)
                {
                    player.Role.Set(PlayerRoles.RoleTypeId.Spectator);
                }
            }

            if (ReadyCheckHandle.IsRunning)
            {
                Timing.KillCoroutines(ReadyCheckHandle);
            }

            Round.IsLobbyLocked = false;
            Timing.CallDelayed(2f, () => { CharacterClassManager.ForceRoundStart(); });
        }

        public static void OnRoundPrepare()
        {
            foreach (BaseLobbyRoom room in LobbyAvailableRooms)
                room.OnRoundPrepare();
        }

        public static void OnWaitingForPlayers()
        {
            Log.Warn("Waiting players");
            Reset();

            if (!WaitAndChillReborn.Singleton.Config.DisplayWaitingForPlayersScreen)
                GameObject.Find("StartRound").transform.localScale = Vector3.zero;

            if (LobbyTimer.IsRunning)
                Timing.KillCoroutines(LobbyTimer);

            if (ReadyCheckHandle.IsRunning)
                Timing.KillCoroutines(ReadyCheckHandle);

            if (Server.FriendlyFire)
                Log.Info(Server.ExecuteCommand("/friendlyfiredetector pause"));

            if (WaitAndChillReborn.Singleton.Config.DisplayWaitMessage)
                LobbyTimer = Timing.RunCoroutine(Methods.LobbyTimer());

            if (Config.UseReadyCheck)
                ReadyCheckHandle = Timing.RunCoroutine(ReadyCheck());

            Log.Warn("Clear turned players");
            Scp173Role.TurnedPlayers.Clear();
            Scp096Role.TurnedPlayers.Clear();

            Methods.SetupSpawnPoints();

            Timing.CallDelayed(
                1f,
                () =>
                {
                    foreach (Pickup pickup in Pickup.List)
                    {
                        LockedPickups.Add(pickup);
                        try
                        {
                            if (!pickup.IsLocked)
                            {
                                PickupSyncInfo info = pickup.Base.NetworkInfo;
                                info.Locked = true;
                                pickup.Base.NetworkInfo = info;

                                pickup.Base.GetComponent<Rigidbody>().isKinematic = true;
                            }
                        }
                        catch (System.Exception)
                        {
                            // ignored
                        }
                    }
                });
        }

        public static void OnRoundStarted()
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
            {
                player.DisableAllEffects();
                PlayerRoles.Voice.Intercom.TrySetOverride(player.ReferenceHub, false);
                PlayerEventHandlers.removeReadyInName(player);
            }

            if (Server.FriendlyFire)
                Log.Info(Server.ExecuteCommand("/friendlyfiredetector unpause"));

            if (LobbyTimer.IsRunning)
                Timing.KillCoroutines(LobbyTimer);

            if (ReadyCheckHandle.IsRunning)
            {
                Log.Warn("Killing Ready Check coroutine");
                Timing.KillCoroutines(ReadyCheckHandle);
            }

            foreach (Pickup pickup in Pickup.List)
            {
                if (LockedPickups.Contains(pickup))
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
                else
                {
                    pickup.Destroy();
                }
            }

            foreach (Ragdoll ragdoll in Ragdoll.List)
            {
                if (!string.IsNullOrWhiteSpace(ragdoll.Nickname))
                    ragdoll.Destroy();
            }

            if (Config.TurnedPlayers)
            {
                Scp096Role.TurnedPlayers.Clear();
                Scp173Role.TurnedPlayers.Clear();
            }

            Reset();
        }

        public static IEnumerator<float> ReadyCheck()
        {
            while (!Round.IsStarted)
            {
                int numPlayers = validPlayers.Count;

                if (numPlayers > 0)
                {
                    List<Player> ready = ReadyPlayers.Intersect(validPlayers).ToList();
                    IsReadyToStartGame = Config.ReadyCheckPercent <= ready.Count * 100 / numPlayers;
                    Round.IsLobbyLocked = !IsReadyToStartGame;
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}