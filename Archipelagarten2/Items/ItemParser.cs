using System;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Constants;
using KG2;

namespace Archipelagarten2.Items
{
    public class ItemParser
    {
        private ArchipelagoClient _archipelago;

        public ItemParser(ArchipelagoClient archipelago)
        {
            _archipelago = archipelago;
        }

        public void ProcessItem(ReceivedItem item)
        {
            ProcessItem(item.ItemName);
        }

        public void ProcessItem(string itemName)
        {
            if (TryHandleMoney(itemName))
            {
                return;
            }
        }

        private bool TryHandleMoney(string itemName)
        {
            if (!itemName.Equals(APItem.MONEY, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (EnvironmentController.Instance == null || EnvironmentController.Instance.saves == null)
            {
                return true;
            }

            var saves = EnvironmentController.Instance.saves;
            foreach (var saveAtTime in saves)
            {
                saveAtTime.Value.money += APItem.MONEY_AMOUNT;
            }

            EnvironmentController.Instance.GetMoney(APItem.MONEY_AMOUNT);

            return true;
        }
    }
}