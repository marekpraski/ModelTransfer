using System;
using System.Collections.Generic;
using System.Drawing;

namespace DatabaseInterface
{
    /// <summary>
    /// zawiera wyniki zapytania do bazy danych oraz metody na ich przetwarzania i uzyskiwania w różnych postaciach; 
    /// </summary>
    public class QueryData
    {
        #region właściwości publiczne
        /// <summary>
        /// zwraca liczbę wierszy danych
        /// </summary>
        public int dataRowsNumber { get { return readData.Count; } }
        public int dataColumnNumber { get => firstRow.Length; }
        public object[] firstRow { get { return readData.Count > 0 ? readData[0] : null; } }

        #endregion

        #region właściwości prywatne
        UtilityTools.NumberHandler nh = new UtilityTools.NumberHandler();

        private List<object[]> readData;
        private List<string> headers;
        private List<string> dataTypes;     //typy danych w kolumnach

        #endregion

        public QueryData()
        {
            readData = new List<object[]>();
            headers = new List<string>();
            dataTypes = new List<string>();
        }

        #region metody publiczne do uzyskiwania danych i informacji o danych
        public List<object[]> getQueryData()
        {
            return readData;
        }

        public List<string[]> getQueryDataAsStrings()
        {
            List<string[]> dataAsStrings = new List<string[]>();
            foreach(object[] rowData in readData)
            {
                string[] stringRowData = new string[rowData.Length];
                for(int i=0; i<rowData.Length; i++)
                {
                    string stringItem = rowData[i].ToString();
                    stringRowData[i] = stringItem;
                }
                dataAsStrings.Add(stringRowData);
            }
            return dataAsStrings;
        }

        /// <summary>
        /// zwraca przeczytane dane w postaci słownika, gdzie kluczem są wartości w kolumnie o indeksie podanym jako parametr
        /// kluczem jest string, żeby uniknąć duplikowania się wpisów, z którego usuwam wszystkie zbędne spacje metodą Trim()
        /// </summary>
        public Dictionary<string, object[]> getQueryDataAsDictionary(int keyColumnIndex)
        {
            Dictionary<string, object[]> dataDict = new Dictionary<string, object[]>();
            foreach(object[] dataRow in readData)
            {
                int index = 0;
                object[] dictValues = new object[dataRow.Length - 1];
                for(int i = 0; i < dataRow.Length; i++)
                {
                    if(i != keyColumnIndex)
                    {
                        dictValues[index] = dataRow[i];
                        index++;
                    }                        
                }
                if (!dataDict.ContainsKey(dataRow[keyColumnIndex].ToString().Trim()))
                    dataDict.Add(dataRow[keyColumnIndex].ToString().Trim(), dictValues);
            }
            return dataDict;
        }

        /// <summary>
        /// zwraca przeczytane dane w postaci słownika, gdzie kluczem są wartości w kolumnie o nazwie podanej jako parametr
        /// kluczem jest string, żeby uniknąć duplikowania się wpisów, z którego usuwam wszystkie zbędne spacje metodą Trim()
        /// </summary>
        public Dictionary<string, object[]> getQueryDataAsDictionary(string columnName)
        {            
            return getQueryDataAsDictionary(getHeaderIndex(columnName)); ;
        }


        public List<string> getHeaders()
        {
            return headers;
        }

        /// <summary>
        /// typy danych sql w poszczególnyc polach
        /// </summary>
        public List<string> getDataTypes()
        {
            return dataTypes;
        }

        /// <summary>
        /// szerokości kolumn obliczone na podstawie długości tekstu; jeżeli jako argument nie jest podany font użyty jest FontFamily.GenericSansSerif, 8.25F, FontStyle.Regular; 
        /// parametr defaultSampleSize określa liczność próbki wyników, na podstawie której określana jest szerokość kolumny
        /// </summary>
        /// <returns></returns>
        public List<int> getColumnWidths(Font font = null, int defaultSampleSize = 10)
        {
            if (font == null)
            {
                font = new Font(FontFamily.GenericSansSerif, 8.25F, FontStyle.Regular);
            }
            List<int> columnWidths = new List<int>();     //szerokości kolumn odczytane z zapisanych danych i nagłówków
            int sampleSize = Math.Min(defaultSampleSize, readData.Count);   //próbka pobrana z ilości domyślnej, chyba że tabela wyników jest mniej liczna
            for (int k = 0; k < headers.Count; k++)
            {
                List<int> widths = new List<int>();      //zawiera próbki szerokości jednej kolumny, też nagłówka
                for (int i = 0; i < sampleSize; i++)
                {
                    string str = readData[i][k].ToString();
                    int colWidth = computeSizeOfString(str, font) + 5;     //muszę dodać inaczej szerokość kolumny jest jednak za mała, tekst się nie mieści
                    widths.Add(colWidth);
                }
                widths.Add(computeSizeOfString(headers[k], font) + 5);
                int maxWidth = getMaxWidth(widths);
                columnWidths.Add(maxWidth);
            }

            return columnWidths;
        }

        private int computeSizeOfString(string text, Font font)
        {
            System.Drawing.Image fakeImage = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(fakeImage);
            return nh.tryGetInt( graphics.MeasureString(text, font).Width);
        }

        /// <summary>
        /// zwraca dane z wybranej kolumny w postaci listy
        /// </summary>
        public List<object> getColumnDataAsList(int columnNr = 0)
        {
            List<object> columnData = new List<object>();
            for (int i = 0; i < readData.Count; i++)
            {
                object columnItem = readData[i][columnNr];
                columnData.Add(columnItem);
            }
            return columnData;
        }

        /// <summary>
        /// zwraca dane z wybranej kolumny w postaci listy
        /// </summary>
        public List<string> getColumnDataAsStringList(int columnNr = 0)
        {
            List<string> columnData = new List<string>();
            for (int i = 0; i < readData.Count; i++)
            {
                string columnItem = readData[i][columnNr].ToString();
                columnData.Add(columnItem);
            }
            return columnData;
        }

        /// <summary>
        /// zwraca dane z wybranej kolumny w postaci listy
        /// </summary>
        public List<string> getColumnDataAsStringList(string columnName)
        {
            return getColumnDataAsStringList(getHeaderIndex(columnName));
        }

        /// <summary>
        /// zwraca indeks wyniku o podanym nagłówku, w razie braku zwraca -1
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public int getHeaderIndex(string headerName)
        {
            for (int index = 0; index < headers.Count; index++)
            {
                if (headers[index].Equals(headerName))
                    return index;
            }
            return -1;
        }
        /// <summary>
        /// zwraca wartość na podanej pozycji
        /// </summary>
        public object getDataValue(int rowIndex, string headerName)
        {
            int columnIndex = getHeaderIndex(headerName);
            if (rowIndex < readData.Count && columnIndex != -1)
                return readData[rowIndex][columnIndex];
            return null;
        }

        /// <summary>
        /// zwraca wartość na podanej pozycji
        /// </summary>
        public object getDataValue(int rowIndex, int columnIndex)
        {
            if (rowIndex < readData.Count && columnIndex < dataColumnNumber)
                return readData[rowIndex][columnIndex];
            return null;
        }

        #endregion

        #region metody wewnętrzne wykorzystywane przez DBReader

        internal void addQueryData(object[] rowData)
        {
            readData.Add(rowData);
        }

        internal void addHeader(string header)
        {
            headers.Add(header);
        }

        internal void addDataType(string dataType)
        {
            dataTypes.Add(dataType);
        }

        #endregion

        #region metody prywatne
        private int getMaxWidth(List<int> widths)
        {
            int width = 1;
            foreach (int w in widths)
            {
                if (w > width)
                {
                    width = w;
                }
            }
            return width;
        }

        #endregion

    }
}
