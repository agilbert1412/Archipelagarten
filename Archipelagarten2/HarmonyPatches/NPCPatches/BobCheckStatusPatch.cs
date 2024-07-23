using System;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(Bob))]
    [HarmonyPatch(nameof(Bob.CheckStatus))]
    public static class BobCheckStatusPatch
    {
        private static ILogger _logger;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(ILogger logger, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _logger = logger;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public override void CheckStatus(TimeOfDay t)
        public static void Postfix(Bob __instance, TimeOfDay t)
        {
            try
            {
                var janitorGaveContract = EnvironmentController.Instance.ContainsFlag(Flag.JanitorGaveContractToBob);
                _logger.LogDebugPatchIsRunning(nameof(Bob), nameof(Bob.CheckStatus), nameof(BobCheckStatusPatch), nameof(Postfix), t, janitorGaveContract);

                if (janitorGaveContract)
                {
                    _locationChecker.AddCheckedLocation("Declare War On Bob");
                }

                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(BobCheckStatusPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
