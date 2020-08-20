namespace PokeBot.Helpers
{
    public static class Extensions
    {
        public static string[] SplitString(this string str, char delimiter)
        {
            return str.Split(delimiter);
        }
    }
}