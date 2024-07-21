using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archipelagarten2.HarmonyPatches;
using BepInEx.Logging;

namespace Archipelagarten2.Patching
{
    public static class DebugLogging
    {
        private static ManualLogSource _logger;

        public static void Initialize(ManualLogSource logger)
        {
            _logger = logger;
        }

        public static void LogDebugPatchIsRunning(string patchedType, string patchedMethod, string patchType, string patchMethod, params object[] arguments)
        {
#if DEBUG
            var argumentsString = arguments != null && arguments.Any() ? string.Join(", ", arguments) : "";
            _logger.LogDebug($"{patchedType}.{patchedMethod}.{patchType}.{patchMethod}({argumentsString})");
#endif
        }
    }
}
