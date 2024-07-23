using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Constants;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.HarmonyPatches.GenericPatches
{
    [HarmonyPatch(typeof(EndDayPanel))]
    [HarmonyPatch("Start")]
    public static class EndDayPanelPatch
    {
        private static ILogger _logger;
        private static KindergartenArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(ILogger logger, KindergartenArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _logger = logger;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // private IEnumerator Start()
        public static void Postfix(EndDayPanel __instance, ref IEnumerator __result)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(EndDayPanel), "Start", nameof(EndDayPanelPatch), nameof(Postfix));

                var finishedChecks = GetEndOfDayChecksToSend();

                foreach (var finishedCheck in finishedChecks)
                {
                    _locationChecker.AddCheckedLocation(finishedCheck);
                }

                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(EndDayPanelPatch), nameof(Postfix), ex);
                return;
            }
        }

        private static IEnumerable<string> GetEndOfDayChecksToSend()
        {
            yield return "Survive Tuesday";
            switch (EnvironmentController.Instance.unlockedItem)
            {
                case Item.APlus:
                    yield return Missions.FLOWERS_FOR_DIANA;
                    break;
                case Item.ApplesoftPin:
                    yield return Missions.HITMAN_GUARD;
                    break;
                case Item.Chemical:
                    yield return Missions.CAIN_NOT_ABLE;
                    if (EnvironmentController.Instance.ContainsFlag(Flag.TedInOnIt))
                    {
                        yield return "Kill Felix";
                    }

                    break;
                case Item.Deck:
                    yield return Missions.CREATURE_FEATURE;
                    if (_archipelago.SlotData.Goal == Goal.CreatureFeature)
                    {
                        _archipelago.ReportGoalCompletion();
                    }

                    break;
                case Item.LaserCutter:
                    yield return Missions.OPPOSITES_ATTRACT;
                    break;
                case Item.MonstermonPlushie:
                    yield return Missions.DODGE_A_NUGGET;
                    break;
                case Item.PennyController:
                    yield return Missions.BREAKING_SAD;
                    break;
                case Item.ToolBelt:
                    yield return Missions.TALE_OF_JANITORS;
                    break;
                case Item.UltraBomb:
                    yield return Missions.THINGS_GO_BOOM;
                    break;
                default:
                    break;
            }

            if (_archipelago.SlotData.Goal == Goal.AllMissions)
            {
                var allMissionsComplete = Missions.ALL_MISSIONS.All(x => _locationChecker.IsLocationChecked(x));
                if (allMissionsComplete)
                {
                    _archipelago.ReportGoalCompletion();
                }
            }
        }
    }
}
