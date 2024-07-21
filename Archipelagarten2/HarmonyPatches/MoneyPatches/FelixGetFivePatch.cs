using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.HarmonyPatches.NPCPatches;
using Archipelagarten2.Locations;
using Archipelagarten2.Utilities;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.MoneyPatches
{
    [HarmonyPatch(typeof(Felix))]
    [HarmonyPatch("GetFive")]
    public static class FelixGetFivePatch
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

        // private void GetFive()
        public static bool Prefix(Felix __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(Felix), "GetFive", nameof(FelixGetFivePatch), nameof(Prefix));

                _locationChecker.AddCheckedLocation("Borrow Money From Felix");

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(FelixGetFivePatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
