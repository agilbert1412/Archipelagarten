using System;
using Archipelagarten2.Archipelago;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.MoneyPatches
{
    [HarmonyPatch(typeof(Monty))]
    [HarmonyPatch("SellInhaler")]
    public static class MontySellInhalerPatch
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

        // private void SellInhaler()
        public static bool Prefix(Monty __instance)
        {
            try
            {
                if (_archipelago.SlotData.ShuffleMoney < 1)
                {
                    return true; // run original logic
                }

                _logger.LogDebugPatchIsRunning(nameof(Monty), "SellInhaler", nameof(MontySellInhalerPatch), nameof(Prefix));

                EnvironmentController.Instance.UseItem(Item.Inhaler);
                _locationChecker.AddCheckedLocation("Sell Inhaler To Monty");

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(MontySellInhalerPatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
