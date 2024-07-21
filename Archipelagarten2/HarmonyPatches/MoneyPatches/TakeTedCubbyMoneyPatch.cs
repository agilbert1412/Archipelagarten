using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.HarmonyPatches.NPCPatches;
using Archipelagarten2.Locations;
using Archipelagarten2.Patching;
using BepInEx.Logging;
using HarmonyLib;
using KG2;
using UnityEngine;

namespace Archipelagarten2.HarmonyPatches.MoneyPatches
{
    [HarmonyPatch(typeof(ObjectInteractable))]
    [HarmonyPatch("TakeCubbyMoney")]
    public static class TakeTedCubbyMoneyPatch
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

        // private void TakeCubbyMoney()
        public static bool Prefix(ObjectInteractable __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(ObjectInteractable), "TakeCubbyMoney", nameof(TakeTedCubbyMoneyPatch), nameof(Prefix));

                GameObject.Find("MoneyInCubby").GetComponent<SpriteRenderer>().enabled = false;
                _locationChecker.AddCheckedLocation("Ted's Cubby");

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(TakeTedCubbyMoneyPatch)}.{nameof(Prefix)}:\n\t{ex}");
                return true; // run original logic
            }
        }
    }
}
