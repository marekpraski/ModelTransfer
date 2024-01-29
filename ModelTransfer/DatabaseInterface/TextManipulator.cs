using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DatabaseInterface
{
    /// <summary>
    /// zawiera różne metody związane z manipulowaniem tekstem, które powtarzają się w całym programie
    /// </summary>
    internal class TextManipulator
    {
        /// <summary>
        /// znajduje położenie wszystkich instancji stringa value
        /// w stringu str, zwraca listę indeksów początków pozycji
        /// </summary>
        internal List<int> getSubstringStartPositions(string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        internal string removeExcessWhiteSpaces(string text)
        {
            string editedText = Regex.Replace(text, @"\s+", " ");
            return editedText;
        }
    }
}
