using System;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(NPCBehavior))]
    [HarmonyPatch(nameof(NPCBehavior.Interact))]
    public static class TalkToAgnesPatch
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

        // public override void Interact()
        public static void Postfix(NPCBehavior __instance)
        {
            try
            {
                if (__instance is not Agnes agnes)
                {
                    return;
                }

                _logger.LogDebugPatchIsRunning(nameof(NPCBehavior), nameof(NPCBehavior.Interact), nameof(TalkToAgnesPatch), nameof(Postfix));
                
                _locationChecker.AddCheckedLocation("Meet A Dumpster Hag");

                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(TalkToAgnesPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
