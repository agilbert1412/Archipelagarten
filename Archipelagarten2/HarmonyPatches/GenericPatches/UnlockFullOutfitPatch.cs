using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Constants;
using Archipelagarten2.Locations;
using Archipelagarten2.Patching;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.GenericPatches
{
    [HarmonyPatch(typeof(EnvironmentController))]
    [HarmonyPatch(nameof(EnvironmentController.UnlockFullOutfit))]
    public static class UnlockFullOutfitPatch
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

        // public void UnlockFullOutfit(int x)
        public static bool Prefix(EnvironmentController __instance, int x)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(EnvironmentController), nameof(EnvironmentController.UnlockFullOutfit), nameof(UnlockFullOutfitPatch), nameof(Prefix));

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
                _logger.LogError($"Failed in {nameof(UnlockFullOutfitPatch)}.{nameof(Prefix)}:\n\t{ex}");
                return true; // run original logic
            }
        }
    }
}
