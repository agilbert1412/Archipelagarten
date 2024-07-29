using System;
using Archipelagarten2.Constants;
using Archipelagarten2.UnityObjects;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using KG2;

namespace Archipelagarten2.Death
{
    public class TrapManager
    {
        private ILogger _logger;
        private UnityActions _unityActions;

        public TrapManager(ILogger logger, UnityActions unityActions)
        {
            _logger = _logger;
            _unityActions = unityActions;
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
                var janitor = _unityActions.FindOrCreateNpc<Janitor>();
                if (!_unityActions.MoveNPCToRangedDistance(janitor))
                {
                    _logger.LogError($"Failed in {nameof(TryHandleJanitorTrap)}, could not bring a Janitor to the player");
                }

                // npc.StartWaitToInteract(1f, EnvironmentController.Instance.ContainsFlag(Flag.JanitorGoGetChainsaw) ? 447 : 444);
                janitor.KillPlayer(DeathId.JANITOR_TRAP);
            }
            catch (Exception ex)
            {
                _logger.LogErrorException($"Failed in {nameof(TryHandleJanitorTrap)}", ex);
            }

            return true;
        }
    }
}
