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
    [HarmonyPatch(nameof(EnvironmentController.UnlockMonstermon))]
    public static class UnlockMonstermonPatch
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

        // public void UnlockMonstermon(int x)
        public static bool Prefix(EnvironmentController __instance, int x)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(EnvironmentController), nameof(EnvironmentController.UnlockMonstermon), nameof(UnlockMonstermonPatch), nameof(Prefix));

                if (!_archipelago.SlotData.ShuffleMonstermon)
                {
                    return true; // run original logic
                }

                var unlockedMonstermon = __instance.GetMonstermonUnlocks();
                if (!unlockedMonstermon.Contains(x))
                {
                    unlockedMonstermon.Add(x);
                }

                UnityEngine.Object.FindObjectOfType<MonstermonUnlockPanel>().ShowUnlockMonstermon(x);

                _locationChecker.AddCheckedLocation(MonstermonCards.CardNames[x]);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(UnlockMonstermonPatch)}.{nameof(Prefix)}:\n\t{ex}");
                return true; // run original logic
            }
        }
    }
}
