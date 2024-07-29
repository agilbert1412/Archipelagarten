using System.Collections.Generic;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Death;
using Archipelagarten2.UnityObjects;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;

namespace Archipelagarten2.Items
{
    public class KindergartenItemManager : ItemManager
    {
        private KindergartenArchipelagoClient _archipelago;
        private ItemParser _itemParser;

        public KindergartenItemManager(ILogger logger, KindergartenArchipelagoClient archipelago, UnityActions characterActions, TrapManager trapManager, IEnumerable<ReceivedItem> itemsAlreadyProcessed) : base(archipelago, itemsAlreadyProcessed)
        {
            _archipelago = archipelago;
            _itemParser = new ItemParser(logger, archipelago.SlotData, characterActions, trapManager);
        }

        protected override void ProcessItem(ReceivedItem receivedItem, bool immediatelyIfPossible)
        {
            _itemParser.ProcessItem(receivedItem);
        }

        public void UpdateItemsAlreadyProcessed()
        {
            var allReceivedItems = _archipelago.GetAllReceivedItems();
            _itemsAlreadyProcessed = new();

            foreach (var receivedItem in allReceivedItems)
            {
                _itemsAlreadyProcessed.Add(receivedItem);
            }
        }
    }
}
