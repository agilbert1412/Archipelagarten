using System;
using System.Collections.Generic;
using BepInEx.Logging;

namespace Archipelagarten2.Archipelago
{
    public class SlotData
    {
        private const string GOAL_KEY = "goal";
        private const string SHUFFLE_MONEY_KEY = "shuffle_money";
        private const string SHUFFLE_MONSTERMON_KEY = "shuffle_monstermon";
        private const string SHUFFLE_OUTFITS_KEY = "shuffle_outfits";
        private const string DEATH_LINK_KEY = "death_link";
        private const string SEED_KEY = "seed";
        private const string MULTIWORLD_VERSION_KEY = "multiworld_version";

        private Dictionary<string, object> _slotDataFields;
        private ManualLogSource _console;

        public string SlotName { get; private set; }
        public Goal Goal { get; private set; }
        public bool ShuffleMoney { get; private set; }
        public bool ShuffleMonstermon { get; private set; }
        public bool ShuffleOutfits { get; private set; }
        public bool DeathLink { get; private set; }
        public int Seed { get; private set; }
        public string MultiworldVersion { get; private set; }

        public SlotData(string slotName, Dictionary<string, object> slotDataFields, ManualLogSource console)
        {
            SlotName = slotName;
            _slotDataFields = slotDataFields;
            _console = console;

            Goal = GetSlotSetting(GOAL_KEY, Goal.CreatureFeature);
            ShuffleMoney = GetSlotSetting(SHUFFLE_MONEY_KEY, true);
            ShuffleMonstermon = GetSlotSetting(SHUFFLE_MONSTERMON_KEY, true);
            ShuffleOutfits = GetSlotSetting(SHUFFLE_OUTFITS_KEY, false);
            DeathLink = GetSlotSetting(DEATH_LINK_KEY, false);
            Seed = GetSlotSetting(SEED_KEY, 0);
            MultiworldVersion = GetSlotSetting(MULTIWORLD_VERSION_KEY, "");
        }

        private int GetSlotSetting(IEnumerable<string> keys, int defaultValue)
        {
            foreach (var key in keys)
            {
                var value = GetSlotSetting(key, defaultValue);
                if (value != defaultValue)
                {
                    return value;
                }
            }

            return defaultValue;
        }

        private T GetSlotSetting<T>(string key, T defaultValue) where T : struct, Enum, IConvertible
        {
            return _slotDataFields.ContainsKey(key) ? (T)Enum.Parse(typeof(T), _slotDataFields[key].ToString(), true) : GetSlotDefaultValue(key, defaultValue);
        }

        private string GetSlotSetting(string key, string defaultValue)
        {
            return _slotDataFields.ContainsKey(key) ? _slotDataFields[key].ToString() : GetSlotDefaultValue(key, defaultValue);
        }

        private int GetSlotSetting(string key, int defaultValue)
        {
            return _slotDataFields.ContainsKey(key) ? (int)(long)_slotDataFields[key] : GetSlotDefaultValue(key, defaultValue);
        }

        private bool GetSlotSetting(string key, bool defaultValue)
        {
            if (_slotDataFields.ContainsKey(key) && _slotDataFields[key] != null)
            {
                if (_slotDataFields[key] is bool boolValue)
                {
                    return boolValue;
                }

                if (_slotDataFields[key] is long longValue)
                {
                    return longValue != 0;
                }

                if (_slotDataFields[key] is int intValue)
                {
                    return intValue != 0;
                }
            }

            return GetSlotDefaultValue(key, defaultValue);
        }

        private T GetSlotDefaultValue<T>(string key, T defaultValue)
        {
            _console.LogWarning($"SlotData did not contain expected key: \"{key}\"");
            return defaultValue;
        }
    }

    public enum Goal
    {
        CreatureFeature = 0,
        AllMissions = 1,
        SecretEnding = 2,
    }
}
