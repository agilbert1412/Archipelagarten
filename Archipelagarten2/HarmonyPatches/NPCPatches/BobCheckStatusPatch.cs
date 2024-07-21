using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Locations;
using Archipelagarten2.Patching;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(Bob))]
    [HarmonyPatch(nameof(Bob.CheckStatus))]
    public static class BobCheckStatusPatch
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

        // public override void CheckStatus(TimeOfDay t)
        public static void Postfix(Bob __instance, TimeOfDay t)
        {
            try
            {
                var janitorGaveContract = EnvironmentController.Instance.ContainsFlag(Flag.JanitorGaveContractToBob);
                DebugLogging.LogDebugPatchIsRunning(nameof(Bob), nameof(Bob.CheckStatus), nameof(BobCheckStatusPatch), nameof(Postfix), t, janitorGaveContract);

                if (janitorGaveContract)
                {
                    _locationChecker.AddCheckedLocation("Declare War On Bob");
                }

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(BobCheckStatusPatch)}.{nameof(Postfix)}:\n\t{ex}");
                return;
            }
        }
    }
}
