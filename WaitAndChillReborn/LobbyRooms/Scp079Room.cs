namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Doors;

    internal class Scp079Room : BaseLobbyRoom
    {
        public const string Name = "079";

        protected override RoomType RoomType => RoomType.Hcz079;

        public override void SetupSpawnPoints()
        {
            foreach (Door door in ThisRoom.Doors)
            {
                if (door.IsGate) 
                { 
                    SpawnPoints.Add(door.Position);
                }
            }
        }
    }
}
