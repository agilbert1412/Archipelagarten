using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Items;
using Archipelagarten2.Locations;
using Archipelagarten2.Utilities;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.GenericPatches
{
    [HarmonyPatch(typeof(EnvironmentController))]
    [HarmonyPatch(nameof(EnvironmentController.CreateSave))]
    public static class CreateSavePatch
    {
        private static ManualLogSource _logger;
        private static ArchipelagoClient _archipelago;
        private static GameStateWriter _gameStateWriter;

        public static void Initialize(ManualLogSource logger, ArchipelagoClient archipelago, GameStateWriter gameStateWriter)
        {
            _logger = logger;
            _archipelago = archipelago;
            _gameStateWriter = gameStateWriter;
        }

        // public void CreateSave()
        public static void Postfix(EnvironmentController __instance)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(EnvironmentController), nameof(EnvironmentController.CreateSave), nameof(CreateSavePatch), nameof(Postfix), string.Join(", ", __instance.saves.Keys.Select(x => x.ToString())));

                if (__instance.saves == null || __instance.saves.Count != 1 || !__instance.saves.ContainsKey(TimeOfDay.BedroomTime))
                {
                    return;
                }

                _gameStateWriter.SetGameStateToArchipelagoState(__instance);
                return;
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(CreateSavePatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
