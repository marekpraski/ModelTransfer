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


    public partial class GetFileNameForm : Form
    {


        public delegate void GetFileNameEventHandler(object sender, MyEventArgs args);
        public event GetFileNameEventHandler GetFileNameEvent;

        public GetFileNameForm()
        {
            InitializeComponent();
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            OnGetFileName();
            this.Close();
            this.Dispose();

        }

        protected virtual void OnGetFileName()
        {
            if(GetFileNameEvent != null)
            {
                MyEventArgs args = new MyEventArgs();
                args.fileName = textBox1.Text;
                GetFileNameEvent(this, args);
            }
        }

        private void GetFileNameForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                OnGetFileName();
                this.Close();
                this.Dispose();
            }
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                OnGetFileName();
                this.Close();
                this.Dispose();
            }
        }

        private void Button1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                OnGetFileName();
                this.Close();
            }
        }
    }
}
