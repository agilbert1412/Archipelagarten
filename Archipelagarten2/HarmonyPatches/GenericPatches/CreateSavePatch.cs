using System;
using System.Linq;
using Archipelagarten2.Items;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.GenericPatches
{
    [HarmonyPatch(typeof(EnvironmentController))]
    [HarmonyPatch(nameof(EnvironmentController.CreateSave))]
    public static class CreateSavePatch
    {
        private static ILogger _logger;
        private static ArchipelagoClient _archipelago;
        private static GameStateWriter _gameStateWriter;

        public static void Initialize(ILogger logger, ArchipelagoClient archipelago, GameStateWriter gameStateWriter)
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
                _logger.LogDebugPatchIsRunning(nameof(EnvironmentController), nameof(EnvironmentController.CreateSave), nameof(CreateSavePatch), nameof(Postfix), string.Join(", ", __instance.saves.Keys.Select(x => x.ToString())));

                if (__instance.saves == null || __instance.saves.Count != 1 || !__instance.saves.ContainsKey(TimeOfDay.BedroomTime))
                {
                    return;
                }

                _gameStateWriter.SetGameStateToArchipelagoState(__instance);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(CreateSavePatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
