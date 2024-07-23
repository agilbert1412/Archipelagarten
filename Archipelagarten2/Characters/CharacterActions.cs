using KG2;

namespace Archipelagarten2.Characters
{
    public class CharacterActions
    { 
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
        }
    }
}
