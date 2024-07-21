using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Locations;
using Archipelagarten2.Utilities;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(NPCBehavior))]
    [HarmonyPatch(nameof(NPCBehavior.Interact))]
    public static class TalkToAgnesPatch
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

        // public override void Interact()
        public static void Postfix(NPCBehavior __instance)
        {
            try
            {
                if (__instance is not Agnes agnes)
                {
                    return;
                }

                DebugLogging.LogDebugPatchIsRunning(nameof(NPCBehavior), nameof(NPCBehavior.Interact), nameof(TalkToAgnesPatch), nameof(Postfix));
                
                _locationChecker.AddCheckedLocation("Meet A Dumpster Hag");

                return;
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(TalkToAgnesPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
