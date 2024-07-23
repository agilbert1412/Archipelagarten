using System;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.GenericPatches
{
    [HarmonyPatch(typeof(EnvironmentController))]
    [HarmonyPatch(nameof(EnvironmentController.AddFlag))]
    public static class AddFlagPatch
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

        // public void AddFlag(Flag flag)
        public static void Postfix(EnvironmentController __instance, Flag flag)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(EnvironmentController), nameof(EnvironmentController.AddFlag), nameof(AddFlagPatch), nameof(Postfix));

                if (flag is Flag.UsingStall1 or Flag.UsingStall2)
                {
                    _locationChecker.AddCheckedLocation("Use A Toilet");
                }

                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(AddFlagPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
