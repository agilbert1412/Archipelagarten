using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Locations;
using Archipelagarten2.Utilities;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(Principal))]
    [HarmonyPatch("SendToRecess")]
    public static class PrincipalSendToRecessPatch
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

        // private void SendToRecess()
        public static void Postfix(Principal __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(Principal), "SendToRecess", nameof(PrincipalSendToRecessPatch), nameof(Postfix));

                _locationChecker.AddCheckedLocation("Eat With The Principal");

                return;
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(PrincipalSendToRecessPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
