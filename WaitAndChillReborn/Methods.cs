namespace WaitAndChillReborn
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Exiled.API.Features;
    using UnityEngine;
    using MEC;
    using static API.API;

    internal static class Methods
    {
        internal static IEnumerator<float> LobbyTimer()
        {
            while (!Round.IsStarted)
            {
                StringBuilder stringBuilder = NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();

                if (WaitAndChillReborn.Singleton.Config.HintVertPos != 0 && WaitAndChillReborn.Singleton.Config.HintVertPos < 0)
                    for (int i = WaitAndChillReborn.Singleton.Config.HintVertPos; i < 0; i++)
                        stringBuilder.Append("\n");

                if (Config.UseReadyCheck && !IsReadyToStartGame)
                {
                    stringBuilder.Append("\n{PLAYERINSTRUCTION}");
                    stringBuilder.Append($"\n{{readyCount}} / {{players}} are Ready");
                    stringBuilder.Replace("{readyCount}", ReadyPlayers.Count.ToString());
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
                    string playerText = text;

                    if (Config.UseReadyCheck)
                    {
                        playerText = text.Replace("{PLAYERINSTRUCTION}", ReadyPlayers.Contains(player) ? Translation.YouAreReady : FindPlayerRoom(player).Instructions());
                    }

                    if (WaitAndChillReborn.Singleton.Config.UseHints)
                        player.ShowHint(playerText, 1.1f);
                    else
                        player.Broadcast(1, playerText);
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        internal static void SetupSpawnPoints()
        {
            if (Config.LobbyRoom.Contains(CD01ReadyRoom.Name)) LobbyAvailableRooms.Add(new CD01ReadyRoom());
            if (Config.LobbyRoom.Contains(Lcz173Room.Name)) LobbyAvailableRooms.Add(new Lcz173Room());
            if (Config.LobbyRoom.Contains(TowerReadyRoom.Name)) LobbyAvailableRooms.Add(new TowerReadyRoom());
            if (Config.LobbyRoom.Contains(Scp079Room.Name)) LobbyAvailableRooms.Add(new Scp079Room());
            if (Config.LobbyRoom.Contains(Scp106Room.Name)) LobbyAvailableRooms.Add(new Scp106Room());
            if (Config.LobbyRoom.Contains(Scp939Room.Name)) LobbyAvailableRooms.Add(new Scp939Room());
            if (Config.LobbyRoom.Contains(GateARoom.Name)) LobbyAvailableRooms.Add(new GateARoom());
            if (Config.LobbyRoom.Contains(GateBRoom.Name)) LobbyAvailableRooms.Add(new GateBRoom());
            if (Config.LobbyRoom.Contains(DrivewayReadyRoom.Name)) LobbyAvailableRooms.Add(new DrivewayReadyRoom());
            if (Config.LobbyRoom.Contains(WcRoom.Name)) LobbyAvailableRooms.Add(new WcRoom());
            if (Config.LobbyRoom.Contains(GlassRoom.Name)) LobbyAvailableRooms.Add(new GlassRoom());

            if (Config.StaticLobbyPositions.Any(p => p != -Vector3.one)) LobbyAvailableRooms.Add(new StaticReadyRoom());

            Log.Debug($"Added {LobbyAvailableRooms.Count} rooms");

            if (!Config.MultipleRooms) LobbyAvailableRooms = new() { LobbyAvailableRooms.RandomItem() };

            foreach (BaseReadyCheckRoom room in LobbyAvailableRooms)
            {
                room.SetUpRoom();
            }
        }
    }
}

