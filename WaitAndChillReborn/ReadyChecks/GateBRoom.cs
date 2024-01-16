namespace WaitAndChillReborn
{
    using Exiled.API.Enums;

    internal class GateBRoom : BaseReadyCheckRoom
    {
        public const string Name = "GATE_B";

        protected override RoomType RoomType => RoomType.EzGateB;

        public override void SetupSpawnPoints()
        {
            SpawnPoints.Add(Gate.Position - Gate.Transform.forward);
        }
    }
}
