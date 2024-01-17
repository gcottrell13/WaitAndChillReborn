namespace WaitAndChillReborn
{
    using UnityEngine;

    /// <summary>
    /// Some of your players may die by falling off the map.
    /// That's a sacrifice we're willing to make.
    /// </summary>
    public class OutsideRoom : BaseLobbyRoom
    {
        public const string Name = "OUTSIDE";

        CubeHelper cube;
        Exiled.API.Features.Toys.Light light;

        public override void SetUpRoom()
        {
            base.SetUpRoom();

            Vector3 pos = new(167.5f, 1010f, 53);
            cube = new CubeHelper(pos);
            light = Exiled.API.Features.Toys.Light.Create(pos + Vector3.up * 3f);
            light.Color = Color.white;
            light.Intensity = 300f;
            light.Range = 200f;
        }

        public override void SetupSpawnPoints()
        {
            SpawnPoints.Add(new Vector3(164.5f, 1010.112f, 35.3f));
        }

        public override void OnRoundPrepare()
        {
            base.OnRoundPrepare();

            cube.Cleanup();
            light.UnSpawn();
        }
    }
}
