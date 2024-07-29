using System;
using KG2;
using UnityEngine;
using ILogger = KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger;
using Object = UnityEngine.Object;

namespace Archipelagarten2.UnityObjects
{
    public class UnityActions
    {
        private const float DISTANCE_OFFSCREEN = 3f;
        private const float DISTANCE_RANGED = 1.5f;
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
            var gameObject = Object.FindObjectOfType<ObjectInteractable>();

            if (gameObject != null)
            {
                return gameObject;
            }

            if (_factory.TryCreateInteractable(out gameObject))
            {
                return gameObject;
            }

            gameObject = new ObjectInteractable();
            return gameObject;
        }

        public PlayerController FindOrCreatePlayer()
        {
            // Player should exist lmao
            return Object.FindObjectOfType<PlayerController>();
        }

        public T FindOrCreatePanel<T>() where T : PanelBehavior, new()
        {
            var gameObject = Object.FindObjectOfType<T>();

            if (gameObject != null)
            {
                return gameObject;
            }

            //if (_factory.TryCreatePanel(out gameObject))
            //{
            //    return gameObject;
            //}

            gameObject = new T();
            return gameObject;
        }

        public T FindOrCreateOther<T>() where T : MonoBehaviour, new()
        {
            var gameObject = Object.FindObjectOfType<T>();

            if (gameObject != null)
            {
                return gameObject;
            }

            if (_factory.TryCreateOther<T>(out gameObject))
            {
                return gameObject;
            }

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

            if (currentDistance > DISTANCE_OFFSCREEN)
            {
                _logger.LogDebug($"Teleporting {npc} a bit closer before walking");
                var dest = new Vector3(npc.player.transform.localPosition.x - DISTANCE_OFFSCREEN, npc.player.transform.localPosition.y, npc.player.transform.localPosition.z);
                var path = new Vector3[1] { dest };
                npc.WalkPath(path, 0.0f, npc.FacePlayer);
                _logger.LogDebug($"Successfully teleported {npc} closer to the player");
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
