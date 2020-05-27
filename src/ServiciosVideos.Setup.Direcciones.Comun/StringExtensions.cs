using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        

        public static string UppercaseWords(this string value, bool removeSpaces = false)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return removeSpaces ? new string(array).Replace(" ", "") : new string(array);
        }

        
        

    }
}
