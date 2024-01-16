namespace WaitAndChillReborn
{
    using MEC;
    using System.Collections.Generic;
    using static API.API;
    using Exiled.API.Features;
    using PlayerRoles;
    using System.Linq;

    internal static class ReadyCheckHandler
    {
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
