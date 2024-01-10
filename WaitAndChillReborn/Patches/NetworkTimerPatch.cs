﻿namespace WaitAndChillReborn.Patches
{
    using GameCore;
    using HarmonyLib;
    using PlayerRoles;

    [HarmonyPatch(typeof(RoundStart), nameof(RoundStart.NetworkTimer), MethodType.Setter)]
    internal static class NetworkTimerPatch
    {
        private static void Postfix(RoundStart __instance, ref short value)
        {
            if (value != 1)
                return;

            foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
            {
                player.ClearInventory();
                EventHandlers.OnRoundPrepare();
                player.Role.Set(RoleTypeId.Spectator);
            }
        }
    }
}