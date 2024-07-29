using System;
using System.Threading;
using System.Threading.Tasks;
using Archipelagarten2.Constants;
using Archipelagarten2.Death;
using Archipelagarten2.UnityObjects;
using HarmonyLib;
using Rewired;
using UnityEngine;
using ILogger = KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger;
using Object = UnityEngine.Object;
using PlayerController = KG2.PlayerController;

namespace Archipelagarten2.HarmonyPatches.DebugPatches
{
    [HarmonyPatch(typeof(PlayerController))]
    [HarmonyPatch("HandleInput")]
    public static class HandleInputPatch
    {
        private static ILogger _logger;
        private static UnityActions _unityActions;
        private static TrapManager _trapManager;
        private static int _lastDeath;

        public static void Initialize(ILogger logger, UnityActions unityActions, TrapManager trapManager)
        {
            _logger = logger;
            _unityActions = unityActions;
            _trapManager = trapManager;
            _lastDeath = 0;
        }

        // private void HandleInput()
        public static bool Prefix(PlayerController __instance)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(PlayerController), "HandleInput", nameof(HandleInputPatch), nameof(Prefix));
                

                if (Input.GetKeyDown(KeyCode.P))
                {
                    var thread = new Thread(KillRandomly);
                    thread.Start();
                }
                else if (Input.GetKeyDown(KeyCode.O))
                {
                    var thread = new Thread(RepeatLastKill);
                    thread.Start();
                }
                else if (Input.GetKeyDown(KeyCode.I))
                {
                    var thread = new Thread(DoJanitorTrap);
                    thread.Start();
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(HandleInputPatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }

        private static void KillRandomly()
        {
            var playerKiller = new PlayerKiller(_logger, _unityActions, true);
            _lastDeath = playerKiller.KillInRandomWay();
        }

        private static void RepeatLastKill()
        {
            var playerKiller = new PlayerKiller(_logger, _unityActions, true);
            playerKiller.KillInSpecificWay(_lastDeath);
        }

        private static void DoJanitorTrap()
        {
            _trapManager.TryHandleTrap(APItem.JANITOR_TRAP);
        }
    }
}