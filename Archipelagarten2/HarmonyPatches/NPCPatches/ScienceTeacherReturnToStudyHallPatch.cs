using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Locations;
using Archipelagarten2.Utilities;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.HarmonyPatches.NPCPatches
{
    [HarmonyPatch(typeof(ScienceTeacher))]
    [HarmonyPatch(nameof(ScienceTeacher.ReturnToStudyHall))]
    public static class ScienceTeacherReturnToStudyHallPatch
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

        // public void ReturnToStudyHall()
        public static void Postfix(ScienceTeacher __instance)
        {
            try
            {
                var playerWasInStudyHall = EnvironmentController.Instance.ContainsFlag(Flag.ForceToStudyHall);
                var playerPosition = __instance.player.transform.localPosition;
                var xIsCorrect = playerPosition.x > 0.44999998807907104 && playerPosition.x < 0.64999997615814209;
                var yIsCorrect = playerPosition.y > -0.699999988079071 && playerPosition.y < -0.34999999403953552;
                var playerIsInCorrectSpot = xIsCorrect && yIsCorrect;
                DebugLogging.LogDebugPatchIsRunning(nameof(ScienceTeacher), nameof(ScienceTeacher.ReturnToStudyHall), nameof(ScienceTeacherReturnToStudyHallPatch), nameof(Postfix), playerWasInStudyHall, playerIsInCorrectSpot);

                if (playerWasInStudyHall && playerIsInCorrectSpot)
                {
                    _locationChecker.AddCheckedLocation("Experience Study Hall");
                }

                return;
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(ScienceTeacherReturnToStudyHallPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
