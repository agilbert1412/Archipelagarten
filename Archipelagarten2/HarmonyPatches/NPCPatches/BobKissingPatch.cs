using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Locations;
using Archipelagarten2.Utilities;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(Bob))]
    [HarmonyPatch(nameof(Bob.KickOutKissing))]
    public static class BobKissingPatch
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

        // public void KickOutKissing()
        public static void Postfix(Bob __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(Bob), nameof(Bob.KickOutKissing), nameof(BobKissingPatch), nameof(Postfix));
                
                _locationChecker.AddCheckedLocation("K-I-S-S-I-N-G");

                return;
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(BobKissingPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
