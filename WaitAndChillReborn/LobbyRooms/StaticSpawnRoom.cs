namespace WaitAndChillReborn
{
    using UnityEngine;
    using static API.API;

    internal class StaticSpawnRoom : BaseLobbyRoom
    {
        public override void SetupSpawnPoints()
        {
            foreach (Vector3 position in Config.StaticLobbyPositions)
            {
                if (position == -Vector3.one)
                    continue;

                SpawnPoints.Add(position);
            }
        }
    }
}
