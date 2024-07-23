using System;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(Principal))]
    [HarmonyPatch("SendToRecess")]
    public static class PrincipalSendToRecessPatch
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

        // private void SendToRecess()
        public static void Postfix(Principal __instance)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(Principal), "SendToRecess", nameof(PrincipalSendToRecessPatch), nameof(Postfix));

                _locationChecker.AddCheckedLocation("Eat With The Principal");

                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(PrincipalSendToRecessPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
