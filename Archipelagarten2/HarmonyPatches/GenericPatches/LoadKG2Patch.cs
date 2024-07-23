using System;
using Archipelagarten2.Archipelago;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.GenericPatches
{
    [HarmonyPatch(typeof(SaveLoadFile))]
    [HarmonyPatch(nameof(SaveLoadFile.LoadKG2))]
    public static class LoadKG2Patch
    {
        private static ILogger _logger;
        private static KindergartenArchipelagoClient _archipelago;

        public static void Initialize(ILogger logger, KindergartenArchipelagoClient archipelago)
        {
            _logger = logger;
            _archipelago = archipelago;
        }

        // public static KG2.SaveData LoadKG2(int x)
        public static bool Prefix(ref int x, ref SaveData __result)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(SaveLoadFile), nameof(SaveLoadFile.LoadKG2), nameof(LoadKG2Patch), nameof(Prefix), x);

                x = _archipelago.SlotData.Seed;

                return true; // run original logic;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(LoadKG2Patch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
