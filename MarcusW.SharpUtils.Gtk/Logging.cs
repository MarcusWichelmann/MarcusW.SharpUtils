using System;
using GLib;
using Microsoft.Extensions.Logging;

namespace MarcusW.SharpUtils.Gtk
{
    public static class Logging
    {
        /// <summary>
        /// Use the Microsoft Logging Extension to handle GLib log output.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="prefix">An optional prefix for the logged messages</param>
        public static void RedirectGLibLogging(ILogger logger, string prefix = null)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            Log.SetDefaultHandler((logDomain, logLevel, message) => {
                if (prefix != null)
                    message = prefix + message;

                if (logLevel.HasFlag(LogLevelFlags.Critical))
                    logger.LogCritical(message);
                else if (logLevel.HasFlag(LogLevelFlags.Error))
                    logger.LogError(message);
                else if (logLevel.HasFlag(LogLevelFlags.Warning))
                    logger.LogWarning(message);
                else
                    logger.LogInformation(message);
            });
        }
    }
}
