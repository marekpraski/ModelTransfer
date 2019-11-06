using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace ModelTransfer
{
    public partial class Form1 : Form
    {
        public delegate void ProgressEventHandler(object sender, ProgressEventArgs args);
        public event ProgressEventHandler ProgressEvent;
        private int percent;
        public Thread computationsThread;

        ModelBundle modelBundle;
        private string currentPath = "";

        System.Timers.Timer timer1;
        public Form1()
        {
            InitializeComponent();


        }

        #region
        private void startTimer()
        {
            timer1 = new System.Timers.Timer();
            timer1.Enabled = true;
            timer1.Interval = 2000;
            timer1.Start();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Create a timer and set a two second interval.
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Interval = 2000;

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            
            //showProgress();
        }

        void runLoopComputations()
        {
            int loopTimes = 20;// int.Parse(textBox1.Text);
            for(int i=0; i<loopTimes; i++)
            {

                percent = (i+1) * 100 / 20;
                showProgress(percent, i);


                Thread.Sleep(1000);
            }
        }

        public delegate void showProgressDelegate(int percent, int i);

        public void showProgress(int percent, int i)
        {
            //TextProgressBar textProgressBar = new TextProgressBar();
            //textProgressBar.VisualMode =  ProgressBarDisplayMode.Percentage;
            if(this.InvokeRequired)
            {
                this.Invoke(new showProgressDelegate(showProgress), percent, i);
            }
            else
            {
                progressBar1.Value = percent;
                label1.Text = i.ToString();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {

            computationsThread = new Thread(new ThreadStart(runLoopComputations));
            computationsThread.Start();


        }
        private delegate void showMsgDel(object sender2, ElapsedEventArgs e2, string msg);

        showMsgDel msd;
        private void Button3_Click(object sender, EventArgs e)
        {
            startTimer();
            timer1.Elapsed += timer_Tic;
            msd = new showMsgDel(show_msg);
        }

        private void timer_Tic(object s2, ElapsedEventArgs e2)
        {
            string msg = "jajajaj";
            msd(s2, e2, msg);
        }

        private void show_msg(object sender, ElapsedEventArgs e, string s)
        {
            MyMessageBox.displayAndClose(s, 1)
;        }
        

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string[] fileNames = openFileDialog1.FileNames;
            string ff = "";
            foreach (string fn in fileNames)
            {
                ff += fn;
            }
            MyMessageBox.display(ff);
        }


        #endregion


        private void Button6_Click(object sender, EventArgs e)
        {



            //int position = int.Parse(textBox4.Text);
            //position = position*8;


            //long Data =long.Parse(textBox3.Text);


            //long s = stream.Length;


            //        //stream.Position = position;
            //        binWriter.Write(Data);

        }

        private void Button7_Click(object sender, EventArgs e)
        {
            //int position = int.Parse(textBox5.Text);


            //byte[] dataPacketReadFromStream = new byte[16];


            //stream.Position = position;

            //r.Read(dataPacketReadFromStream, 0, 16);      //dane do zmiennej dataRwadFromStream zapisuje od pozycji 0 streamu, na podaną liczbę bajtów
            long number = binReader.ReadInt64();

            //stream.Position = position;
            //stream.Read(Data, position, 8);



            textBox6.Text = number.ToString();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            //string serializationFile = @"C:\testDesktop\conf\testFileNotCompressed.bin";

            ////serialize
            //stream = File.Open(serializationFile, FileMode.Open);
            //long l = stream.Length;

            //binWriter = new BinaryWriter(stream);
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            //string serializationFile = @"C:\testDesktop\conf\testFileNotCompressed.bin";

            ////serialize
            //stream = File.Open(serializationFile, FileMode.Open);
            //binReader = new BinaryReader(stream);
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            //stream.Close();
        }


        BinaryFormatter bformatter;


        BinaryReader binReader;




        long headerLength;


        //tworzę plik, w nim file header i zapisuję jego długość na pozycji 0
        private void Button5_Click(object sender, EventArgs e)
        {
            string serializationFile = @"C:\testDesktop\conf\testFileNotCompressed.bin";

            FileStream stream = new FileStream(serializationFile, FileMode.Create);

            BinaryWriter binWriter = new BinaryWriter(stream);
            int i = int.Parse(textBox2.Text);
            long nr = 0;
            for
                (int k = 0; k<=i; k++)
            {
                binWriter.Write(nr+k);
            }
            headerLength = stream.Length;
            streamPositionForPacketReading = headerLength;

            //resetuję pozycję strumienia i zapisuję wielkość nagłówka
            stream.Position = 0;
            binWriter.Write(i+1);
            binWriter.Dispose();
            stream.Close();


        }



        int packetPosition = 0;


        //dopisuję tekst na końcu pliku oraz wielkość tego pakietu na wybranej pozycji
        private void Button10_Click(object sender, EventArgs e)
        {
            string log =  "headerSize  " + headerLength.ToString() + "\r\n";
            string serializationFile = @"C:\testDesktop\conf\testFileNotCompressed.bin";
            packetPosition++;
            textBox8.Text = packetPosition.ToString();
            string modelBundle = textBox7.Text;
            long packetSize;
            long fileSizeBeforeAppend;
            long fileSizeAfterAppend;
            byte[] data;
            byte[] data1;
            //serialize
            using (FileStream stream = new FileStream(serializationFile, FileMode.Open))
            {
                fileSizeBeforeAppend = stream.Length;
                log += "stream przed dopisaniem  " + stream.Length.ToString() + "\r\n";

                //paczkę zapisuję do strumienia w pamięci
                MemoryStream originalMemoryStream = new MemoryStream();
                MemoryStream compressedMemoryStream = new MemoryStream();

                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(originalMemoryStream, modelBundle);

                //paczkę kompresuję
                using (GZipStream gzipStream = new GZipStream(compressedMemoryStream, CompressionMode.Compress))
                    {
                    originalMemoryStream.WriteTo(gzipStream);
                }

                data = compressedMemoryStream.ToArray();     //zapisuję strumień do tablicy
                data1 = originalMemoryStream.ToArray();
                originalMemoryStream.Close();
                compressedMemoryStream.Close();

                

                packetSize = data.Length;
                BinaryWriter writer = new BinaryWriter(stream);
                stream.Position = stream.Length;
                writer.Write(data);

                //resetuję pozycję strumienia, żeby dopisać info w nagłówku
                stream.Position = 8 * packetPosition;
                writer.Write(packetSize);

                fileSizeAfterAppend = stream.Length;

                log += "before  " + fileSizeBeforeAppend.ToString() + "\r\n";
                log += "after  " + fileSizeAfterAppend.ToString() + "\r\n";
                log += "packetSize  " + packetSize.ToString() + "\r\n";
                writer.Dispose();
            }

            
            //MyMessageBox.display(log);
        }

        int packetNumber = 0;
        long streamPositionForPacketReading = 0;
        //przycisk odczytywania tekstu z podanej pozycji
        private void Button11_Click(object sender, EventArgs e)
        {
            string log = "";
            packetNumber++;
            textBox10.Text = packetNumber.ToString();
            string modelBundle = "";
            string serializationFile = @"C:\testDesktop\conf\testFileNotCompressed.bin";
            long numberOfDataPackets;
            long packetSize;
            int packetInfoPosition;
            long packetEndPosition;
            

            //czytam header pliku
            using (FileStream stream = new FileStream(serializationFile, FileMode.Open))
            {
                BinaryReader bReader = new BinaryReader(stream);
                
                numberOfDataPackets = bReader.ReadInt64();      //czytam jeden long od początku strumienia


                packetInfoPosition = 8*(packetNumber);          //czytam jeden long na pozycji strumienia, gdzie jest informacja o pakiecie
                stream.Position = packetInfoPosition;
                packetSize = bReader.ReadInt64();

                log += "numberOfDataPackets  " + numberOfDataPackets.ToString() + "\r\n";
                log += "packetInfoPosition  " + packetInfoPosition.ToString() + "\r\n";
                log += "packetSize  " + packetSize.ToString() +"\r\n";
                

            //deserialize

                byte[] buffer = new byte[(int)packetSize];


                if (packetNumber == 1)
                {
                    streamPositionForPacketReading = 8 * numberOfDataPackets;
                }

                stream.Position = streamPositionForPacketReading;
                bReader.Read(buffer, 0, buffer.Length);

                MemoryStream compressedMemoryStream = new MemoryStream(buffer);
                MemoryStream decompressedMemoryStream = new MemoryStream();
                using (GZipStream gzipStream = new GZipStream(compressedMemoryStream, CompressionMode.Decompress))
                {
                    gzipStream.CopyTo(decompressedMemoryStream);
                    
                }

                    BinaryFormatter bformatter = new BinaryFormatter();
                decompressedMemoryStream.Position = 0;
                    modelBundle = (string)bformatter.Deserialize(decompressedMemoryStream); 


            }
            streamPositionForPacketReading += packetSize;

            textBox9.Text = modelBundle;
            //MyMessageBox.display(log);
        }
    }

}
