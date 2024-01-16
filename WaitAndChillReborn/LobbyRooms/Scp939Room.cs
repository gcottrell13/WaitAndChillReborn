namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using PlayerRoles;

    internal class Scp939Room : BaseLobbyRoom
    {
        public const string Name = "939";

        protected override RoomType RoomType => RoomType.Hcz939;

        public override void SetupSpawnPoints()
        {
            SpawnPoints.Add(RoleTypeId.Scp939.GetRandomSpawnLocation().Position);
        }
    }
}
