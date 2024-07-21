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
            if (EnvironmentController.Instance == null || EnvironmentController.Instance.saves == null)
            {
                return;
            }

            if (TryHandleMoney(itemName))
            {
                return;
            }
            if (TryHandlePocketChange(itemName))
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

            var moneyAmount = Math.Max(1, _archipelago.SlotData.ShuffleMoney);
            AddMoney(moneyAmount);

            return true;
        }

        private bool TryHandlePocketChange(string itemName)
        {
            if (!itemName.Equals(APItem.POCKET_CHANGE, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            var moneyAmount = Math.Max(1, _archipelago.SlotData.ShuffleMoney) * APItem.POCKET_CHANGE_MULTIPLIER;
            AddMoney(moneyAmount);

            return true;
        }

        private static void AddMoney(float moneyAmount)
        {
            var saves = EnvironmentController.Instance.saves;
            foreach (var saveAtTime in saves)
            {
                saveAtTime.Value.money += moneyAmount;
            }

            EnvironmentController.Instance.GetMoney(moneyAmount);
        }
    }
}