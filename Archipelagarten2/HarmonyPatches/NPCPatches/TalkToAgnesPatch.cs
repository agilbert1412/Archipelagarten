using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Locations;
using Archipelagarten2.Patching;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(Agnes))]
    [HarmonyPatch(nameof(Agnes.Interact))]
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
        public static void Postfix(Agnes __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(Agnes), nameof(Agnes.Interact), nameof(TalkToAgnesPatch), nameof(Postfix));
                
                _locationChecker.AddCheckedLocation("Meet A Dumpster Hag");

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(TalkToAgnesPatch)}.{nameof(Postfix)}:\n\t{ex}");
                return;
            }
        }
    }
}
