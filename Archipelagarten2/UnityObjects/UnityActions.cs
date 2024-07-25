using System;
using KG2;
using UnityEngine;
using UnityEngine.SceneManagement;
using ILogger = KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger;
using Object = UnityEngine.Object;

namespace Archipelagarten2.UnityObjects
{
    public class UnityActions
    {
        private const float DISTANCE_OFFSCREEN = 4f;
        private const float DISTANCE_RANGED = 2f;
        private const float DISTANCE_MELEE = 0.255f;

        private ILogger _logger;
        private GameObjectFactory _factory;

        public UnityActions(ILogger logger, GameObjectFactory gameObjectFactory)
        {
            _logger = logger;
            _factory = gameObjectFactory;
        }

        public GameObject FindOrCreateByName(string name)
        {
            var gameObject = GameObject.Find(name);
            if (gameObject == null)
            {
                // Not sure how to do that
                // GameObject.Instantiate()
                throw new ArgumentException($"Could not Find GameObject named {name}");
            }

            return gameObject;
        }

        public T FindOrCreateNpc<T>() where T : NPCBehavior, new()
        {
            var gameObject = Object.FindObjectOfType<T>();

            if (gameObject != null)
            {
                return gameObject;
            }

            if (_factory.TryCreateNPC(out gameObject))
            {
                return gameObject;
            }

            gameObject = new T();
            return gameObject;
        }

        public ObjectInteractable FindOrCreateInteractable()
        {
            return FindOrCreate<ObjectInteractable>();
        }

        public PlayerController FindOrCreatePlayer()
        {
            // Player should exist lmao
            return Object.FindObjectOfType<PlayerController>();
        }

        public T FindOrCreatePanel<T>() where T : PanelBehavior, new()
        {
            return FindOrCreate<T>();
        }

        public T FindOrCreate<T>() where T : MonoBehaviour, new()
        {
            var gameObject = Object.FindObjectOfType<T>();

            if (gameObject != null)
            {
                return gameObject;
            }

            //if (_factory.TryCreate(out gameObject))
            //{
            //    return gameObject;
            //}

            gameObject = new T();
            return gameObject;
        }

        public bool MoveNPCToMeleeDistance(NPCBehavior npc)
        {
            _logger.LogDebug($"Attempting to move {npc.name} to Melee Distance");
            return MoveNPCToCurrentRoom(npc, DISTANCE_MELEE);
        }

        public bool MoveNPCToRangedDistance(NPCBehavior npc)
        {
            _logger.LogDebug($"Attempting to move {npc.name} to Ranged Distance");
            return MoveNPCToCurrentRoom(npc, DISTANCE_RANGED);
        }

        public bool MoveNPCToCurrentRoom(NPCBehavior npc, float distanceFromPlayer)
        {
            if (npc == null)
            {
                return false;
            }

            npc.player.SetPlayerState(PlayerState.AnimState);
            TransferNpcToCurrentRoom(npc);

            var currentDistance = Math.Abs(npc.transform.position.x - npc.player.transform.position.x) +
                                  Math.Abs(npc.transform.position.y - npc.player.transform.position.y);

            if (currentDistance < distanceFromPlayer)
            {
                return true;
            }

            var playerOffsetX = -distanceFromPlayer;
            if (npc.transform.position.x > (double)npc.player.transform.position.x)
            {
                playerOffsetX = distanceFromPlayer;
            }

            _logger.LogDebug($"Attempting to walk {npc} towards player");
            npc.WalkStraightLine(new Vector3(npc.player.transform.localPosition.x + playerOffsetX, npc.player.transform.localPosition.y, npc.player.transform.localPosition.z), 0.0f, npc.FacePlayer);
            _logger.LogDebug($"Successfully requested {npc} to walk towards player");
            return true;
        }

        private void TransferNpcToCurrentRoom(NPCBehavior npc)
        {
            _logger.LogDebug($"Attempting to move {npc} to current room");
            var playerRoom = EnvironmentController.currentRoom;
            _logger.LogDebug($"Current room is {playerRoom}");
            var npcRoom = npc.GetCurrentRoom();
            _logger.LogDebug($"Npc room is {npcRoom}");

            if (npcRoom == playerRoom)
            {
                return;
            }

            EnvironmentController.Instance.SetCharacterRoom(npc.transform, playerRoom, npcRoom);
            _logger.LogDebug($"Attempting to set camera target");
            npc.SetCameraTarget();
        }
    }
}
