namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using PlayerRoles;

    internal class Scp049Room : BaseLobbyRoom
    {
        public const string Name = "049";

        protected override RoomType RoomType => RoomType.Hcz049;

        public override void SetupSpawnPoints()
        {
            SpawnPoints.Add(RoleTypeId.Scp049.GetRandomSpawnLocation().Position);
        }
    }
}
