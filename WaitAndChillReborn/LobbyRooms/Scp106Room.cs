namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using PlayerRoles;

    internal class Scp106Room : BaseLobbyRoom
    {
        public const string Name = "106";

        protected override RoomType RoomType => RoomType.Hcz106;

        public override void SetupSpawnPoints()
        {
            SpawnPoints.Add(RoleTypeId.Scp106.GetRandomSpawnLocation().Position);
        }
    }
}
