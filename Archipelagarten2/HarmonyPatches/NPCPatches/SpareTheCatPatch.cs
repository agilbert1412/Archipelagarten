using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Locations;
using Archipelagarten2.Patching;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(ObjectInteractable))]
    [HarmonyPatch("SpareTheCat")]
    public static class SpareTheCatPatch
    {
        private static ManualLogSource _logger;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(ManualLogSource logger, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _logger = logger;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // private void SpareTheCat()
        public static void Postfix(ObjectInteractable __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(ObjectInteractable), "SpareTheCat", nameof(SpareTheCatPatch), nameof(Postfix));
                
                _locationChecker.AddCheckedLocation("Common Decency");

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(SpareTheCatPatch)}.{nameof(Postfix)}:\n\t{ex}");
                return;
            }
        }
    }
}
