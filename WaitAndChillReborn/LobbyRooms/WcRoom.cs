namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Doors;
    using System.Linq;

    internal class WcRoom : BaseLobbyRoom
    {
        public const string Name = "WC";

        protected override RoomType RoomType => RoomType.LczToilets;

        public override void SetupSpawnPoints()
        {
            Door door = ThisRoom.Doors.First(door => door.Rooms.Count == 1);
            SpawnPoints.Add(door.Position + door.Transform.forward);
        }
    }
}
