﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using BepInEx.Logging;
using HarmonyLib;

namespace Archipelagarten2.Archipelago
{
    public class ArchipelagoClient
    {
        private const string GAME_NAME = "Kindergarten 2";
        private ManualLogSource _console;
        private ArchipelagoSession _session;
        private DeathLinkService _deathLinkService;

        private ArchipelagoConnectionInfo _connectionInfo;

        private Action _itemReceivedFunction;

        private ArchipelagoSession Session => _session;

        public bool IsConnected { get; private set; }
        public SlotData SlotData { get; private set; }
        public Dictionary<string, ScoutedLocation> ScoutedLocations { get; set; }

        private DataPackageCache _localDataPackage;

        public ArchipelagoClient(ManualLogSource console, Action itemReceivedFunction)
        {
            _console = console;
            _itemReceivedFunction = itemReceivedFunction;
            IsConnected = false;
            ScoutedLocations = new Dictionary<string, ScoutedLocation>();
            _localDataPackage = new DataPackageCache();
        }

        public void Connect(ArchipelagoConnectionInfo connectionInfo, out string errorMessage)
        {
            DisconnectPermanently();
            _connectionInfo = connectionInfo;
            var success = TryConnect(_connectionInfo, out errorMessage);
            if (!success)
            {
                DisconnectPermanently();
                return;
            }
        }

        private bool TryConnect(ArchipelagoConnectionInfo connectionInfo, out string errorMessage)
        {
            LoginResult result;
            try
            {
                InitSession(connectionInfo);
                var itemsHandling = ItemsHandlingFlags.AllItems;
                var minimumVersion = new Version(0, 4, 0);
                var tags = connectionInfo.DeathLink ? new[] { "AP", "DeathLink" } : new[] { "AP" };
                result = _session.TryConnectAndLogin(GAME_NAME, _connectionInfo.SlotName, itemsHandling, minimumVersion, tags, null, _connectionInfo.Password);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                var failure = (LoginFailure)result;
                errorMessage = $"Failed to Connect to {_connectionInfo.HostUrl}:{_connectionInfo.Port} as {_connectionInfo.SlotName}:";
                foreach (var error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }

                var detailedErrorMessage = errorMessage;
                foreach (var error in failure.ErrorCodes)
                {
                    detailedErrorMessage += $"\n    {error}";
                }

                _console.LogError(detailedErrorMessage);
                DisconnectAndCleanup();
                return false; // Did not connect, show the user the contents of `errorMessage`
            }

            errorMessage = "";

            // Successfully connected, `ArchipelagoSession` (assume statically defined as `session` from now on) can now be used to interact with the server and the returned `LoginSuccessful` contains some useful information about the initial connection (e.g. a copy of the slot data as `loginSuccess.SlotData`)
            var loginSuccess = (LoginSuccessful)result;
            var loginMessage = $"Connected to Archipelago server as {connectionInfo.SlotName} (Team {loginSuccess.Team}).";
            _console.LogInfo(loginMessage);

            // Must go AFTER a successful connection attempt
            InitializeSlotData(connectionInfo.SlotName, loginSuccess.SlotData);
            InitializeAfterConnection();
            connectionInfo.DeathLink = SlotData.DeathLink;
            return true;
        }

        private void InitializeAfterConnection()
        {
            if (_session == null)
            {
                _console.LogError($"_session is null in InitializeAfterConnection(). This should NEVER happen");
                DisconnectPermanently();
                return;
            }

            IsConnected = true;

            _session.Items.ItemReceived += OnItemReceived;
            _session.MessageLog.OnMessageReceived += OnMessageReceived;
            _session.Socket.ErrorReceived += SessionErrorReceived;
            _session.Socket.SocketClosed += SessionSocketClosed;

            InitializeDeathLink();
            // MultiRandom = new Random(SlotData.Seed);
        }

        public ArchipelagoSession GetSession()
        {
            if (!MakeSureConnected())
            {
                return null;
            }

            return Session;
        }

        public void Sync()
        {
            if (!MakeSureConnected(0))
            {
                return;
            }

            try
            {
                _session.Socket.SendPacket(new SyncPacket());
            }
            catch (Exception ex)
            {
                _console.LogError(ex.Message);
            }
        }

        private void InitializeDeathLink()
        {
            _deathLinkService = _session.CreateDeathLinkService();
            if (SlotData.DeathLink)
            {
                _deathLinkService.EnableDeathLink();
                _deathLinkService.OnDeathLinkReceived += ReceiveDeathLink;
            }
            else
            {
                _deathLinkService.DisableDeathLink();
            }
        }

        private void InitializeSlotData(string slotName, Dictionary<string, object> slotDataFields)
        {
            SlotData = new SlotData(slotName, slotDataFields, _console);
        }

        private void InitSession(ArchipelagoConnectionInfo connectionInfo)
        {
            _session = ArchipelagoSessionFactory.CreateSession(connectionInfo.HostUrl,
                connectionInfo.Port);
            _connectionInfo = connectionInfo;
        }

        private void OnMessageReceived(LogMessage message)
        {
            try
            {
                var fullMessage = string.Join(" ", message.Parts.Select(str => str.Text));
                _console.LogInfo(fullMessage);
            }
            catch (Exception ex)
            {
                _console.LogError($"Failed in {nameof(ArchipelagoClient)}.{nameof(OnMessageReceived)}:\n\t{ex}");
                Debugger.Break();
                return; // run original logic
            }
        }

        public void SendMessage(string text)
        {
            if (!MakeSureConnected())
            {
                return;
            }

            try
            {
                var packet = new SayPacket()
                {
                    Text = text
                };

                _session.Socket.SendPacket(packet);
            }
            catch (Exception ex)
            {
                _console.LogError(ex.Message);
            }
        }

        private void OnItemReceived(ReceivedItemsHelper receivedItemsHelper)
        {
            try
            {
                if (!MakeSureConnected())
                {
                    return;
                }

                _itemReceivedFunction();
            }
            catch (Exception ex)
            {
                _console.LogError($"Failed in {nameof(ArchipelagoClient)}.{nameof(OnItemReceived)}:\n\t{ex}");
                Debugger.Break();
                return; // run original logic
            }
        }

        public void ReportCheckedLocationsAsync(long[] locationIds)
        {
            var newLocations = locationIds.Except(_session.Locations.AllLocationsChecked).ToArray();
            if (!newLocations.Any())
            {
                return;
            }

            ThreadPool.QueueUserWorkItem((o) => ReportCheckedLocations(newLocations));
        }

        public void ReportCheckedLocations(long[] locationIds)
        {
            if (!MakeSureConnected())
            {
                return;
            }

            try
            {
                _session.Locations.CompleteLocationChecks(locationIds);
            }
            catch (Exception ex)
            {
                _console.LogError(ex.Message);
            }
        }

        private string GetPlayerName(int playerId)
        {
            return _session.Players.GetPlayerName(playerId) ?? "Archipelago";
        }

        public string GetPlayerAlias(string playerName)
        {
            try
            {
                var player = _session.Players.AllPlayers.FirstOrDefault(x => x.Name == playerName);
                if (player == null || player.Alias == playerName)
                {
                    return null;
                }

                return player.Alias.Substring(0, player.Alias.Length - playerName.Length - 3);
            }
            catch (Exception ex)
            {
                _console.LogError(ex.Message);
                return null;
            }
        }

        public int GetTeam()
        {
            return _session.ConnectionInfo.Team;
        }

        public Dictionary<string, long> GetAllCheckedLocations()
        {
            if (!MakeSureConnected())
            {
                return new Dictionary<string, long>();
            }

            try
            {
                var allLocationsCheckedIds = _session.Locations.AllLocationsChecked;
                var allLocationsChecked = allLocationsCheckedIds.ToDictionary(GetLocationName, x => x);
                return allLocationsChecked;
            }
            catch (Exception ex)
            {
                _console.LogError(ex.Message);
                return new Dictionary<string, long>();
            }
        }

        public List<ReceivedItem> GetAllReceivedItems()
        {
            var allReceivedItems = new List<ReceivedItem>();
            if (!MakeSureConnected())
            {
                return allReceivedItems;
            }

            try
            {
                var apItems = _session.Items.AllItemsReceived.ToArray();
                for (var itemIndex = 0; itemIndex < apItems.Length; itemIndex++)
                {
                    var apItem = apItems[itemIndex];
                    var itemName = GetItemName(apItem);
                    var playerName = GetPlayerName(apItem.Player);
                    var locationName = GetLocationName(apItem) ?? "Thin air";

                    var receivedItem = new ReceivedItem(locationName, itemName, playerName, apItem.LocationId, apItem.ItemId,
                        apItem.Player, itemIndex);

                    allReceivedItems.Add(receivedItem);
                }
            }
            catch (Exception ex)
            {
                _console.LogError(ex.Message);
            }

            return allReceivedItems;
        }

        private Dictionary<string, int> GetAllReceivedItemNamesAndCounts()
        {
            if (!MakeSureConnected())
            {
                return new Dictionary<string, int>();
            }

            var receivedItemsGrouped = _session.Items.AllItemsReceived.GroupBy(x => x.ItemId);
            var receivedItemsWithCount = receivedItemsGrouped.ToDictionary(x => GetItemName(x.First()), x => x.Count());
            return receivedItemsWithCount;
        }

        public bool HasReceivedItem(string itemName, out string sendingPlayer)
        {
            sendingPlayer = "";
            if (!MakeSureConnected())
            {
                return false;
            }

            try
            {
                foreach (var receivedItem in _session.Items.AllItemsReceived)
                {
                    if (GetItemName(receivedItem) != itemName)
                    {
                        continue;
                    }

                    sendingPlayer = _session.Players.GetPlayerName(receivedItem.Player);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _console.LogError(ex.Message);
            }

            return false;
        }

        public int GetReceivedItemCount(string itemName)
        {
            if (!MakeSureConnected())
            {
                return 0;
            }

            try
            {
                return _session.Items.AllItemsReceived.Count(x => GetItemName(x) == itemName);
            }
            catch (Exception ex)
            {
                _console.LogError(ex.Message);
                return 0;
            }
        }

        private Hint[] GetHints()
        {
            if (!MakeSureConnected())
            {
                return Array.Empty<Hint>();
            }

            return _session.DataStorage.GetHints();
        }

        public void ReportGoalCompletion()
        {
            if (!MakeSureConnected())
            {
                return;
            }

            try
            {
                _console.LogMessage("Goal Complete!");
                var statusUpdatePacket = new StatusUpdatePacket
                {
                    Status = ArchipelagoClientState.ClientGoal,
                };
                _session.Socket.SendPacket(statusUpdatePacket);
            }
            catch (Exception ex)
            {
                _console.LogError(ex.Message);
            }
        }

        private string GetLocationName(ItemInfo item)
        {
            if (!string.IsNullOrWhiteSpace(item.LocationName))
            {
                return item.LocationName;
            }

            if (!MakeSureConnected())
            {
                return "";
            }

            var locationName = _session.Locations.GetLocationNameFromId(item.LocationId);
            if (string.IsNullOrWhiteSpace(locationName) && item.LocationGame.Equals(GAME_NAME, StringComparison.InvariantCultureIgnoreCase))
            {
                locationName = _localDataPackage.GetLocalLocationName(item.LocationId);
            }

            return locationName;
        }

        private string GetLocationName(long locationId)
        {
            if (!MakeSureConnected())
            {
                return "";
            }

            var locationName = _session.Locations.GetLocationNameFromId(locationId);
            if (string.IsNullOrWhiteSpace(locationName))
            {
                locationName = _localDataPackage.GetLocalLocationName(locationId);
            }

            return locationName;
        }

        public bool LocationExists(string locationName)
        {
            if (!MakeSureConnected())
            {
                return false;
            }

            var id = GetLocationId(locationName);
            return _session.Locations.AllLocations.Contains(id);
        }

        public long GetLocationId(string locationName, string gameName = GAME_NAME)
        {
            var locationId = -1L;
            if (MakeSureConnected())
            {
                try
                {
                    locationId = _session.Locations.GetLocationIdFromName(gameName, locationName);
                }
                catch (Exception ex)
                {
                    _console.LogError(ex.Message);
                }
            }
            if (locationId <= 0)
            {
                locationId = _localDataPackage.GetLocalLocationId(locationName);
            }

            return locationId;
        }

        private string GetItemName(ItemInfo item)
        {
            if (!string.IsNullOrWhiteSpace(item.ItemName))
            {
                return item.ItemName;
            }

            if (!MakeSureConnected())
            {
                return "";
            }

            var itemName = _session.Items.GetItemName(item.ItemId);
            if (string.IsNullOrWhiteSpace(itemName) && item.ItemGame.Equals(GAME_NAME, StringComparison.InvariantCultureIgnoreCase))
            {
                itemName = _localDataPackage.GetLocalItemName(item.ItemId);
            }

            return itemName;
        }

        public void SendDeathLinkAsync(string player, string reason = "Unknown cause")
        {
            Task.Run(() => SendDeathLink(player, reason));
        }

        public void SendDeathLink(string player, string reason = "Unknown cause")
        {
            if (!MakeSureConnected())
            {
                return;
            }

            try
            {
                _deathLinkService.SendDeathLink(new DeathLink(player, reason));
            }
            catch (Exception ex)
            {
                _console.LogError(ex.Message);
            }
        }

        private void ReceiveDeathLink(DeathLink deathlink)
        {
            // DiePatch.ReceiveDeathLink();
            var deathLinkMessage = $"You have been killed by {deathlink.Source} ({deathlink.Cause})";
            _console.LogInfo(deathLinkMessage);
        }

        public Dictionary<string, ScoutedLocation> ScoutManyLocations(IEnumerable<string> locationNames)
        {
            var scoutResult = new Dictionary<string, ScoutedLocation>();
            if (!MakeSureConnected() || locationNames == null || !locationNames.Any())
            {
                _console.LogInfo($"Could not scout locations {locationNames}");
                return scoutResult;
            }

            var namesToScout = new List<string>();
            var idsToScout = new List<long>();
            foreach (var locationName in locationNames)
            {
                if (ScoutedLocations.ContainsKey(locationName))
                {
                    scoutResult.Add(locationName, ScoutedLocations[locationName]);
                    continue;
                }

                var locationId = GetLocationId(locationName);
                if (locationId == -1)
                {
                    _console.LogInfo($"Could get location id for \"{locationName}\".");
                    continue;
                }

                namesToScout.Add(locationName);
                idsToScout.Add(locationId);
            }

            if (!idsToScout.Any())
            {
                return scoutResult;
            }

            ScoutedItemInfo[] scoutResponse;
            try
            {
                scoutResponse = ScoutLocations(idsToScout.ToArray(), false);
                if (scoutResponse.Length < 1)
                {
                    _console.LogInfo($"Could not scout location ids \"{idsToScout}\".");
                    return scoutResult;
                }
            }
            catch (Exception e)
            {
                _console.LogInfo($"Could not scout location ids \"{idsToScout}\". Message: {e.Message}");
                return scoutResult;
            }

            for (var i = 0; i < idsToScout.Count; i++)
            {
                if (scoutResponse.Length <= i)
                {
                    break;
                }

                var itemScouted = scoutResponse[i];
                var itemName = GetItemName(itemScouted);
                var playerSlotName = _session.Players.GetPlayerName(itemScouted.Player);

                var scoutedLocation = new ScoutedLocation(namesToScout[i], itemName, playerSlotName, idsToScout[i], itemScouted.ItemId, itemScouted.Player);

                ScoutedLocations.Add(namesToScout[i], scoutedLocation);
                scoutResult.Add(namesToScout[i], scoutedLocation);
            }

            return scoutResult;
        }

        public ScoutedLocation ScoutSingleLocation(string locationName, bool createAsHint = false)
        {
            if (ScoutedLocations.ContainsKey(locationName))
            {
                return ScoutedLocations[locationName];
            }

            if (!MakeSureConnected())
            {
                _console.LogInfo($"Could not find the id for location \"{locationName}\".");
                return null;
            }

            try
            {
                var locationId = GetLocationId(locationName);
                if (locationId == -1)
                {
                    _console.LogInfo($"Could not find the id for location \"{locationName}\".");
                    return null;
                }

                var scoutedItemInfo = ScoutLocation(locationId, createAsHint);
                if (scoutedItemInfo == null)
                {
                    _console.LogInfo($"Could not scout location \"{locationName}\".");
                    return null;
                }
                
                var itemName = GetItemName(scoutedItemInfo);
                var playerSlotName = _session.Players.GetPlayerName(scoutedItemInfo.Player);

                var scoutedLocation = new ScoutedLocation(locationName, itemName, playerSlotName, locationId,
                    scoutedItemInfo.ItemId, scoutedItemInfo.Player);

                ScoutedLocations.Add(locationName, scoutedLocation);
                return scoutedLocation;
            }
            catch (Exception e)
            {
                _console.LogInfo($"Could not scout location \"{locationName}\". Message: {e.Message}");
                return null;
            }
        }

        private ScoutedItemInfo ScoutLocation(long locationId, bool createAsHint)
        {
            var scoutTask = _session.Locations.ScoutLocationsAsync(createAsHint, locationId);
            scoutTask.Wait();
            var scoutedItems = scoutTask.Result;
            if (scoutedItems == null || !scoutedItems.Any())
            {
                return null;
            }

            return scoutedItems.First().Value;
        }

        private ScoutedItemInfo[] ScoutLocations(long[] locationIds, bool createAsHint)
        {
            var scoutTask = _session.Locations.ScoutLocationsAsync(createAsHint, locationIds);
            scoutTask.Wait();
            var scoutedItems = scoutTask.Result;
            if (scoutedItems == null || !scoutedItems.Any())
            {
                return null;
            }

            return scoutedItems.Values.ToArray();
        }

        private void SessionErrorReceived(Exception e, string message)
        {
            _console.LogError($"Connection to Archipelago lost due to receiving a server error. The game will try reconnecting later.\n\tMessage: {message}\n\tException: {e}");
            _lastConnectFailure = DateTime.Now;
            DisconnectAndCleanup();
        }

        private void SessionSocketClosed(string reason)
        {
            _console.LogError($"Connection to Archipelago lost due to the socket closing. The game will try reconnecting later.\n\tReason: {reason}");
            _lastConnectFailure = DateTime.Now;
            DisconnectAndCleanup();
        }

        public void DisconnectAndCleanup()
        {
            if (!IsConnected)
            {
                return;
            }

            if (_session != null)
            {
                _session.Items.ItemReceived -= OnItemReceived;
                _session.MessageLog.OnMessageReceived -= OnMessageReceived;
                _session.Socket.ErrorReceived -= SessionErrorReceived;
                _session.Socket.SocketClosed -= SessionSocketClosed;
                _session.Socket.DisconnectAsync();
            }

            _session = null;
            IsConnected = false;
        }

        public void DisconnectTemporarily()
        {
            DisconnectAndCleanup();
            _allowRetries = false;
        }

        public void ReconnectAfterTemporaryDisconnect()
        {
            _allowRetries = true;
            MakeSureConnected(0);
        }

        public void DisconnectPermanently()
        {
            DisconnectAndCleanup();
            _connectionInfo = null;
        }

        private DateTime _lastConnectFailure;
        private const int THRESHOLD_TO_RETRY_CONNECTION_IN_SECONDS = 15;
        private bool _allowRetries = true;

        public bool MakeSureConnected(int threshold = THRESHOLD_TO_RETRY_CONNECTION_IN_SECONDS)
        {
            if (IsConnected)
            {
                return true;
            }

            if (_connectionInfo == null)
            {
                return false;
            }

            var now = DateTime.Now;
            var timeSinceLastFailure = now - _lastConnectFailure;
            if (timeSinceLastFailure.TotalSeconds < threshold)
            {
                return false;
            }

            if (!_allowRetries)
            {
                _console.LogError("Reconnection attempt failed");
                _lastConnectFailure = DateTime.Now;
                return false;
            }

            TryConnect(_connectionInfo, out _);
            if (!IsConnected)
            {
                _console.LogError("Reconnection attempt failed");
                _lastConnectFailure = DateTime.Now;
                return false;
            }

            _console.LogMessage("Reconnection attempt successful!");
            return IsConnected;
        }
    }
}
