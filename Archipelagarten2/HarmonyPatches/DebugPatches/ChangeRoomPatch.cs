using System;
using Archipelagarten2.Utilities;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.DebugPatches
{
    [HarmonyPatch(typeof(EnvironmentController))]
    [HarmonyPatch(nameof(EnvironmentController.ChangeRoom))]
    public static class ChangeRoomPatch
    {
        private static ManualLogSource _logger;

        public static void Initialize(ManualLogSource logger)
        {
            _logger = logger;
        }

        // public void ChangeRoom(Room r)
        public static void Postfix(EnvironmentController __instance, Room r)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(EnvironmentController), nameof(EnvironmentController.ChangeRoom), nameof(ChangeRoomPatch), nameof(Postfix), r);
                return;
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(ChangeRoomPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
