using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModelTransfer
{
    public partial class ProgressBarForm : Form
    {
        public ProgressBarForm()
        {
            InitializeComponent();
        }

        public void onReceivingProgress(object sender, ProgressEventArgs args)
        {
            this.progressBar1.Value = args.progressPercentage;
            this.label1.Text = args.labelText;
        }
    }
}
