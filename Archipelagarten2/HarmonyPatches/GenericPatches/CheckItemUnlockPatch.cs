using System;
using System.Collections;
using System.Collections.Generic;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Constants;
using Archipelagarten2.Locations;
using Archipelagarten2.Utilities;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.GenericPatches
{
    [HarmonyPatch(typeof(MissionButton))]
    [HarmonyPatch("CheckItemUnlock")]
    public static class CheckItemUnlockPatch
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

        // private bool CheckItemUnlock()
        public static bool Prefix(MissionButton __instance, ref bool __result)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(MissionButton), "CheckItemUnlock", nameof(CheckItemUnlockPatch), nameof(Prefix));

                _logger.LogInfo($"__instance.itemToUnlock: {__instance.itemToUnlock}");

                if (Missions.ItemToMissionMap.ContainsKey(__instance.itemToUnlock))
                {
                    var missionName = Missions.ItemToMissionMap[__instance.itemToUnlock];
                    _logger.LogInfo($"missionName: {missionName}");
                    var locationExists = _archipelago.LocationExists(missionName);
                    var locationNotChecked = _locationChecker.IsLocationNotChecked(missionName);
                    var locationMissing = locationExists && locationNotChecked;
                    _logger.LogInfo($"locationExists: {locationExists}");
                    _logger.LogInfo($"locationNotChecked: {locationNotChecked}");
                    _logger.LogInfo($"locationMissing: {locationMissing}");
                    __result = !locationMissing;
                }
                else
                {
                    __result = false;
                }

                _logger.LogInfo($"__result: {__result}");
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(CheckItemUnlockPatch), nameof(Prefix), ex);
                return true;
            }
        }
    }
}