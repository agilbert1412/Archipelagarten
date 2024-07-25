using System;
using Archipelagarten2.HarmonyPatches.NPCPatches;
using DG.Tweening;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KG2;
using UnityEngine;
using ILogger = KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger;

namespace Archipelagarten2.UnityObjects
{
    [HarmonyPatch(typeof(NPCBehavior))]
    [HarmonyPatch(nameof(NPCBehavior.WalkToPoint), typeof(Vector3), typeof(float))]
    public static class WalkToPointPatch1
    {
        private static ILogger _logger;

        public static void Initialize(ILogger logger)
        {
            _logger = logger;
        }

        // public void WalkToPoint(Vector3 dest, float time)
        public static void Prefix(NPCBehavior __instance, Vector3 dest, float time)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(NPCBehavior), nameof(NPCBehavior.WalkToPoint), nameof(WalkToPointPatch1), nameof(Prefix), dest, time);

                _logger.LogMessage($"__instance.name: {__instance.name}");
                _logger.LogMessage($"\tdest: {dest}");
                _logger.LogMessage($"\ttime: {time}");
                _logger.LogMessage($"\t__instance.GetComponent<Collider2D>(): {__instance.GetComponent<Collider2D>()}");
                _logger.LogMessage($"\t__instance.transform: {__instance.transform}");
                _logger.LogMessage($"\t__instance.transform.localPosition: {__instance.transform.localPosition}");
                _logger.LogMessage($"\t__instance.transform.parent: {__instance.transform.parent}");
                _logger.LogMessage($"\t__instance.transform.parent.parent: {__instance.transform.parent.parent}");
                _logger.LogMessage($"\t__instance.transform.parent.parent.GetComponent<RoomEventManager>(): {__instance.transform.parent.parent.GetComponent<RoomEventManager>()}");
                _logger.LogMessage($"\t__instance.transform.parent.parent.GetComponent<RoomEventManager>().pathFinder: {__instance.transform.parent.parent.GetComponent<RoomEventManager>().pathFinder}");

                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(WalkToPointPatch1), nameof(Prefix), ex);
                return;
            }
        }
    }

    [HarmonyPatch(typeof(NPCBehavior))]
    [HarmonyPatch(nameof(NPCBehavior.WalkToPoint), typeof(Vector3), typeof(float), typeof(TweenCallback))]
    public static class WalkToPointPatch2
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
                _logger.LogDebugPatchIsRunning(nameof(NPCBehavior), nameof(NPCBehavior.WalkToPoint), nameof(WalkToPointPatch2), nameof(Prefix), dest, time, del);

                if (__instance.ReturnHeadSprite() == null)
                {
                    __instance.GetHeadSprite();
                }

                _logger.LogMessage($"__instance.name: {__instance.name}");
                _logger.LogMessage($"\tdest: {dest}");
                _logger.LogMessage($"\ttime: {time}");
                _logger.LogMessage($"\tdel: {del}");
                
                _logger.LogMessage($"\t__instance.expressions: {__instance.expressions}");
                _logger.LogMessage($"\t__instance.face: {__instance.face}");
                _logger.LogMessage($"\t__instance.faceMO: {__instance.faceMO}");
                _logger.LogMessage($"\t__instance.ReturnHeadSprite(): {__instance.ReturnHeadSprite()}");
                // _logger.LogMessage($"\t__instance.pCurrentExpression: {__instance.pCurrentExpression}");
                _logger.LogMessage($"\t__instance.GetComponent<Collider2D>(): {__instance.GetComponent<Collider2D>()}");
                _logger.LogMessage($"\t__instance.transform: {__instance.transform}");
                _logger.LogMessage($"\t__instance.transform.localPosition: {__instance.transform.localPosition}");
                _logger.LogMessage($"\t__instance.transform.parent: {__instance.transform.parent}");
                _logger.LogMessage($"\t__instance.transform.parent.parent: {__instance.transform.parent.parent}");
                _logger.LogMessage($"\t__instance.transform.parent.parent.GetComponent<RoomEventManager>(): {__instance.transform.parent.parent.GetComponent<RoomEventManager>()}");
                _logger.LogMessage($"\t__instance.transform.parent.parent.GetComponent<RoomEventManager>().pathFinder: {__instance.transform.parent.parent.GetComponent<RoomEventManager>().pathFinder}");
                _logger.LogMessage($"\t__instance.transform.parent.parent.GetComponent<RoomEventManager>().GetComponentInChildren<PathFinder>(): {__instance.transform.parent.parent.GetComponent<RoomEventManager>().GetComponentInChildren<PathFinder>()}");
                
                if (__instance.transform.parent.parent.GetComponent<RoomEventManager>().pathFinder == null)
                {
                    var path = new[] { dest };
                    __instance.WalkPath(path, time, del);
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(WalkToPointPatch2), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
