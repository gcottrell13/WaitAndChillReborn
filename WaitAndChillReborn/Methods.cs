namespace WaitAndChillReborn
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Configs;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using MapGeneration.Distributors;
    using PlayerRoles;
    using UnityEngine;
    using MEC;
    using static API.API;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Roles;

    internal static class Methods
    {
        private static List<Player> validPlayers => Player.List.Where(p => p.Role.Type != RoleTypeId.Overwatch).ToList();

        internal static void PrepareForNewLobby()
        {
            LobbyAvailableSpawnPoints.Clear();
            AllowedInteractableDoors.Clear();
            SpawnedInPlayers.Clear();
            SpawnedPinataThisRound = false;
        }

        internal static IEnumerator<float> LobbyTimer()
        {
            while (!Round.IsStarted)
            {
                StringBuilder stringBuilder = NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();

                if (WaitAndChillReborn.Singleton.Config.HintVertPos != 0 && WaitAndChillReborn.Singleton.Config.HintVertPos < 0)
                    for (int i = WaitAndChillReborn.Singleton.Config.HintVertPos; i < 0; i++)
                        stringBuilder.Append("\n");

                if (WaitAndChillReborn.Singleton.Config.LobbyConfig.UseReadyCheck && ReadyPlayers < validPlayers.Count)
                {
                    stringBuilder.Append("Waiting for players to:");
                    stringBuilder.Append($"\n{ReadyCheckRoom?.Instructions()}");
                    stringBuilder.Append($"\n{{readyCount}} / {{players}}");
                    stringBuilder.Replace("{readyCount}", ReadyPlayers.ToString());
                }
                else
                {
                    stringBuilder.Append("\n" + Translation.TopMessage);
                    stringBuilder.Append("\n" + Translation.BottomMessage);

                    short networkTimer = GameCore.RoundStart.singleton.NetworkTimer;

                    switch (networkTimer)
                    {
                        case -2: stringBuilder.Replace("{seconds}", Translation.ServerIsPaused); break;

                        case -1: stringBuilder.Replace("{seconds}", Translation.RoundIsBeingStarted); break;

                        case 1: stringBuilder.Replace("{seconds}", $"{networkTimer} {Translation.OneSecondRemain}"); break;

                        case 0: stringBuilder.Replace("{seconds}", Translation.RoundIsBeingStarted); break;

                        default: stringBuilder.Replace("{seconds}", $"{networkTimer} {Translation.XSecondsRemains}"); break;
                    }
                }

                if (validPlayers.Count == 1)
                {
                    stringBuilder.Replace("{players}", $"1 {Translation.OnePlayerConnected}");
                }
                else
                {
                    stringBuilder.Replace("{players}", $"{validPlayers.Count} {Translation.XPlayersConnected}");
                }

                if (WaitAndChillReborn.Singleton.Config.HintVertPos != 0 && WaitAndChillReborn.Singleton.Config.HintVertPos > 0)
                    for (int i = 0; i < WaitAndChillReborn.Singleton.Config.HintVertPos; i++)
                        stringBuilder.Append("\n");

                string text = NorthwoodLib.Pools.StringBuilderPool.Shared.ToStringReturn(stringBuilder);
                
                foreach (Player player in validPlayers)
                {
                    if (WaitAndChillReborn.Singleton.Config.UseHints)
                        player.ShowHint(text, 1.1f);
                    else
                        player.Broadcast(1, text);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        internal static IEnumerator<float> ReadyCheck()
        {
            var colors = new[]
            {
                System.Drawing.Color.Red,
                System.Drawing.Color.Blue,
                System.Drawing.Color.Green,
                System.Drawing.Color.Orange,
                System.Drawing.Color.Purple,
                System.Drawing.Color.Yellow,
            };
            var currentColor = 0;

            while (!Round.IsStarted)
            {
                ReadyPlayers = 0;
                var numPlayers = validPlayers.Count;
                foreach (var player in validPlayers)
                {
                    if (player.CurrentRoom == null)
                    {
                        // the spawned players list does not actually count towards the ready check, it only serves to smooth over some weirdness
                        // in-between game states.

                        // we need to make sure we must and only count spectators that have been previously corporeal.
                        // if we don't count spectators, then the game will never start because everyone is made into a spectator before the round starts.
                        if (player.Role.Type == RoleTypeId.Spectator && HasSpawnedPlayer(player))
                        {
                            ReadyPlayers++;
                        }
                    }
                    else if (ReadyCheckRoom?.IsPlayerReady(player) == true && player.Role.Type != RoleTypeId.Spectator)
                    {
                        ReadyPlayers++;
                        AddSpawnedPlayer(player);
                    }
                }

                Log.Debug($"{ReadyPlayers} / {numPlayers} players are ready");

                if (ReadyPlayers < numPlayers)
                {
                    Round.IsLobbyLocked = true;
                }
                else
                {
                    Round.IsLobbyLocked = false;
                }

                var nextColor = colors[currentColor++ % colors.Length];
                Room.Get(RoomType.Surface).Color = new Color(nextColor.R / 255f, nextColor.G / 255f, nextColor.B / 255f);

                doPinata();
                yield return Timing.WaitForSeconds(1f);
            }
        }

        internal static void SetupReadyCheckPositions()
        {
            Log.Debug("Setting up available Ready Check positions");

            Log.Debug("UseReadyCheck");

            var readyCheckRooms = new List<IReadyCheckRoom>();

            if (Config.LobbyRoom.Contains("ClassDSpawn")) readyCheckRooms.Add(new CD01ReadyCheck());
            if (Config.LobbyRoom.Contains("Lcz173Spawn")) readyCheckRooms.Add(new Lcz173ReadyCheck());

            Log.Debug($"SetupReadyCheckPositions has {readyCheckRooms.Count} rooms available to choose from");
            ReadyCheckRoom = readyCheckRooms[Random.Range(0, readyCheckRooms.Count)];
            PrepareForNewLobby();

            ReadyCheckRoom.SetUpRoom();

            _pickSpawnPoint();
        }

        internal static void SetupAvailablePositions()
        {
            Log.Debug("Setting up available positions");

            for (int i = 0; i < Config.LobbyRoom.Count; i++)
                Config.LobbyRoom[i] = Config.LobbyRoom[i].ToUpper();
            
            if (Config.LobbyRoom.Contains("TOWER1")) LobbyAvailableSpawnPoints.Add(new Vector3(39.150f, 1015.112f, -31.818f));
            if (Config.LobbyRoom.Contains("TOWER2")) LobbyAvailableSpawnPoints.Add(new Vector3(162.125f, 1019.440f, -13f));
            if (Config.LobbyRoom.Contains("TOWER3")) LobbyAvailableSpawnPoints.Add(new Vector3(108.3f, 1048.048f, -14.075f));
            if (Config.LobbyRoom.Contains("TOWER4")) LobbyAvailableSpawnPoints.Add(new Vector3(-15.105f, 1014.461f, -31.797f));
            if (Config.LobbyRoom.Contains("TOWER5")) LobbyAvailableSpawnPoints.Add(new Vector3(44.137f, 1013.065f, -50.931f));
            if (Config.LobbyRoom.Contains("NUKE_SURFACE")) LobbyAvailableSpawnPoints.Add(new Vector3(29.69f, 991.86f, -26.7f));

            Dictionary<RoomType, string> roomToString = new ()
            {
                { RoomType.EzShelter, "SHELTER" },
                { RoomType.EzGateA, "GATE_A" },
                { RoomType.EzGateB, "GATE_B" },
            };

            foreach (Room room in Room.List)
            {
                if (roomToString.ContainsKey(room.Type) && Config.LobbyRoom.Contains(roomToString[room.Type]))
                {
                    Vector3 roomPos = room.transform.position;
                    LobbyAvailableSpawnPoints.Add(new Vector3(roomPos.x, roomPos.y + 2f, roomPos.z));
                }
            }

            if (Config.LobbyRoom.Contains("079"))
            {
                Vector3 secondDoorPos = Door.Get("079_SECOND").Transform.position;
                LobbyAvailableSpawnPoints.Add(Vector3.MoveTowards(RoleTypeId.Scp079.GetRandomSpawnLocation().Position, secondDoorPos, 7f));
                Scp079sDoors(true);
                Log.Error("079");
            }
            
            Dictionary<string, RoleTypeId> stringToRole = new()
            {
                { "049", RoleTypeId.Scp049 },
                { "106", RoleTypeId.Scp106 },
                { "173", RoleTypeId.Scp173 },
                { "939", RoleTypeId.Scp939 },
            };

            foreach (KeyValuePair<string, RoleTypeId> role in stringToRole)
                if (Config.LobbyRoom.Contains(role.Key))
                {
                    LobbyAvailableSpawnPoints.Add(role.Value.GetRandomSpawnLocation().Position);
                    Log.Error(role.Key);
                }

            foreach (Vector3 position in Config.StaticLobbyPositions)
            {
                if (position == -Vector3.one)
                    continue;

                LobbyAvailableSpawnPoints.Add(position);
            }

            _pickSpawnPoint();
        }

        private static void _pickSpawnPoint()
        {
            Log.Debug($"Have {LobbyAvailableSpawnPoints.Count} Spawn Points:");

            LobbyAvailableSpawnPoints.ShuffleList();
            LobbyChoosedSpawnPoint = LobbyAvailableSpawnPoints.GetNext();
        }

        internal static void Scp079sDoors(bool state)
        {
            Vector3 secondDoorPos = Door.Get("079_SECOND").Transform.position;

            foreach (Door controlRoomDoor in Door.List.Where(d => (d.Transform.position - secondDoorPos).sqrMagnitude < 25f))
                controlRoomDoor.IsOpen = state;
        }

        internal static void doPinata()
        {
            if (!SpawnedPinataThisRound && Scp956Pinata.TryGetInstance(out var pinata))
            {
                var child = Player.List.Where(p => p.TryGetEffect(EffectType.Scp559, out var effect) && effect.Intensity > 0).ToList().FirstOrDefault();
                if (child != null)
                {
                    SpawnedPinataThisRound = true;
                    pinata.SpawnBehindTarget(child.ReferenceHub);
                }
            }
        }

        private static readonly Translation Translation = WaitAndChillReborn.Singleton.Translation;
        private static readonly LobbyConfig Config = WaitAndChillReborn.Singleton.Config.LobbyConfig;
    }
}

