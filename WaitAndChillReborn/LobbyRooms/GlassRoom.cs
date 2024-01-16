namespace WaitAndChillReborn
{
    using Exiled.API.Enums;

    internal class GlassRoom : BaseLobbyRoom
    {
        public const string Name = "GR18";

        protected override RoomType RoomType => RoomType.LczGlassBox;

        public override void SetupSpawnPoints()
        {
            SpawnPoints.Add(Gate.Position - Gate.Transform.forward);
        }
    }
}
