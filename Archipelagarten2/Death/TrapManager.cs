using System;
using Archipelagarten2.Characters;
using Archipelagarten2.Constants;
using Archipelagarten2.Utilities;
using KG2;
using Object = UnityEngine.Object;

namespace Archipelagarten2.Death
{
    public class TrapManager
    {
        private CharacterActions _characterActions;

        public TrapManager(CharacterActions characterActions)
        {
            _characterActions = characterActions;
        }

        public bool TryHandleTrap(string itemName)
        {
            if (TryHandleJanitorTrap(itemName))
            {
                return true;
            }

            return false;
        }

        private bool TryHandleJanitorTrap(string itemName)
        {
            if (itemName != APItem.JANITOR_TRAP)
            {
                return false;
            }

            try
            {
                var janitor = Object.FindObjectOfType<Janitor>();
                _characterActions.MoveNPCToCurrentRoom(janitor);
                // npc.StartWaitToInteract(1f, EnvironmentController.Instance.ContainsFlag(Flag.JanitorGoGetChainsaw) ? 447 : 444);
                janitor.KillPlayer(DeathId.JANITOR_TRAP);
            }
            catch (Exception ex)
            {
                DebugLogging.LogErrorException(ex, nameof(TryHandleJanitorTrap));
            }

            return true;
        }
    }
}
