using System;
using HarmonyLib;
using KG2;
using ILogger = KaitoKid.ArchipelagoUtilities.Net.Interfaces.ILogger;

namespace Archipelagarten2.Death
{
    [HarmonyPatch(typeof(NPCBehavior))]
    [HarmonyPatch(nameof(NPCBehavior.SetFacialExpression), typeof(FacialExpression), typeof(string))]
    public static class SetFacialExpressionPatch
    {
        private static ILogger _logger;

        public static void Initialize(ILogger logger)
        {
            _logger = logger;
        }

        // public void SetFacialExpression(FacialExpression f, string s)
        public static bool Prefix(NPCBehavior __instance, FacialExpression f, string s)
        {
            try
            {
                _logger.LogDebugPatchIsRunning(nameof(NPCBehavior), nameof(NPCBehavior.SetFacialExpression), nameof(SetFacialExpressionPatch), nameof(Prefix), f, s);

                if (__instance.ReturnHeadSprite() == null)
                {
                    __instance.GetHeadSprite();
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _logger.LogErrorException(nameof(SetFacialExpressionPatch), nameof(Prefix), ex);
                return true; // run original logic
            }
        }
    }
}
