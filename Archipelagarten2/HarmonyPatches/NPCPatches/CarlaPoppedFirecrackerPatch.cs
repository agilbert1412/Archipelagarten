using System;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(Carla))]
    [HarmonyPatch("GiveFirecrackerPopped")]
    public static class CarlaPoppedFirecrackerPatch
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

        // private void GiveFirecrackerPopped()
        public static void Postfix(Carla __instance)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(Carla), "GiveFirecrackerPopped", nameof(CarlaPoppedFirecrackerPatch), nameof(Postfix));
                
                _locationChecker.AddCheckedLocation("Win A Bet With Carla");

                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(CarlaPoppedFirecrackerPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
