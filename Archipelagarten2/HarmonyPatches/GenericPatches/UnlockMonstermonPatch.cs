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
    [HarmonyPatch(nameof(EnvironmentController.UnlockMonstermon))]
    public static class UnlockMonstermonPatch
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

        // public void UnlockMonstermon(int x)
        public static bool Prefix(EnvironmentController __instance, int x)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(EnvironmentController), nameof(EnvironmentController.UnlockMonstermon), nameof(UnlockMonstermonPatch), nameof(Prefix));

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
                _logger.LogErrorException(nameof(UnlockMonstermonPatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
