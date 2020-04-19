using System;
using Microsoft.Extensions.Logging;
using NLog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TicTacToeCSharpPlayground.Helper
{
    public static class LoggerExtensionMethods
    {
        public static void I(this ILogger logger, string message, params object[] args)
        {
            logger.LogInformation(message, args);
        }

        public static void E(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.LogError(exception, message, args);
        }
    }

    public static class NLoggerExtensionMethods
    {
        public static void I(this Logger logger, string message, params object[] args)
        {
            logger.Info(message, args);
        }

        public static void E(this Logger logger, Exception exception, string message, params object[] args)
        {
            logger.Error(exception, message, args);
        }
    }
}
