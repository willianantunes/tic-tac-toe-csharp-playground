using System.Text.RegularExpressions;

namespace src.Helper
{
    public static class RegexMatchExtensionMethods
    {
        public static bool NotSuccess(this Match targetMatch)
        {
            return !targetMatch.Success;
        }
    }
}