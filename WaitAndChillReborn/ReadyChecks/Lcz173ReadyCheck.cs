using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Toys;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WaitAndChillReborn
{
    using static API.API;

    internal class Lcz173ReadyCheck : IReadyCheckRoom
    {
        private Room thisroom;
        private Door gate;
        private Door otherdoor;

        private Primitive THE_CUBE;
        private CoroutineHandle cubeEffect;

        private Vector3 behindHere;

        public void SetUpRoom()
        {
            thisroom = Room.Get(Exiled.API.Enums.RoomType.Lcz173);

            Log.Debug($"LCZ 173 room at: {thisroom.Position}");

            LobbyAvailableSpawnPoints.Add(SpawnLocationType.Inside173Armory.GetPosition());
            LobbyAvailableSpawnPoints.Add(SpawnLocationType.Inside173Connector.GetPosition());

            foreach (var door in thisroom.Doors)
            {
                if (door.Rooms.Any(room => room.RoomName != MapGeneration.RoomName.Lcz173)) continue; // this is a door to the outside, we don't want to go out there!
                if (door.IsGate)
                {
                    // we don't want to spawn by the gate, but let's open it
                    door.IsOpen = true;
                    gate = door;
                    continue;
                }
                AllowedInteractableDoors.Add(door); // this door is inside this room, this door is OK to open!
                door.IsOpen = true;
                otherdoor = door;
            }

            var dx = otherdoor.Position.x - gate.Position.x;
            var dz = otherdoor.Position.z - gate.Position.z;
            if (Math.Abs(dx) < Math.Abs(dz))
            {
                behindHere = new Vector3(0, 0, -Math.Sign(dz));
            }
            else
            {
                behindHere = new Vector3(-Math.Sign(dx), 0, 0);
            }

            // ------------------------------------------------------------------------------------------
            // THE CUBE

            var cubePosition = gate.Position + behindHere * 6 + Vector3.up * 1.5f;
            THE_CUBE = Primitive.Create(new(PrimitiveType.Cube, Color.red, cubePosition, Vector3.zero, Vector3.one * 2f, true));
            THE_CUBE.MovementSmoothing = 60;

            var effects = new Action<float>[] { PulseCube, RotateCube };
            var effect = UnityEngine.Random.Range(0, effects.Length);
            var action = effects[effect];

            var startTime = DateTime.Now;
            cubeEffect = Timing.CallPeriodically(float.PositiveInfinity, 0.1f, () =>
            {
                action((float)(DateTime.Now - startTime).TotalSeconds);
            });
        }

        public bool IsPlayerReady(Player player)
        {
            var playerDir = player.Position - gate.Position;
            return playerDir.x * behindHere.x + playerDir.z * behindHere.z > 0;
        }

        public void OnRoundPrepare()
        {

        }

        public void OnRoundStart()
        {
            gate.IsOpen = false;
            THE_CUBE.UnSpawn();
            if (cubeEffect.IsRunning) Timing.KillCoroutines(cubeEffect);
        }

        public string Instructions() => WaitAndChillReborn.Singleton.Translation.Lcz173ReadyInstructions;

        public void OnPlayerSpawn(Player player)
        {
            GivenCandyToPlayer.Remove(player);
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
