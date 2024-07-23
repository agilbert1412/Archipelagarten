//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Archipelagarten2.Archipelago;
//using Archipelagarten2.Utilities;
//using BepInEx.Logging;

//namespace Archipelagarten2.Locations
//{
//    public class LocationChecker
//    {
//        private static ILogger _logger;
//        private ArchipelagoClient _archipelago;
//        private Dictionary<string, long> _checkedLocations;

//        public LocationChecker(ManualLogSource logger, ArchipelagoClient archipelago, List<string> locationsAlreadyChecked)
//        {
//            _logger = logger;
//            _archipelago = archipelago;
//            _checkedLocations = locationsAlreadyChecked.ToDictionary(x => x, x => (long)-1);
//        }

//        public List<string> GetAllLocationsAlreadyChecked()
//        {
//            return _checkedLocations.Keys.ToList();
//        }

//        public bool IsLocationChecked(string locationName)
//        {
//            return _checkedLocations.ContainsKey(locationName);
//        }

//        public bool IsLocationNotChecked(string locationName)
//        {
//            return !IsLocationChecked(locationName);
//        }

//        public bool IsLocationMissing(string locationName)
//        {
//            return _archipelago.LocationExists(locationName) && IsLocationNotChecked(locationName);
//        }

//        public void AddCheckedLocation(string locationName)
//        {
//            if (_checkedLocations.ContainsKey(locationName))
//            {
//                return;
//            }

//            var locationId = _archipelago.GetLocationId(locationName);

//            if (locationId == -1)
//            {
//                var alternateName = GetAllLocationsNotChecked().FirstOrDefault(x => x.Equals(locationName, StringComparison.InvariantCultureIgnoreCase));
//                if (alternateName == null)
//                {
//                    DebugLogging.LogErrorMessage($"Location \"{locationName}\" could not be converted to an Archipelago id");
//                    return;
//                }

//                locationId = _archipelago.GetLocationId(alternateName);
//                _logger.LogWarning($"Location \"{locationName}\" not found, checking location \"{alternateName}\" instead");
//            }

//            _checkedLocations.Add(locationName, locationId);
//            SendAllLocationChecks();
//        }

//        public void SendAllLocationChecks()
//        {
//            if (!_archipelago.IsConnected)
//            {
//                return;
//            }

//            TryToIdentifyUnknownLocationNames();

//            var allCheckedLocations = new List<long>();
//            allCheckedLocations.AddRange(_checkedLocations.Values);

//            allCheckedLocations = allCheckedLocations.Distinct().Where(x => x > -1).ToList();

//            _archipelago.ReportCheckedLocations(allCheckedLocations.ToArray());
//        }

//        public void VerifyNewLocationChecksWithArchipelago()
//        {
//            var allCheckedLocations = _archipelago.GetAllCheckedLocations();
//            foreach (var checkedLocation in allCheckedLocations)
//            {
//                if (!_checkedLocations.ContainsKey(checkedLocation.Key))
//                {
//                    _checkedLocations.Add(checkedLocation.Key, checkedLocation.Value);
//                }
//            }
//        }

//        private void TryToIdentifyUnknownLocationNames()
//        {
//            foreach (var locationName in _checkedLocations.Keys)
//            {
//                if (_checkedLocations[locationName] > -1)
//                {
//                    continue;
//                }

//                var locationId = _archipelago.GetLocationId(locationName);
//                if (locationId == -1)
//                {
//                    continue;
//                }

//                _checkedLocations[locationName] = locationId;
//            }
//        }

//        public void ForgetLocations(IEnumerable<string> locations)
//        {
//            foreach (var location in locations)
//            {
//                if (!_checkedLocations.ContainsKey(location))
//                {
//                    continue;
//                }

//                _checkedLocations.Remove(location);
//            }
//        }

//        public IEnumerable<string> GetAllLocationsNotChecked()
//        {
//            if (!_archipelago.IsConnected)
//            {
//                return Enumerable.Empty<string>();
//            }

//            return _archipelago.Session.Locations.AllMissingLocations.Select(_archipelago.GetLocationName)
//                .Where(x => x != null);
//        }
//    }
//}
