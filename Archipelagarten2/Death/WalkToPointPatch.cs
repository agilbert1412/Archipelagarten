using System;
using DG.Tweening;
using HarmonyLib;
using KG2;
using UnityEngine;
using ILogger = KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger;

namespace Archipelagarten2.Death
{
    [HarmonyPatch(typeof(NPCBehavior))]
    [HarmonyPatch(nameof(NPCBehavior.WalkToPoint), typeof(Vector3), typeof(float), typeof(TweenCallback))]
    public static class WalkToPointPatch
    {
        private static ILogger _logger;

        public static void Initialize(ILogger logger)
        {
            _logger = logger;
        }

        // public void WalkToPoint(Vector3 dest, float time, TweenCallback del)
        public static bool Prefix(NPCBehavior __instance, Vector3 dest, float time, TweenCallback del)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(NPCBehavior), nameof(NPCBehavior.WalkToPoint), nameof(WalkToPointPatch), nameof(Prefix), dest, time, del);

                if (__instance.transform.parent.parent.GetComponent<RoomEventManager>().pathFinder == null)
                {
                    __instance.WalkStraightLine(dest, time, del);
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(WalkToPointPatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
