using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTransfer
{
    //podczas edycji datagridu - sprawdza czy wartość wpisywana przez użytkownika odpowiada typowi bazodanowemu tej celki
    //podczas tworzenia kwerendy zapisującej wartość celki do bazy konwertuje tę wartość na taki ciąg znaków, który można bezpośrednio
    //wpisać do kwerendy
    public class DBValueTypeConverter
    {       
        public bool verifyCellDataType(object value, string typeName)
        {
            // rozważam bazodanowe typy danych:  bit, int, bigint, oraz  w grupach: (char, varchar), (float, decimal, numeric), (datetime), (geometry)

            if (value != null)
            {
                if (typeName.Contains("bigint"))
                {
                    return handleBigint(value);
                }
                else if (typeName.Contains("bit"))
                {
                    return handleBit(value);
                }
                else if (typeName.Contains("int"))
                {
                    return handleInt(value);
                }
                else if (typeName.Contains("char"))
                {
                    return true;
                }
                else if (typeName.Contains("float") || typeName.Contains("decimal") || typeName.Contains("numeric"))
                {
                    return handleDouble(value);
                }
                else if (typeName.Contains("date"))
                {
                    return handleDatetime(value);
                }
                else if (typeName.Contains("geometry"))
                {
                    MyMessageBox.display("Nie można edytować pól typu geometry", MessageBoxType.Error);
                }
                return false;
            }
            return true;
        }

        private bool handleInt(object objectValue)
        {
            try
            {
                int value = int.Parse(objectValue.ToString());    //zwykły parse zapisany jako  (int)cellValue  nie działa
                return true;
            }
            catch (FormatException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić liczbę całkowitą", MessageBoxType.Error);
            }
            catch (OverflowException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić liczbę całkowitą od -32,768 do 32,767 ", MessageBoxType.Error);
            }
            return false;
        }

        private bool handleBit(object objectValue)
        {
            try
            {
                int value = int.Parse(objectValue.ToString());
                if (value == 1 || value == 0)
                {
                    return true;
                }
                else
                {
                    MyMessageBox.display("\r\nNależy wprowadzić liczbę 0 lub 1", MessageBoxType.Error);
                    return false;
                }
            }
            catch (FormatException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić liczbę 0 lub 1", MessageBoxType.Error);
            }
            catch (OverflowException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić liczbę 0 lub 1", MessageBoxType.Error);
            }
            return false;
        }

        private bool handleBigint(object objectValue)
        {
            try
            {
                long value = long.Parse(objectValue.ToString());
                return true;
            }
            catch (FormatException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić liczbę całkowitą", MessageBoxType.Error);
            }
            return false;
        }

        private bool handleDatetime(object objectValue)
        {
            try
            {
                DateTime value = DateTime.Parse(objectValue.ToString());
                return true;
            }
            catch (FormatException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić datę", MessageBoxType.Error);
            }
            return false;
        }

        private bool handleDouble(object objectValue)
        {
            string stringCellValue = objectValue.ToString(); 
            try
            {
                double value = Double.Parse(stringCellValue);
                return true;
            }
            catch (InvalidCastException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić liczbę", MessageBoxType.Error);
            }
            catch (FormatException ex)
            {
                MyMessageBox.display(ex.Message + "\r\nZnak separatora dziesiętnego musi być taki jaki jest ustawiony w systemie");
            }
            return false;
        }
        
        //skoro wartość przeszła pierwsze sprawdzenie funkcją verifyCellDataType to ta funkcja musi rozważyć tylko trzy przypadki:
        //czy zwrócona wartość nie jest nullowa (użytkownik skasował zawartość celki)
        //zwrócić string w apostrofach i zamienić przecinek na kropkę w double
        public string getConvertedValue(object objectValue, string typeName)
        {
            if (objectValue != null)
            {
                string convertedValue = objectValue.ToString();      //zamieniam każdą wartość na string żeby móc łatwo złożyć kwerendę
                if (typeName.Contains("float") || typeName.Contains("decimal") || typeName.Contains("numeric"))
                {
                    return convertedValue.Replace(",", ".");
                }
                else if (typeName.Contains("char") || typeName.Contains("date"))
                {
                    return "'" + convertedValue + "'";
                }
                return convertedValue;
            }
            return null;
        }

    }
}
