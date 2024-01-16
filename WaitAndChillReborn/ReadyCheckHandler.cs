namespace WaitAndChillReborn
{
    using MEC;
    using PlayerRoles;
    using System.Collections.Generic;
    using static API.API;
    using Exiled.API.Features;

    internal static class ReadyCheckHandler
    {
        public static IEnumerator<float> ReadyCheck()
        {
            while (!Round.IsStarted)
            {
                var readyPlayerCount = 0;
                int numPlayers = validPlayers.Count;
                foreach (var player in validPlayers)
                {
                    // the spawned players list does not actually count towards the ready check, it only serves to smooth over some weirdness
                    // in-between game states.

                    // we need to make sure we must and only count spectators that have been previously corporeal.
                    // if we don't count spectators, then the game will never start because everyone is made into a spectator before the round starts.
                    if (player.Role.Type == RoleTypeId.Spectator && HasSpawnedPlayer(player))
                    {
                        readyPlayerCount++;
                    }
                    else if (ReadyPlayers.Contains(player) && player.Role.Type != RoleTypeId.Spectator)
                    {
                        readyPlayerCount++;
                        AddSpawnedPlayer(player);
                    }
                }

                if (numPlayers > 0)
                    IsReadyToStartGame = Config.ReadyCheckPercent <= readyPlayerCount * 100 / numPlayers;
                Round.IsLobbyLocked = !IsReadyToStartGame;

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
