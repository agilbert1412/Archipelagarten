using DG.Tweening;
using KG2;
using System;
using UnityEngine;

namespace Archipelagarten2.Characters
{
    public class CharacterActions
    {
        private const float DISTANCE_TO_BE_VISIBLE = 2f;

        public void MoveNPCToCurrentRoom(NPCBehavior npc)
        {
            var npcRoom = npc.GetCurrentRoom();
            var playerRoom = EnvironmentController.currentRoom;

            if (npcRoom == playerRoom)
            {
                return;
            }

            npc.player.SetPlayerState(PlayerState.AnimState);
            EnvironmentController.Instance.SetCharacterRoom(npc.transform, playerRoom, npcRoom);
            npc.SetCameraTarget();

            var currentDistance = Math.Abs(npc.transform.position.x - npc.player.transform.position.x) +
                                  Math.Abs(npc.transform.position.y - npc.player.transform.position.y);

            if (currentDistance < 3)
            {
                return;
            }

            var num = -DISTANCE_TO_BE_VISIBLE;
            if (npc.transform.position.x > (double)npc.player.transform.position.x)
            {
                num = DISTANCE_TO_BE_VISIBLE;
            }

            npc.WalkStraightLine(new Vector3(npc.player.transform.localPosition.x + num, npc.player.transform.localPosition.y, npc.player.transform.localPosition.z), 0.0f, npc.FacePlayer);
        }
    }
}
