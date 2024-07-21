using System.Collections.Generic;
using Archipelagarten2.Archipelago;
using BepInEx.Logging;

namespace Archipelagarten2.Items
{
    public class ItemManager
    {
        private ManualLogSource _log;
        private ArchipelagoClient _archipelago;

        // private ArchipelagoNotificationsHandler _notificationHandler;
        private ItemParser ItemParser { get; }
        private HashSet<ReceivedItem> _itemsAlreadyProcessedThisRun;

        public ItemManager(ManualLogSource log, ArchipelagoClient archipelago)
        {
            _log = log;
            _archipelago = archipelago;
            // _notificationHandler = notificationHandler;

            ItemParser = new ItemParser(archipelago);
            _itemsAlreadyProcessedThisRun = new HashSet<ReceivedItem>();
        }

        public void UpdateItemsAlreadyProcessed()
        {
            var allReceivedItems = _archipelago.GetAllReceivedItems();
            _itemsAlreadyProcessedThisRun = new HashSet<ReceivedItem>();

            foreach (var receivedItem in allReceivedItems)
            {
                _itemsAlreadyProcessedThisRun.Add(receivedItem);
            }
        }

        public void ReceiveAllNewItems()
        {
            var allReceivedItems = _archipelago.GetAllReceivedItems();

            foreach (var receivedItem in allReceivedItems)
            {
                ReceiveNewItem(receivedItem);
            }
        }

        private void ReceiveNewItem(ReceivedItem receivedItem)
        {
            if (_itemsAlreadyProcessedThisRun.Contains(receivedItem))
            {
                return;
            }
            
            ItemParser.ProcessItem(receivedItem);
            _itemsAlreadyProcessedThisRun.Add(receivedItem);
            
            _log.LogMessage($"Item Received: {receivedItem.ItemName}");
            // _notificationHandler.AddItemNotification(receivedItem.ItemName);
        }
    }
}
