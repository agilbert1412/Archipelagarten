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
    [HarmonyPatch(typeof(Monty))]
    [HarmonyPatch("SellWeed")]
    public static class MontySellWeedPatch
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

        // private void SellWeed()
        public static bool Prefix(Monty __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(Monty), "SellWeed", nameof(MontySellWeedPatch), nameof(Prefix));

                EnvironmentController.Instance.UseItem(Item.BagOfWeed);
                _locationChecker.AddCheckedLocation("Sell Drugs To Monty");

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(MontySellWeedPatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
