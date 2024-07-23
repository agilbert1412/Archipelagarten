using System;
using Archipelagarten2.Archipelago;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.MoneyPatches
{
    [HarmonyPatch(typeof(Monty))]
    [HarmonyPatch("SellWeedMonstermon")]
    public static class MontySellWeedMonstermonPatch
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

        // private void SellWeedMonstermon()
        public static bool Prefix(Monty __instance)
        {
            try
            {
                if (_archipelago.SlotData.ShuffleMoney < 1)
                {
                    return true; // run original logic
                }

                _logger.LogDebugPatchIsRunning(nameof(Monty), "SellWeedMonstermon", nameof(MontySellWeedMonstermonPatch), nameof(Prefix));

                EnvironmentController.Instance.UseItem(Item.BagOfWeed);
                EnvironmentController.Instance.UnlockMonstermon(25);
                _locationChecker.AddCheckedLocation("Sell Drugs To Monty");

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(MontySellWeedMonstermonPatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
