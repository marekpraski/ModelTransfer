using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModelTransfer
{
    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try { 
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(args[0], args[1]));
                //Application.Run(new Form1());
        }
            catch(IndexOutOfRangeException ex)
            {
                MyMessageBox.display(ex.Message + "  \r\nniewłaściwa liczba parametrów w pliku bat", MessageBoxType.Error);
            }
}
    }
}
