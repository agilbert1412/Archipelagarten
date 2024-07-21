using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Locations;
using Archipelagarten2.Patching;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.GenericPatches
{
    [HarmonyPatch(typeof(EnvironmentController))]
    [HarmonyPatch(nameof(EnvironmentController.AddFlag))]
    public static class AddFlagPatch
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

        // public void AddFlag(Flag flag)
        public static void Postfix(EnvironmentController __instance, Flag flag)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(EnvironmentController), nameof(EnvironmentController.AddFlag), nameof(AddFlagPatch), nameof(Postfix));

                if (flag is Flag.UsingStall1 or Flag.UsingStall2)
                {
                    _locationChecker.AddCheckedLocation("Use A Toilet");
                }

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(AddFlagPatch)}.{nameof(Postfix)}:\n\t{ex}");
                return;
            }
        }
    }
}
