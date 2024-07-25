using System;
using System.Collections.Generic;
using System.Linq;
using KG2;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;
using UnityEngine;
using ILogger = KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger;

namespace Archipelagarten2.UnityObjects
{
    public class GameObjectFactory
    {
        private ILogger _logger;

        public GameObjectFactory(ILogger logger)
        {
            _logger = logger;
        }

        public void CacheInstancesOfEverythingInCurrentScene()
        {
        }

        public bool TryCreateNPC<T>(out T createdObject) where T : NPCBehavior
        {
            var player = Object.FindObjectOfType<PlayerController>();
            _logger.LogMessage($"\tTrying to create an instance of {typeof(T)}");
            var currentTime = Object.FindObjectOfType<WorldEventManager>().time;
            _logger.LogMessage($"\tCurrent time: {currentTime}");

            var currentScene = SceneManager.GetActiveScene();
            var roomEventManager = Object.FindObjectOfType<RoomEventManager>();

            foreach (var timeOfDay in Enum.GetValues(typeof(TimeOfDay)).Cast<TimeOfDay>())
            {
                if (timeOfDay == currentTime)
                {
                    _logger.LogMessage($"\tSkipping current time {timeOfDay}");
                    continue;
                }

                _logger.LogMessage($"\tLoading scene for time {timeOfDay}");
                var loadSceneOperation = SceneManager.LoadSceneAsync(timeOfDay.ToString(), LoadSceneMode.Additive);

                while (!loadSceneOperation.isDone)
                {
                    // _logger.LogMessage($"\tWaiting...");
                }

                _logger.LogMessage($"\tLooking for an object of type {typeof(T)}");
                var foundObject = Object.FindObjectOfType<T>();
                if (foundObject != null)
                {
                    _logger.LogMessage($"\tFound one! Trying to assign it to the current room");

                    // SceneManager.MoveGameObjectToScene(foundObject.gameObject, currentScene);
                    

                    if (!TryFindSpecificChild(roomEventManager.transform, "NPCs", out var currentNpcs))
                    {
                        _logger.LogMessage($"\tCould not find 'NPCs' for the found object");
                        continue;
                    }

                    foundObject.transform.parent = currentNpcs;
                    // Object.DontDestroyOnLoad();
                    createdObject = foundObject;

                    // createdObject = Object.Instantiate(foundObject);

                    _logger.LogMessage($"\tfoundObject.transform.parent: {foundObject.transform.parent}");
                    _logger.LogMessage($"\tfoundObject.transform.parent.parent: {foundObject.transform.parent.parent}");
                    _logger.LogMessage($"\tfoundObject.transform.parent.parent.GetComponent<RoomEventManager>(): {foundObject.transform.parent.parent.GetComponent<RoomEventManager>()}");

                    if (createdObject is Interactable interactable)
                    {
                        interactable.player = player;
                    }

                    _logger.LogMessage($"\tUnloading the {timeOfDay} scene");
                    var unloadSceneOperation = SceneManager.UnloadSceneAsync(timeOfDay.ToString());

                    while (!unloadSceneOperation.isDone)
                    {
                        // _logger.LogMessage($"\tWaiting...");
                    }


                    if (createdObject is NPCBehavior npcBehavior)
                    {
                        _logger.LogMessage($"\tnpcBehavior.GetComponent<Collider2D>(): {npcBehavior.GetComponent<Collider2D>()}");
                    }

                    _logger.LogMessage($"\tfoundObject.transform.parent: {foundObject.transform.parent}");
                    _logger.LogMessage($"\tfoundObject.transform.parent.parent: {foundObject.transform.parent.parent}");
                    _logger.LogMessage($"\tfoundObject.transform.parent.parent.GetComponent<RoomEventManager>(): {foundObject.transform.parent.parent.GetComponent<RoomEventManager>()}");
                    _logger.LogMessage($"\tfoundObject.transform.parent.parent.GetComponent<RoomEventManager>().pathFinder: {foundObject.transform.parent.parent.GetComponent<RoomEventManager>().pathFinder}");

                    return true;
                }

                _logger.LogMessage($"\tUnloading the {timeOfDay} scene");
                SceneManager.UnloadSceneAsync(timeOfDay.ToString());
            }

            _logger.LogMessage($"\tCould not find an object of type {typeof(T)}");
            createdObject = null;
            return false;
        }

        private bool TryFindSpecificChild(Transform transform, string childName, out Transform foundChild)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name == childName)
                {
                    foundChild = child;
                    return true;
                }
            }

            foundChild = null;
            return false;
        }
    }
}
