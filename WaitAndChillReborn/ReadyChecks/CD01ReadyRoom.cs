namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using System.Linq;

    internal class CD01ReadyRoom : BaseReadyCheckRoom
    {
        public const string Name = "CD";

        protected override RoomType RoomType => RoomType.LczClassDSpawn;

        public override void SetupSpawnPoints()
        {
            var cdDoors = ThisRoom.Doors.Where(door => door.Rooms.Count == 1).ToList();
            SpawnPoints.AddRange(cdDoors.Select(door => door.Position - door.Transform.forward * 1.5f));
        }

        public override void OnPlayerSpawn(Player player)
        {
            base.OnPlayerSpawn(player);

            var door = Door.GetClosest(player.Position, out var d);
            if (door != null && d < 3)
            {
                door.IsOpen = false;
            }
        }
    }
}
