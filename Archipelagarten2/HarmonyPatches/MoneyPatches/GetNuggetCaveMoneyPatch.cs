using System;
using Archipelagarten2.Archipelago;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.MoneyPatches
{
    [HarmonyPatch(typeof(ObjectInteractable))]
    [HarmonyPatch("GetNuggetCaveMoney")]
    public static class GetNuggetCaveMoneyPatch
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

        // private void GetNuggetCaveMoney()
        public static bool Prefix(ObjectInteractable __instance)
        {
            try
            {
                if (_archipelago.SlotData.ShuffleMoney < 1)
                {
                    return true; // run original logic
                }

                _logger.LogDebugPatchIsRunning(nameof(ObjectInteractable), "GetNuggetCaveMoney", nameof(GetNuggetCaveMoneyPatch), nameof(Prefix));

                _locationChecker.AddCheckedLocation("Skeleton Wallet");

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(GetNuggetCaveMoneyPatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
