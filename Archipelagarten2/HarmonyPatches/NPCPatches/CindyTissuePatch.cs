using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Locations;
using Archipelagarten2.Patching;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(Cindy))]
    [HarmonyPatch("GetTissue")]
    public static class CindyTissuePatch
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

        // private void GetTissue()
        public static void Postfix(Cindy __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(Cindy), "GetTissue", nameof(CindyTissuePatch), nameof(Postfix));
                
                _locationChecker.AddCheckedLocation("Cry A River");

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(CindyTissuePatch)}.{nameof(Postfix)}:\n\t{ex}");
                return;
            }
        }
    }
}
