namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using UnityEngine;

    internal class PC15Room : BaseLobbyRoom
    {
        public const string Name = "PC15";

        private int sign = 1;

        protected override RoomType RoomType => RoomType.LczCafe;

        public override void SetupSpawnPoints()
        {
            Vector3 intoRoom = Entrance.Transform.forward * sign;
            Vector3 right = Entrance.Transform.right;
            SpawnPoints.Add(Entrance.Position + intoRoom * 15 + right * 4);
            SpawnPoints.Add(Entrance.Position + intoRoom * 15 - right * 4);
        }

        public override void OnPlayerSpawn(Player player)
        {
            base.OnPlayerSpawn(player);

            if (player?.CurrentRoom?.Type != RoomType)
            {
                // The door's forward direction does not always point into the room.
                // This process appears seamless enough to the player, it shouldn't be jarring.
                // It will only happen to one person, fixing it for everyone else.
                sign = -1;
                SpawnPoints.Clear();
                SetupSpawnPoints();
                Log.Debug("Flipping PC15 sign");
                base.OnPlayerSpawn(player);
            }
        }
    }
}
