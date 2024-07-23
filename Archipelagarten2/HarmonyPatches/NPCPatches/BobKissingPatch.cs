using System;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(Bob))]
    [HarmonyPatch(nameof(Bob.KickOutKissing))]
    public static class BobKissingPatch
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

        // public void KickOutKissing()
        public static void Postfix(Bob __instance)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(Bob), nameof(Bob.KickOutKissing), nameof(BobKissingPatch), nameof(Postfix));
                
                _locationChecker.AddCheckedLocation("K-I-S-S-I-N-G");

                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(BobKissingPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
