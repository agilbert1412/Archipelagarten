using System;
using Archipelagarten2.Archipelago;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KG2;
using UnityEngine;
using ILogger = KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger;

namespace Archipelagarten2.HarmonyPatches.MoneyPatches
{
    [HarmonyPatch(typeof(ObjectInteractable))]
    [HarmonyPatch("TakeCubbyMoney")]
    public static class TakeTedCubbyMoneyPatch
    {
        private static ILogger _logger;
        private static KindergartenArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(ILogger logger, KindergartenArchipelagoClient archipelago, LocationChecker locationChecker)
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
                if (_archipelago.SlotData.ShuffleMoney < 1)
                {
                    return true; // run original logic
                }

                _logger.LogDebugPatchIsRunning(nameof(ObjectInteractable), "TakeCubbyMoney", nameof(TakeTedCubbyMoneyPatch), nameof(Prefix));

                GameObject.Find("MoneyInCubby").GetComponent<SpriteRenderer>().enabled = false;
                _locationChecker.AddCheckedLocation("Ted's Cubby");

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(TakeTedCubbyMoneyPatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
