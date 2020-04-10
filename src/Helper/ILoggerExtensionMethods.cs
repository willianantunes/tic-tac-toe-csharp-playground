using Microsoft.Extensions.Logging;

namespace src.Helper
{
    public static class ILoggerExtensionMethods
    {
        public static void Info(this ILogger logger, string message, params object[] args)
        {
            logger.LogInformation(message, args);
        }
    }
}