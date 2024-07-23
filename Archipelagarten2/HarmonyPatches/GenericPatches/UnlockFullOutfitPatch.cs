using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Constants;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.GenericPatches
{
    [HarmonyPatch(typeof(EnvironmentController))]
    [HarmonyPatch(nameof(EnvironmentController.UnlockFullOutfit))]
    public static class UnlockFullOutfitPatch
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

        // public void UnlockFullOutfit(int x)
        public static bool Prefix(EnvironmentController __instance, int x)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(EnvironmentController), nameof(EnvironmentController.UnlockFullOutfit), nameof(UnlockFullOutfitPatch), nameof(Prefix));

                if (!_archipelago.SlotData.ShuffleOutfits)
                {
                    return true; // run original logic
                }

                var unlockedOutfits = __instance.GetOutfitUnlocks();
                if (!unlockedOutfits.Contains(x))
                    unlockedOutfits.Add(x);
                UnityEngine.Object.FindObjectOfType<OutfitUnlockPanel>().ShowUnlockOutfit(x);

                _locationChecker.AddCheckedLocation(Outfits.OutfitNames[x]);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(UnlockFullOutfitPatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
