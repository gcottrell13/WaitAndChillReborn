namespace WaitAndChillReborn
{
    using UnityEngine;

    public class DrivewayRoom : BaseLobbyRoom
    {
        public const string Name = "DRIVEWAY";

        public override void SetupSpawnPoints()
        {
            SpawnPoints.Add(new Vector3(0f, 995f, -8f));
        }
    }
}
