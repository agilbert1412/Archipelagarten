using System;
using Archipelagarten2.UnityObjects;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.Death
{
    [HarmonyPatch(typeof(RoomEventManager))]
    [HarmonyPatch(nameof(RoomEventManager.RunStartFunction))]
    public static class BedroomStartPatch
    {
        private static ILogger _logger;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static GameObjectFactory _gameObjectFactory;

        public static void Initialize(ILogger logger, ArchipelagoClient archipelago, LocationChecker locationChecker, GameObjectFactory gameObjectFactory)
        {
            _logger = logger;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _gameObjectFactory = gameObjectFactory;
        }

        // public void RunStartFunction()
        public static void Postfix(RoomEventManager __instance)
        {
            try
            {
                var player = UnityEngine.Object.FindObjectOfType<PlayerController>();

                _logger.LogDebugPatchIsRunning(nameof(RoomEventManager), nameof(RoomEventManager.RunStartFunction), nameof(BedroomStartPatch), nameof(Postfix), __instance.room, player?.GetPlayerState());

                _gameObjectFactory.CacheInstancesOfEverythingInCurrentScene();

                if (__instance.room == Room.Bedroom && player != null && player.GetPlayerState() != PlayerState.WalkState)
                {
                    player.SetPlayerState(PlayerState.WalkState);
                }

                return;
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(BedroomStartPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
