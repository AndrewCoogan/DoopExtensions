using System;

namespace DoopExtensions.Extensions
{
    internal static class StringExtensions
    {
        public static bool IsEmpty(this string input)
        {
            return string.IsNullOrWhiteSpace(input);
        }

        public static void ThrowIfEmpty(this string input)
        {
            if (input.IsEmpty())
            {
                throw new ArgumentNullException(nameof(input));
            }
        }
    }
}