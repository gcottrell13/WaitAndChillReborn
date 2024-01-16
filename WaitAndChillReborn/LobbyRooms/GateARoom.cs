namespace WaitAndChillReborn
{
    using Exiled.API.Enums;

    internal class GateARoom : BaseLobbyRoom
    {
        public const string Name = "GATE_A";

        protected override RoomType RoomType => RoomType.EzGateA;

        public override void SetupSpawnPoints()
        {
            SpawnPoints.Add(Gate.Position - Gate.Transform.forward);
        }
    }
}
