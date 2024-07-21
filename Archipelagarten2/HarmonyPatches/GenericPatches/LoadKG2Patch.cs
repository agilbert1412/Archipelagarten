using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Constants;
using Archipelagarten2.Utilities;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.GenericPatches
{
    [HarmonyPatch(typeof(SaveLoadFile))]
    [HarmonyPatch(nameof(SaveLoadFile.LoadKG2))]
    public static class LoadKG2Patch
    {
        private static ManualLogSource _logger;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(ManualLogSource logger, ArchipelagoClient archipelago)
        {
            _logger = logger;
            _archipelago = archipelago;
        }

        // public static KG2.SaveData LoadKG2(int x)
        public static bool Prefix(ref int x, ref SaveData __result)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(SaveLoadFile), nameof(SaveLoadFile.LoadKG2), nameof(LoadKG2Patch), nameof(Prefix), x);

                x = _archipelago.SlotData.Seed;

                return true; // run original logic;
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(LoadKG2Patch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
