using static System.Char;
using static System.String;

namespace Fly.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str)
        {
            if (IsNullOrEmpty(str) || IsLower(str, 0))
                return str;

            return ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}
