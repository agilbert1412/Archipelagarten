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
    [HarmonyPatch("SellWeedMonstermon")]
    public static class MontySellWeedMonstermonPatch
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

        // private void SellWeedMonstermon()
        public static bool Prefix(Monty __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(Monty), "SellWeedMonstermon", nameof(MontySellWeedMonstermonPatch), nameof(Prefix));

                EnvironmentController.Instance.UseItem(Item.BagOfWeed);
                EnvironmentController.Instance.UnlockMonstermon(25);
                _locationChecker.AddCheckedLocation("Sell Drugs To Monty");

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(MontySellWeedMonstermonPatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
