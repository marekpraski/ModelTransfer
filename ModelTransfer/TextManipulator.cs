using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModelTransfer
{
    //zawiera różne metody związane z manipulowaniem tekstem, które powtarzają się w całym programie
    public class TextManipulator
    {
        //znajduje położenie wszystkich instancji stringa value
        //w stringu str, zwraca listę indeksów początków pozycji
        public List<int> getSubstringStartPositions(string str, string value)
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

        public string removeExcessWhiteSpaces(string text)
        {
            string editedText = Regex.Replace(text, @"\s+", " ");
            return editedText;
        }
    }
}
