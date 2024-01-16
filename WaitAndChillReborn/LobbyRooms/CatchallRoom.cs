namespace WaitAndChillReborn
{
    using Exiled.API.Features;

    internal class CatchallRoom : BaseLobbyRoom
    {
        public override void SetupSpawnPoints() { }

        public override bool IsPlayerInRoom(Player player) => true;
    }
}
