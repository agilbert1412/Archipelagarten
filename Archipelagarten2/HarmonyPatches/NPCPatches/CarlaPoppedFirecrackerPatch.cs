using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Locations;
using Archipelagarten2.Utilities;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(Carla))]
    [HarmonyPatch("GiveFirecrackerPopped")]
    public static class CarlaPoppedFirecrackerPatch
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

        // private void GiveFirecrackerPopped()
        public static void Postfix(Carla __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(Carla), "GiveFirecrackerPopped", nameof(CarlaPoppedFirecrackerPatch), nameof(Postfix));
                
                _locationChecker.AddCheckedLocation("Win A Bet With Carla");

                return;
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(CarlaPoppedFirecrackerPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
