using System.Text.RegularExpressions;

namespace TicTacToeCSharpPlayground.Helper
{
    public static class RegexMatchExtensionMethods
    {
        public static bool NotSuccess(this Match targetMatch)
        {
            return !targetMatch.Success;
        }
    }
}
