﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Archipelagarten2.Archipelago;
using Archipelagarten2.Characters;
using Archipelagarten2.HarmonyPatches;
using Archipelagarten2.Items;
using Archipelagarten2.Serialization;
using Archipelagarten2.Utilities;
using BepInEx;
using HarmonyLib;
using KaitoKid.ArchipelagoUtilities.Net;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using Newtonsoft.Json;
using UnityEngine;
using ILogger = KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger;

namespace Archipelagarten2
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;

        private ILogger _logger;
        private PatchInitializer _patcherInitializer;
        private Harmony _harmony;
        private KindergartenArchipelagoClient _archipelago;
        private ArchipelagoConnectionInfo APConnectionInfo { get; set; }
        private LocationChecker _locationChecker;
        private KindergartenItemManager _itemManager;
        private CharacterActions _characterActions;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Loading {MyPluginInfo.PLUGIN_GUID}...");

            try
            {
                _logger = new LogHandler(Logger);
                _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
                _harmony.PatchAll();
            }
            catch (FileNotFoundException fnfe)
            {
                _logger.LogError($"Cannot load {MyPluginInfo.PLUGIN_GUID}: A Necessary Dependency is missing [{fnfe.FileName}]");
                throw;
            }

            InitializeBeforeConnection();
            ConnectToArchipelago();
            InitializeAfterConnection();

            _logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void InitializeBeforeConnection()
        {
            _patcherInitializer = new PatchInitializer();
            _characterActions = new CharacterActions();
            _archipelago = new KindergartenArchipelagoClient(_logger, _characterActions, OnItemReceived);
        }

        private void InitializeAfterConnection()
        {
            _locationChecker = new LocationChecker(_logger, _archipelago, new List<string>());
            _itemManager = new KindergartenItemManager(_logger, _archipelago, _characterActions, new List<ReceivedItem>());

            _locationChecker.VerifyNewLocationChecksWithArchipelago();
            _locationChecker.SendAllLocationChecks();
            _itemManager.UpdateItemsAlreadyProcessed();
            _patcherInitializer.InitializeAllPatches(_logger, _harmony, _archipelago, _locationChecker);
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

            _itemManager.ReceiveAllNewItems();
        }
    }
}