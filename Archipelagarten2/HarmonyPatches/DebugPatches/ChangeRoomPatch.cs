using System;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.DebugPatches
{
    [HarmonyPatch(typeof(EnvironmentController))]
    [HarmonyPatch(nameof(EnvironmentController.ChangeRoom))]
    public static class ChangeRoomPatch
    {
        private static ILogger _logger;

        public static void Initialize(ILogger logger)
        {
            _logger = logger;
        }

        // public void ChangeRoom(Room r)
        public static void Postfix(EnvironmentController __instance, Room r)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(EnvironmentController), nameof(EnvironmentController.ChangeRoom), nameof(ChangeRoomPatch), nameof(Postfix), r);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(ChangeRoomPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
