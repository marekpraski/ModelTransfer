using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTransfer
{
    public class MyEventArgs : EventArgs
    {
        public string fileName { get; set; }
        public string selectedDirectoryId { get; set; }
    }
}
