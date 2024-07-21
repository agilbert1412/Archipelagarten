using Archipelagarten2.Archipelago;
using Archipelagarten2.Constants;
using KG2;
using System;

namespace Archipelagarten2.Items
{
    public class GameStateWriter
    {
        private ArchipelagoClient _archipelago;

        public GameStateWriter(ArchipelagoClient archipelago)
        {
            _archipelago = archipelago;
        }

        public void SetGameStateToArchipelagoState(EnvironmentController environment)
        {
            environment.saveFile.SaveSlot = _archipelago.SlotData.Seed;

            SetInventoryItemUnlocks(environment);
            SetMoney(environment);
            SetMonstermonCards(environment);
            SetOutfits(environment);

            environment.saves.Clear();
            var saveState = new EnvironmentController.SaveState(environment.flags, environment.inventory, environment.money, TimeOfDay.BedroomTime);
            environment.saves.Add(TimeOfDay.BedroomTime, saveState);
        }

        private void SetInventoryItemUnlocks(EnvironmentController environment)
        {
            environment.saveFile.UnlockedToolBelt = _archipelago.HasReceivedItem("Bob's Toolbelt");
            environment.saveFile.UnlockedAPlus = _archipelago.HasReceivedItem("An A+");
            environment.saveFile.UnlockedPin = _archipelago.HasReceivedItem("Prestigious Pin");
            environment.saveFile.UnlockedChemical = _archipelago.HasReceivedItem("Felix's Strange Chemical");
            environment.saveFile.UnlockedLaser = _archipelago.HasReceivedItem("Penny's Laser Beam");
            environment.saveFile.UnlockedPlushie = _archipelago.HasReceivedItem("A Monstermon Plushie");
            environment.saveFile.UnlockedBomb = _archipelago.HasReceivedItem("Carla's Laser Bomb");
            environment.saveFile.UnlockedRemote = _archipelago.HasReceivedItem("Faculty Remote");
        }

        private void SetMoney(EnvironmentController environment)
        {
            if (_archipelago.SlotData.ShuffleMoney > 0)
            {
                environment.money = GetReceivedMoney(_archipelago.SlotData.ShuffleMoney);
                EnvironmentController.Instance.GetMoney(0);
            }
            else
            {
                EnvironmentController.Instance.GetMoney(GetReceivedMoney(1));
            }
        }

        private float GetReceivedMoney(int shuffleMoneyValue)
        {
            var receivedMoney = _archipelago.GetReceivedItemCount(APItem.MONEY);
            var receivedPocketChange = _archipelago.GetReceivedItemCount(APItem.POCKET_CHANGE);
            var moneyPerMoney = shuffleMoneyValue;
            var moneyPerPocketChange = moneyPerMoney * APItem.POCKET_CHANGE_MULTIPLIER;
            return (receivedMoney * moneyPerMoney) + (receivedPocketChange * moneyPerPocketChange);
        }

        private void SetMonstermonCards(EnvironmentController environment)
        {
            if (!_archipelago.SlotData.ShuffleMonstermon)
            {
                return;
            }

            for (var i = 0; i < environment.saveFile.Monstermon.Length; i++)
            {
                environment.saveFile.Monstermon[i] = _archipelago.HasReceivedItem(MonstermonCards.CardNames[i]);
            }
        }

        private void SetOutfits(EnvironmentController environment)
        {
            if (!_archipelago.SlotData.ShuffleOutfits)
            {
                return;
            }

            for (var i = 0; i < environment.saveFile.Outfits.Length; i++)
            {
                environment.saveFile.Outfits[i] = _archipelago.HasReceivedItem(Outfits.OutfitNames[i]);
            }

            var firstUnlockedOutfit = GetFirstUnlockedOutfit(environment);
            if (!environment.saveFile.Outfits[environment.saveFile.CurrentHairStyle])
            {
                environment.saveFile.CurrentHairStyle = firstUnlockedOutfit;
            }

            if (!environment.saveFile.Outfits[environment.saveFile.CurrentOutfit])
            {
                environment.saveFile.CurrentOutfit = firstUnlockedOutfit;
            }
        }

        private static int GetFirstUnlockedOutfit(EnvironmentController environment)
        {
            for (var i = 0; i < environment.saveFile.Outfits.Length; i++)
            {
                if (environment.saveFile.Outfits[i])
                {
                    return i;
                }
            }

            return 0;
        }
    }
}