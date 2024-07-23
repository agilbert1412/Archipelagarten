using System;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(Cindy))]
    [HarmonyPatch("GetTissue")]
    public static class CindyTissuePatch
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

        // private void GetTissue()
        public static void Postfix(Cindy __instance)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(Cindy), "GetTissue", nameof(CindyTissuePatch), nameof(Postfix));
                
                _locationChecker.AddCheckedLocation("Cry A River");

                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(CindyTissuePatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
