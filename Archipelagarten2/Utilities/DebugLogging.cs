using System;
using System.Linq;
using BepInEx.Logging;
using DG.Tweening;

namespace Archipelagarten2.Utilities
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
            _logger.LogDebug($"{patchedType}.{patchedMethod}.{patchType}.{patchMethod}({GenerateArgumentsString(arguments)})");
#endif
        }

        public static void LogDebug(string message, params object[] arguments)
        {
            _logger.LogDebug($"{message}\n\t{GenerateArgumentsString(arguments)})");
        }

        public static void LogErrorException(Exception ex, params object[] arguments)
        {
            _logger.LogError($"Exception Thrown:\n\t{ex}\n\t{GenerateArgumentsString(arguments)}");
        }

        public static void LogWarningException(Exception ex, params object[] arguments)
        {
            _logger.LogWarning($"Exception Thrown:\n\t{ex}\n\t{GenerateArgumentsString(arguments)}");
        }

        public static void LogErrorMessage(string message, params object[] arguments)
        {
            _logger.LogError($"{message}\n\t{GenerateArgumentsString(arguments)}");
        }

        public static void LogErrorException(string patchType, string patchMethod, Exception ex, params object[] arguments)
        {
            _logger.LogError($"Failed in {patchType}.{patchMethod}:\n\t{ex}\n\t{GenerateArgumentsString(arguments)}");
        }

        private static string GenerateArgumentsString(object[] arguments)
        {
            var argumentsString = arguments != null && arguments.Any() ? string.Join(", ", arguments) : "";
            return argumentsString;
        }
    }
}
