namespace TicTacToeCSharpPlayground.Helper
{
    public static class StringExtensionMethods
    {
        public static int ToInt(this string target)
        {
            return int.Parse(target);
        }
    }
}
