using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Constants;
using Archipelagarten2.Locations;
using Archipelagarten2.Utilities;
using BepInEx.Logging;
using HarmonyLib;
using KG2;

namespace Archipelagarten2.Death
{
    [HarmonyPatch(typeof(DeathPanel))]
    [HarmonyPatch(nameof(DeathPanel.SetDeathMessage))]
    public static class DeathMessagePatch
    {
        private static ManualLogSource _logger;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        private static string _playerName;

        public static void Initialize(ManualLogSource logger, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _logger = logger;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            SetPlayerName("");
        }

        public static void SetPlayerName(string playerName)
        {
            if (!string.IsNullOrWhiteSpace(playerName))
            {
                _playerName = playerName;
            }
            else
            {
                _playerName = "you";
            }
        }

        // public void SetDeathMessage(int x)
        public static bool Prefix(DeathPanel __instance, int x)
        {
            try
            {
                DebugLogging.LogDebugPatchIsRunning(nameof(DeathPanel), nameof(DeathPanel.SetDeathMessage), nameof(DeathMessagePatch), nameof(Prefix), x);

                if (x > DeathId.DEATHLINK_OFFSET)
                {
                    var deathMessages = DeathPanel.DeathMessages.LoadDeathMessage(__instance.deathXML);
                    __instance.youDiedText.text = deathMessages.TitleText;
                    __instance.restartCurrentText.text = deathMessages.RestartCurrentText;
                    __instance.quitToMenuText.text = deathMessages.QuitText;
                    foreach (var message in deathMessages.Messages)
                    {
                        if (message.DeathIndex == x - DeathId.DEATHLINK_OFFSET)
                        {
                            __instance.deathMessage.text = message.Message
                                .Replace("You", _playerName)
                                .Replace("you", _playerName)
                                .Replace("your", $"{_playerName}'s");
                        }
                    }

                    AudioController.instance.PlaySound("DeathTone");

                    return false; // don't run original logic
                }

                if (x == DeathId.JANITOR_TRAP)
                {
                    var deathMessages = DeathPanel.DeathMessages.LoadDeathMessage(__instance.deathXML);
                    __instance.youDiedText.text = deathMessages.TitleText;
                    __instance.restartCurrentText.text = deathMessages.RestartCurrentText;
                    __instance.quitToMenuText.text = deathMessages.QuitText;
                    __instance.deathMessage.text = "You should stay a mop's length away from the multiworld";
                    AudioController.instance.PlaySound("DeathTone");

                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(nameof(DeathMessagePatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
