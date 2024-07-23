using System;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;
using Object = UnityEngine.Object;

namespace Archipelagarten2.HarmonyPatches.DebugPatches
{
    [HarmonyPatch(typeof(Interactable))]
    [HarmonyPatch(nameof(Interactable.GoToNextArea))]
    public static class GoToNextAreaPatch
    {
        private static ILogger _logger;

        public static void Initialize(ILogger logger)
        {
            _logger = logger;
        }

        // public void GoToNextArea()
        public static void Postfix(Interactable __instance)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(Interactable), nameof(Interactable.GoToNextArea), nameof(GoToNextAreaPatch), nameof(Postfix), Object.FindObjectOfType<WorldEventManager>().time);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(GoToNextAreaPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
