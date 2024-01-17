namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using UnityEngine;

    internal class Lcz173Room : BaseLobbyRoom
    {
        public const string Name = "173";

        private CubeHelper cube;

        protected override RoomType RoomType => RoomType.Lcz173;

        public override void SetUpRoom()
        {
            base.SetUpRoom();
            Gate.IsOpen = true;

            Vector3 cubePosition = Gate.Position - Gate.Transform.forward * 6 + Vector3.up * 1.5f;
            cube = new CubeHelper(cubePosition);
        }

        public override void SetupSpawnPoints()
        {
            SpawnPoints.Add(SpawnLocationType.Inside173Armory.GetPosition());
            SpawnPoints.Add(SpawnLocationType.Inside173Connector.GetPosition());
        }

        public override void OnRoundPrepare()
        {
            base.OnRoundPrepare();

            cube.Cleanup();
        }


    }
}
