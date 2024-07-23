using System;
using Archipelagarten2.Archipelago;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.GenericPatches
{
    [HarmonyPatch(typeof(Nugget))]
    [HarmonyPatch("EnterSanctum")]
    public static class EnterSanctumPatch
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

        // private void EnterSanctum()
        public static void Postfix(Nugget __instance)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(Nugget), "EnterSanctum", nameof(EnterSanctumPatch), nameof(Postfix));

                _locationChecker.AddCheckedLocation("Secret Ending");
                if (_archipelago.SlotData.Goal == Goal.SecretEnding)
                {
                    _archipelago.ReportGoalCompletion();
                }

                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(EnterSanctumPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
