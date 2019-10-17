using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace ModelTransfer
{
    class FileManipulator
    {        
        //podana musi być pełna ścieżka do obu plików
        //jeżeli istnieje już plik docelowy, zostaje nadpisany
        //oryginał zostaje zachowany
        
        public bool fileCopyAs(string file1, string file2)
        {
            if (File.Exists(file1))
            {
                if (File.Exists(file2))
                {
                    File.Delete(file2);
                }
                File.Copy(file1, file2);
                return true;
            }
            else
            {
                MyMessageBox.display("brak pliku " + file1, MessageBoxType.Error);
                return false;
            }
        }

        public string saveTextToFile(string file, string text)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.WriteAllText(file, text);
                    return "zmieniono plik konfiguracyjny " + file + "\r\n";
                }
                else
                {
                    MyMessageBox.display("plik " + file + " nie został znaleziony", MessageBoxType.Warning);
                    return "plik " + file + " nie został znaleziony\r\n";
                }
            }
            catch (DirectoryNotFoundException exc)
            {
                MyMessageBox.display(exc.Message, MessageBoxType.Error);
                return "plik " + file + " nie został znaleziony\r\n";
            }
        }

        public bool assertFileExists (string file)
        {
            try
            {
                return File.Exists(file);
            }
            catch (DirectoryNotFoundException exc)
            {
                MyMessageBox.display(exc.Message, MessageBoxType.Error);
                return false;
            }
            catch (FileNotFoundException exc)
            {
                MyMessageBox.display(exc.Message, MessageBoxType.Error);
                return false;
            }
        }

        
        public string readFile(string file)
        {
            string fileText = "";
            try
            {
                fileText = File.ReadAllText(file);
                if (fileText.Equals(""))
                {
                    MyMessageBox.display("plik " + file + " jest pusty");
                    return "";
                }
            }
            catch (DirectoryNotFoundException exc)
            {
                MyMessageBox.display(exc.Message, MessageBoxType.Error);
                return "";
            }
            catch (FileNotFoundException exc)
            {
                MyMessageBox.display(exc.Message, MessageBoxType.Error);
                return "";
            }
            return fileText;
        }
                       
       
        public void runProgram(string program, string arguments)
        {
            try
            {                
                System.Diagnostics.Process.Start(program, arguments); 
            }
            catch (System.ComponentModel.Win32Exception exc)
            {
                MyMessageBox.display(exc.Message + "\r\n" + program);
            }
            catch (DirectoryNotFoundException exc)
            {
                MyMessageBox.display(exc.Message, MessageBoxType.Error);                
            }
            catch(FileNotFoundException exc)
            {
                MyMessageBox.display(exc.Message, MessageBoxType.Error);
            }
        }     
    }
}
