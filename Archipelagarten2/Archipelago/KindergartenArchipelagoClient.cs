using System;
using System.Collections.Generic;
using System.Linq;
using Archipelagarten2.Characters;
using Archipelagarten2.Death;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;

namespace Archipelagarten2.Archipelago
{
    public class KindergartenArchipelagoClient : ArchipelagoClient
    {
        private readonly CharacterActions _characterActions;

        public override string GameName => "Kindergarten 2";
        public override string ModName => "Archipelagarten2";
        public override string ModVersion => MyPluginInfo.PLUGIN_VERSION;

        public SlotData SlotData => (SlotData)_slotData;

        public KindergartenArchipelagoClient(ILogger logger, CharacterActions characterActions, Action itemReceivedFunction) : 
            base(logger, new DataPackageCache("kindergarten_2", "BepInEx", "plugins", "Archipelagarten", "IdTables"), itemReceivedFunction)
        {
            _characterActions = characterActions;
        }

        protected override void InitializeSlotData(string slotName, Dictionary<string, object> slotDataFields)
        {
            _slotData = new SlotData(slotName, slotDataFields, Logger);
        }

        protected override void OnMessageReceived(LogMessage message)
        {
            var fullMessage = string.Join(" ", message.Parts.Select(str => str.Text));
            Logger.LogInfo(fullMessage);
        }

        protected override void KillPlayerDeathLink(DeathLink deathLink)
        {
            DeathMessagePatch.SetPlayerName(deathLink.Source);
            var deathLinkPlayerKiller = new PlayerKiller(Logger, _characterActions, true);
            deathLinkPlayerKiller.KillInSpecificWay(deathLink.Cause);
        }
    }
}
