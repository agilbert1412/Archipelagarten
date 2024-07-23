using System;
using Archipelagarten2.Characters;
using Archipelagarten2.Constants;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;
using Object = UnityEngine.Object;

namespace Archipelagarten2.Death
{
    public class TrapManager
    {
        private ILogger _logger;
        private CharacterActions _characterActions;

        public TrapManager(ILogger logger, CharacterActions characterActions)
        {
            _logger = _logger;
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
                _logger.LogErrorException(ex, nameof(TryHandleJanitorTrap));
            }

            return true;
        }
    }
}
