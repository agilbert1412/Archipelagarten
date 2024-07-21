using System;
using System.Collections;
using System.Collections.Generic;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Locations;
using Archipelagarten2.Patching;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.GenericPatches
{
    [HarmonyPatch(typeof(Nugget))]
    [HarmonyPatch("EnterSanctum")]
    public static class EnterSanctumPatch
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

        // private void EnterSanctum()
        public static void Postfix(Nugget __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(Nugget), "EnterSanctum", nameof(EnterSanctumPatch), nameof(Postfix));

                _locationChecker.AddCheckedLocation("Secret Ending");
                if (_archipelago.SlotData.Goal == Goal.SecretEnding)
                {
                    _archipelago.ReportGoalCompletion();
                }

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(EnterSanctumPatch)}.{nameof(Postfix)}:\n\t{ex}");
                return;
            }
        }
    }
}
