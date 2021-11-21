using System;
using System.Collections.Generic;
using System.Text;

namespace IS.Reading
{
    internal static class StringExtensions
    {
        public static bool EqualsCI(this string str1, string str2)
            => string.Compare(str1, str2, true) == 0;
    }
}
