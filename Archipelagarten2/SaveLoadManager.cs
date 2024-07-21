using Archipelagarten2.Archipelago;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Archipelagarten2
{
    public class SaveLoadManager
    {
        private ManualLogSource _logger;
        private Harmony _harmony;
        private ArchipelagoClient _archipelago;

        public SaveLoadManager(ManualLogSource logger, Harmony harmony, ArchipelagoClient archipelago)
        {
            _logger = logger;
            _harmony = harmony;
            _archipelago = archipelago;
        }
    }
}
