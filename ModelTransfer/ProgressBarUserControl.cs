using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModelTransfer
{
    public partial class ProgressBarUserControl : UserControl
    {
        public ProgressBarUserControl()
        {
            InitializeComponent();
        }

        public void setLabel1Text(string text)
        {
            label1.Text = "text";
        }
    }
}
