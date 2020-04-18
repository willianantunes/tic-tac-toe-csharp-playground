using Microsoft.Extensions.Logging;

namespace TicTacToeCSharpPlayground.Helper
{
    public static class LoggerExtensionMethods
    {
        public static void I(this ILogger logger, string message, params object[] args)
        {
            logger.LogInformation(message, args);
        }
    }
}
