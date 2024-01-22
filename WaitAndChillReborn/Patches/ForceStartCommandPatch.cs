namespace WaitAndChillReborn.Patches
{
    using CommandSystem.Commands.RemoteAdmin;
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Exiled.API.Features.Pools;
    using static HarmonyLib.AccessTools;
    using System.Reflection;

    [HarmonyPatch(typeof(ForceStartCommand), nameof(ForceStartCommand.Execute))]
    internal class ForceStartCommandPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            MethodInfo forceStartMethod = typeof(CharacterClassManager).GetMethod(nameof(CharacterClassManager.ForceRoundStart));
            int index = newInstructions.FindIndex(instruction => instruction.Calls(forceStartMethod));

            CodeInstruction forceStartCall = newInstructions[index];
            CodeInstruction customForceStart = new(OpCodes.Call, Method(typeof(EventHandlers), nameof(EventHandlers.ForceStart)));

            forceStartCall.MoveLabelsTo(customForceStart);
            forceStartCall.MoveBlocksTo(customForceStart);

            newInstructions[index] = customForceStart;

            newInstructions.Insert(index, new(OpCodes.Ldc_I4_1));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
