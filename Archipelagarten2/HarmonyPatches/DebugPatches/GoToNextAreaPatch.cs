using System;
using Archipelagarten2.Utilities;
using BepInEx.Logging;
using HarmonyLib;
using KG2;
using Object = UnityEngine.Object;

namespace Archipelagarten2.HarmonyPatches.DebugPatches
{
    [HarmonyPatch(typeof(Interactable))]
    [HarmonyPatch(nameof(Interactable.GoToNextArea))]
    public static class GoToNextAreaPatch
    {
        private static ManualLogSource _logger;

        public static void Initialize(ManualLogSource logger)
        {
            _logger = logger;
        }

        // public void GoToNextArea()
        public static void Postfix(Interactable __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(Interactable), nameof(Interactable.GoToNextArea), nameof(GoToNextAreaPatch), nameof(Postfix), Object.FindObjectOfType<WorldEventManager>().time);
                return;
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(GoToNextAreaPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
