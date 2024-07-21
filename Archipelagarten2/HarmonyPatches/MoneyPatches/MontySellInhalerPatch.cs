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
    [HarmonyPatch("SellInhaler")]
    public static class MontySellInhalerPatch
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

        // private void SellInhaler()
        public static bool Prefix(Monty __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(Monty), "SellInhaler", nameof(MontySellInhalerPatch), nameof(Prefix));

                EnvironmentController.Instance.UseItem(Item.Inhaler);
                _locationChecker.AddCheckedLocation("Sell Inhaler To Monty");

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(MontySellInhalerPatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
