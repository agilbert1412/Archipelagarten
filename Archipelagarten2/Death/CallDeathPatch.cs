using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Locations;
using Archipelagarten2.Utilities;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.Death
{
    [HarmonyPatch(typeof(UIController))]
    [HarmonyPatch(nameof(UIController.CallDeath), typeof(int))]
    public static class CallDeathPatch
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

        // public void CallDeath(int x)
        public static void Postfix(UIController __instance, int x)
        {
            try
            {
                if (!_archipelago.DeathLink)
                {
                    return;
                }

                DebugLogging.LogDebugPatchIsRunning(nameof(UIController), nameof(UIController.CallDeath), nameof(CallDeathPatch), nameof(Postfix), x);

                var deathMessages = DeathPanel.DeathMessages.LoadDeathMessage(__instance.deathPanel.deathXML);
                foreach (var message in deathMessages.Messages)
                {
                    _logger.LogDebug($"{message.DeathIndex}: {message.Message}");
                    if (message.DeathIndex == x)
                    {
                        _archipelago.SendDeathLink(message.Message);
                        return;
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(CallDeathPatch), nameof(Postfix), ex);
                return;
            }
        }
    }
}
