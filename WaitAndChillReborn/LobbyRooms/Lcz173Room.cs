namespace WaitAndChillReborn
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Toys;
    using MEC;
    using System;
    using System.Linq;
    using UnityEngine;

    internal class Lcz173Room : BaseLobbyRoom
    {
        public const string Name = "173";

        private Primitive THE_CUBE;
        private CoroutineHandle cubeEffect;

        protected override RoomType RoomType => RoomType.Lcz173;

        public override void SetUpRoom()
        {
            base.SetUpRoom();
            Door gate = ThisRoom.Doors.First(door => door.IsGate);
            gate.IsOpen = true;

            Vector3 cubePosition = gate.Position - gate.Transform.forward * 6 + Vector3.up * 1.5f;
            THE_CUBE = Primitive.Create(new(PrimitiveType.Cube, Color.red, cubePosition, Vector3.zero, Vector3.one * 2f, true));
            THE_CUBE.MovementSmoothing = 60;

            Action<float>[] effects = new Action<float>[] { PulseCube, RotateCube };
            int effect = UnityEngine.Random.Range(0, effects.Length);
            Action<float> action = effects[effect];

            DateTime startTime = DateTime.Now;
            cubeEffect = Timing.CallPeriodically(float.PositiveInfinity, 0.1f, () =>
            {
                action((float)(DateTime.Now - startTime).TotalSeconds);
            });
        }

        public override void SetupSpawnPoints()
        {
            SpawnPoints.Add(SpawnLocationType.Inside173Armory.GetPosition());
            SpawnPoints.Add(SpawnLocationType.Inside173Connector.GetPosition());
        }

        public override void OnRoundPrepare()
        {
            THE_CUBE.UnSpawn();
            if (cubeEffect.IsRunning) Timing.KillCoroutines(cubeEffect);
        }

        private void PulseCube(float time)
        {
            var scale = (float)Math.Sin(time);
            THE_CUBE.Scale = new Vector3(scale, scale, scale);
            if (scale < 0)
                THE_CUBE.Color = Color.LerpUnclamped(Color.blue, Color.red, -scale);
            else
                THE_CUBE.Color = Color.LerpUnclamped(Color.blue, Color.clear, scale);
        }

        private void RotateCube(float time)
        {
            THE_CUBE.Rotation = Quaternion.AngleAxis(time * 180, Vector3.up);
        }

    }
}
