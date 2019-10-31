﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
    }
}