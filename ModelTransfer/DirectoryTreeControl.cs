using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ModelTransfer
{
    public partial class DirectoryTreeControl : UserControl
    {

        protected Dictionary<string, ModelDirectory> directoryDict = new Dictionary<string, ModelDirectory>();
        protected List<ModelDirectory> baseTreeDirectories = new List<ModelDirectory>();                //zawiera id wszystkich katalogów, które są parentami

        public delegate void DirectorySelectedEventHandler(object sender, MyEventArgs args);
        public event DirectorySelectedEventHandler directorySelectedEvent;

        public DirectoryTreeControl()
        {
            InitializeComponent();
        }

        public void setUpTreeview(DBReader reader)
        {
            getDirectories(reader);
            populateTreeview();

        }


        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            onDirectorySelected();
        }


        public virtual void onDirectorySelected()
        {
            if(directorySelectedEvent != null)
            {
                MyEventArgs args = new MyEventArgs();
                args.selectedDirectoryId = treeView1.SelectedNode.Name;
                directorySelectedEvent(this, args);
            }
        }

        private void getDirectories(DBReader reader)
        {
            ModelDirectory directory;
            string query = SqlQueries.getDirectories;
            List<string[]> directoryData = reader.readFromDB(query).getQueryDataAsStrings();
            foreach (string[] row in directoryData)
            {
                directory = new ModelDirectory();
                directory.parentId = row[SqlQueries.getDirectories_parentIdIndex];
                directory.id = row[SqlQueries.getDirectories_directoryIdIndex];
                directory.name = row[SqlQueries.getDirectories_directoryNameIndex];

                if (directory.parentId == null || directory.parentId == "")
                    baseTreeDirectories.Add(directory);

                directoryDict.Add(directory.id, directory);
            }

            assignChildren();
        }

        public void assignChildren()
        {
            ModelDirectory dir;
            ModelDirectory parentDir;
            foreach(string dirId in directoryDict.Keys)
            {
                directoryDict.TryGetValue(dirId, out dir);
                if (dir.parentId != null && dir.parentId != "")
                {
                    directoryDict.TryGetValue(dir.parentId, out parentDir);
                    parentDir.addChild(dir);
                }
            }
        }


        private void populateTreeview()
        {
            foreach (ModelDirectory dir in baseTreeDirectories)
            {
                treeView1.Nodes.Add(createDirectoryNode(dir));
            }
        }

        public TreeNode createDirectoryNode( ModelDirectory dir)
        {
            var dirNode = new TreeNode(dir.name);
            dirNode.Name = dir.id;
            if (dir.isParent())
            {
                foreach (var child in dir.children)
                {
                    dirNode.Nodes.Add(createDirectoryNode(child));
                }
            }
            return dirNode;
        }

    }
}
