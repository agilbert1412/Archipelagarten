using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.UnityObjects;
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
        private static GameObjectFactory _gameObjectFactory;

        public static void Initialize(ILogger logger, KindergartenArchipelagoClient archipelago, GameObjectFactory gameObjectFactory)
        {
            _logger = logger;
            _archipelago = archipelago;
            _gameObjectFactory = gameObjectFactory;
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

        //public static void Postfix(ref int x, ref SaveData __result)
        //{
        //    try
        //    {
        //        _logger.LogDebugPatchIsRunning(nameof(SaveLoadFile), nameof(SaveLoadFile.LoadKG2), nameof(LoadKG2Patch), nameof(Postfix), x);

        //        _gameObjectFactory.CacheInstancesOfEverything();

        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogErrorException(nameof(LoadKG2Patch), nameof(Postfix), ex);
        //        return;
        //    }
        //}
    }
}
