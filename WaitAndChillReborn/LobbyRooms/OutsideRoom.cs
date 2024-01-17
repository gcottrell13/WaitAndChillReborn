namespace WaitAndChillReborn
{
    using UnityEngine;

    public class OutsideRoom : BaseLobbyRoom
    {
        public const string Name = "OUTSIDE";

        public override void SetupSpawnPoints()
        {
            SpawnPoints.Add(new Vector3(164.5f, 1010.112f, 35.3f));
        }
    }
}
