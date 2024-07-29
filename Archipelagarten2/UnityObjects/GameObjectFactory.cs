using System;
using System.Linq;
using System.Threading;
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

        //public bool TryCreatePanel<T>(out T createdObject) where T : PanelBehavior
        //{
        //    return TryCreate<T>(out createdObject, "Panels");
        //}

        public bool TryCreateInteractable<T>(out T createdObject) where T : Interactable
        {
            return TryCreate(out createdObject, "Interactables");
        }

        public bool TryCreateNPC<T>(out T createdObject) where T : NPCBehavior
        {
            return TryCreate(out createdObject, "NPCs");
        }

        public bool TryCreateOther<T>(out T createdObject) where T : MonoBehaviour
        {
            return TryCreate(out createdObject, "NPCs");
        }

        public bool TryCreate<T>(out T createdObject, string parentName) where T : MonoBehaviour
        {
            var player = Object.FindObjectOfType<PlayerController>();
            _logger.LogMessage($"\tTrying to create an instance of {typeof(T)}");
            var currentTime = Object.FindObjectOfType<WorldEventManager>().time;
            _logger.LogMessage($"\tCurrent time: {currentTime}");

            var currentScene = SceneManager.GetActiveScene();
            var roomEventManager = Object.FindObjectOfType<RoomEventManager>();

            if (!TryFindSpecificChild(roomEventManager.transform, parentName, out var currentParent))
            {
                _logger.LogMessage($"\tCould not find '{parentName}' for the found object");
                createdObject = null;
                return false;
            }

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
                    Thread.Sleep(10);
                    // _logger.LogMessage($"\tWaiting...");
                }

                _logger.LogMessage($"\tLooking for an object of type {typeof(T)}");
                var foundObject = Object.FindObjectOfType<T>();
                if (foundObject != null)
                {
                    _logger.LogMessage($"\tFound one! Trying to assign it to the current room");

                    foundObject.transform.parent = currentParent;
                    createdObject = foundObject;
                    if (createdObject is Interactable interactable)
                    {
                        interactable.player = player;
                    }

                    _logger.LogMessage($"\tUnloading the {timeOfDay} scene");
                    var unloadSceneOperation = SceneManager.UnloadSceneAsync(timeOfDay.ToString());

                    while (!unloadSceneOperation.isDone)
                    {
                        Thread.Sleep(10);
                        // _logger.LogMessage($"\tWaiting...");
                    }

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
