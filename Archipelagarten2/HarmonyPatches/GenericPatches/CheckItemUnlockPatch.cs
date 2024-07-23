using System;
using Archipelagarten2.Constants;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.GenericPatches
{
    [HarmonyPatch(typeof(MissionButton))]
    [HarmonyPatch("CheckItemUnlock")]
    public static class CheckItemUnlockPatch
    {
        private static ILogger _logger;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(ILogger logger, ArchipelagoClient archipelago, LocationChecker locationChecker)
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
                _logger.LogDebugPatchIsRunning(nameof(MissionButton), "CheckItemUnlock", nameof(CheckItemUnlockPatch), nameof(Prefix));

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
                _logger.LogErrorException(nameof(CheckItemUnlockPatch), nameof(Prefix), ex);
                return true;
            }
        }
    }
}