using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTransfer
{
    public class ProgressEventArgs : EventArgs
    {
        public string labelText {get; set;}
        public int progressPercentage { get; set; }
    }
}
