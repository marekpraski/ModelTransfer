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
        public string selectedUserId { get; set; }
        public bool checkedDirectoriesExist { get; set; }

        public MyEventArgs()
        {
        }

    }
}
