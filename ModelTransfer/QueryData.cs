using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModelTransfer
{
    [Serializable]
    public class QueryData
    {
        public DataSet dataSet { get; set; }
        public DataTable dataTable { get; set; }
        private List<object[]> readData;
        private List<string> headers;
        private List<string> dataTypes;     //typy danych w kolumnach

        public QueryData()
        {
            readData = new List<object[]>();
            headers = new List<string>();
            dataTypes = new List<string>();
        }

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


        public List<string> getHeaders()
        {
            return headers;
        }

        public void addQueryData(object[] rowData)
        {
            readData.Add(rowData);
        }        

        public void addHeader(string header)
        {
            headers.Add(header);
        }

        public void addDataType(string dataType)
        {
            dataTypes.Add(dataType);
        }

        public List<string> getDataTypes()
        {
            return dataTypes;
        }

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
                    int colWidth = TextRenderer.MeasureText(str, font).Width + 5;      //muszę dodać inaczej szerokość kolumny jest jednak za mała, tekst się nie mieści
                    widths.Add(colWidth);
                }
                widths.Add(TextRenderer.MeasureText(headers[k], font).Width + 5);
                int maxWidth = getMaxWidth(widths);
                columnWidths.Add(maxWidth);
            }

            return columnWidths;
        }


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

        public int getDataRowsNumber()
        {
            return getColumnDataAsList().Count;
        }


    }
}
