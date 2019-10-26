using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModelTransfer
{
    public partial class Form1 : Form
    {
        BackgroundWorker bgw = new BackgroundWorker();
        ModelBundle modelBundle;
        private string currentPath = "";
        public Form1()
        {
            InitializeComponent();
            label1.Text = "";
            label2.Text = "";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            bgw.DoWork += new DoWorkEventHandler(bgw_DoSth);
            bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            bgw.WorkerReportsProgress = true;
            bgw.RunWorkerAsync();
        }

        void bgw_DoSth(object sender, DoWorkEventArgs e)
        {
            //int total = 57; //some number (this is your variable to change)!!

            //for (int i = 0; i <= total; i++) //some number (total)
            //{
            //    System.Threading.Thread.Sleep(100);
            //    int percents = (i * 100) / total;
            //    bgw.ReportProgress(percents, i);
            //    //2 arguments:
            //    //1. procenteges (from 0 t0 100) - i do a calcumation 
            //    //2. some current value!
            //}



            FileManipulator fm = new FileManipulator();
            string fileName = "modele.bin";
            string filePath = currentPath;
            modelBundle = new ModelBundle();

            FileInfo fileInfo;
            long length;

            try
            {
                if (fm.assertFileExists(filePath + @"\" + fileName))
                {
                    fileInfo = new FileInfo(filePath + @"\" + fileName);
                    length = fileInfo.Length;
                    long currentPosition = 0;

                    //deserialize
                    using (Stream stream = File.Open(filePath + @"\" + fileName, FileMode.Open))
                    {
                        var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                        while (stream.CanRead == false)
                        {
                            currentPosition += stream.Position;
                            int percents = (int)(((decimal)currentPosition / (decimal)length) * (decimal)100);

                            modelBundle = (ModelBundle)bformatter.Deserialize(stream);

                            bgw.ReportProgress(percents, currentPosition);
                        }
                    }
                }
            }
            catch (ArgumentException ex)
            {
                MyMessageBox.display(ex.Message, MessageBoxType.Error);
            }
        }

        void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label1.Text = String.Format("Progress: {0} %", e.ProgressPercentage);
            label2.Text = String.Format("Total items transfered: {0}", e.UserState);
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //do the code when bgv completes its work
        }
    }
}
