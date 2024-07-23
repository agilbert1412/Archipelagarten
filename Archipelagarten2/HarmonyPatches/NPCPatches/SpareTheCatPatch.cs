using System;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(ObjectInteractable))]
    [HarmonyPatch("SpareTheCat")]
    public static class SpareTheCatPatch
    {
        private static ILogger _logger;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(ILogger logger, ArchipelagoClient archipelago, LocationChecker locationChecker)
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
                _logger.LogDebugPatchIsRunning(nameof(ObjectInteractable), "SpareTheCat", nameof(SpareTheCatPatch), nameof(Postfix));
                
                _locationChecker.AddCheckedLocation("Common Decency");

                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(SpareTheCatPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
