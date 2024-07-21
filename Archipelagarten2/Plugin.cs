using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Archipelagarten2.Archipelago;
using Archipelagarten2.HarmonyPatches;
using Archipelagarten2.Items;
using Archipelagarten2.Locations;
using Archipelagarten2.Serialization;
using Archipelagarten2.Utilities;
using BepInEx;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;

namespace Archipelagarten2
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;

        private PatchInitializer _patcherInitializer;
        private Harmony _harmony;
        private SaveLoadManager _saveLoadManager;
        private ArchipelagoClient _archipelago;
        private ArchipelagoConnectionInfo APConnectionInfo { get; set; }
        private LocationChecker _locationChecker;
        private ItemManager _itemManager;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Loading {MyPluginInfo.PLUGIN_GUID}...");

            Debugger.Break();

            try
            {
                TypeAccess.Initialize(Logger);
                DebugLogging.Initialize(Logger);
                _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
                _harmony.PatchAll();
            }
            catch (FileNotFoundException fnfe)
            {
                Logger.LogError($"Cannot load {MyPluginInfo.PLUGIN_GUID}: A Necessary Dependency is missing [{fnfe.FileName}]");
                throw;
            }

            Initialize();
            ConnectToArchipelago();

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void Initialize()
        {
            _patcherInitializer = new PatchInitializer();
            _archipelago = new ArchipelagoClient(Logger, _harmony, OnItemReceived);
            _saveLoadManager = new SaveLoadManager(Logger, _harmony, _archipelago);
            _locationChecker = new LocationChecker(Logger, _archipelago, new List<string>());
            _itemManager = new ItemManager(Logger, _archipelago);
            _patcherInitializer.InitializeAllPatches(Logger, _harmony, _archipelago, _locationChecker);
            _locationChecker.VerifyNewLocationChecksWithArchipelago();
            _locationChecker.SendAllLocationChecks();
        }

        private void ConnectToArchipelago()
        {
            ReadPersistentArchipelagoData();
            // _chatForwarder = new ChatForwarder(Monitor, _harmony, _giftHandler, _appearanceRandomizer);

            if (APConnectionInfo != null && !_archipelago.IsConnected)
            {
                _archipelago.Connect(APConnectionInfo, out var errorMessage);
            }

            if (!_archipelago.IsConnected)
            {
                APConnectionInfo = null;
                var userMessage = $"Could not connect to archipelago. Please verify the connection file ({Persistency.CONNECTION_FILE}) and that the server is available.{Environment.NewLine}";
                Logger.LogError(userMessage);
                const int timeUntilClose = 10;
                Logger.LogError($"The Game will close in {timeUntilClose} seconds");
                Thread.Sleep(timeUntilClose * 1000);
                Application.Quit();
                return;
            }
            
            Logger.LogMessage($"Connected to Archipelago as {_archipelago.SlotData.SlotName}.");
            WritePersistentArchipelagoData();
            // PatcherInitializer.InitializeEarly(Logger, _archipelago);
        }

        private void ReadPersistentArchipelagoData()
        {
            if (!File.Exists(Persistency.CONNECTION_FILE))
            {
                var defaultConnectionInfo = new ArchipelagoConnectionInfo("archipelago.gg", 38281, "Name", false);
                WritePersistentData(defaultConnectionInfo, Persistency.CONNECTION_FILE);
            }

            var jsonString = File.ReadAllText(Persistency.CONNECTION_FILE);
            var connectionInfo = JsonConvert.DeserializeObject<ArchipelagoConnectionInfo>(jsonString);
            if (connectionInfo == null)
            {
                return;
            }

            APConnectionInfo = connectionInfo;
        }

        private void WritePersistentArchipelagoData()
        {
            WritePersistentData(APConnectionInfo, Persistency.CONNECTION_FILE);
        }

        private void WritePersistentData(object data, string path)
        {
            var jsonObject = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, jsonObject);
        }

        private void OnItemReceived()
        {
            if (_archipelago == null || _itemManager == null)
            {
                return;
            }

            Logger.LogDebug($"Received a new item!");

            _itemManager.ReceiveAllNewItems();
        }
    }
}