
namespace WaitAndChillReborn
{
    using Exiled.API.Features.Toys;
    using MEC;
    using System;
    using UnityEngine;

    internal class CubeHelper
    {
        private Primitive cube;
        private CoroutineHandle cubeEffect;

        public CubeHelper(Vector3 cubePosition)
        {
            CreateCube(cubePosition);
        }

        public void Cleanup()
        {
            cube?.UnSpawn();
            if (cubeEffect.IsRunning)
            {
                Timing.KillCoroutines(cubeEffect);
            }

            
        }

        public Primitive CreateCube(Vector3 cubePosition)
        {
            cube = Primitive.Create(new(PrimitiveType.Cube, Color.red, cubePosition, Vector3.zero, Vector3.one * 2f, true));
            cube.MovementSmoothing = 60;

            Action<float>[] effects = new Action<float>[] { PulseCube, RotateCube };
            int effect = UnityEngine.Random.Range(0, effects.Length);
            Action<float> action = effects[effect];

            DateTime startTime = DateTime.Now;
            cubeEffect = Timing.CallPeriodically(float.PositiveInfinity, 0.1f, () =>
            {
                action((float)(DateTime.Now - startTime).TotalSeconds);
            });
            return cube;
        }

        private void PulseCube(float time)
        {
            float scale = (float)Math.Sin(time);
            cube.Scale = new Vector3(scale, scale, scale);
            if (scale < 0)
                cube.Color = Color.LerpUnclamped(Color.blue, Color.red, -scale);
            else
                cube.Color = Color.LerpUnclamped(Color.blue, Color.green, scale);
        }

        private void RotateCube(float time)
        {
            cube.Rotation = Quaternion.AngleAxis(time * 180, Vector3.up);
        }
    }
}
