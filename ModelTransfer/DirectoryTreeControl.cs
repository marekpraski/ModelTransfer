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

        public Dictionary<string, ModelDirectory> checkedDirectories { get; }       //kluczem jest id

        public delegate void DirectorySelectedEventHandler(object sender, MyEventArgs args);
        public event DirectorySelectedEventHandler directorySelectedEvent;

        public delegate void DirectoryCheckedEventHandler(object sender, MyEventArgs args);
        public event DirectoryCheckedEventHandler directoryCheckedEvent;


        public DirectoryTreeControl()
        {
            InitializeComponent();
            checkedDirectories = new Dictionary<string, ModelDirectory>();
        }

        public void setUpThisForm(DBReader reader)
        {
            getDirectories(reader);
            populateTreeview();
        }

        public void turnTreeviewCheckboxesOn()
        {
            treeView1.CheckBoxes = true;
        }

        #region Region - zdarzenia wywołane akcją użytkownika


        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            onDirectorySelected();
        }


        private void TreeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
                ModelDirectory dir =null;
                directoryDict.TryGetValue(e.Node.Name, out dir);

            if (e.Node.Checked == true)
            {
                checkedDirectories.Add(dir.id, dir);
                checkChildren(dir);
            }
            else
            {
                if (dir!=null && checkedDirectories.Keys.Contains(dir.id))
                checkedDirectories.Remove(dir.id);
            }
            onDirectoryChecked();
        }

        #endregion


        #region Region - funkcje wywołane zdarzeniami użytkownika

        protected virtual void onDirectorySelected()
        {
            if(directorySelectedEvent != null)
            {
                MyEventArgs args = new MyEventArgs();
                args.selectedDirectoryId = treeView1.SelectedNode.Name;
                directorySelectedEvent(this, args);
            }
        }

        private void onDirectoryChecked()
        {
            if (directoryCheckedEvent != null)
            {
                MyEventArgs args = new MyEventArgs();
                args.checkedDirectoriesExist = checkedDirectories.Count > 0;
                directoryCheckedEvent(this, args);
            }
        }

        private void checkChildren(ModelDirectory dir)
        {
            if (dir.isParent())
            {
                List<ModelDirectory> children = dir.children;
                foreach(ModelDirectory child in children)
                {
                    TreeNode[] nodes = treeView1.Nodes.Find(child.id, true);        //Directory.id to jest TreeNode.Name 
                    nodes[0].Checked = true;                //zawsze jest tylko jedna, bo index jest unikalny
                }
            }
        }

        private List<string> getCheckedDirectories()
        {
            return null;
        }



        #endregion

        private void getDirectories(DBReader reader)
        {
            try
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
            catch(NullReferenceException ex)
            {
                MyMessageBox.display(ex.Message + "  \r\nbłąd getDirectories");
            }
}

        private void assignChildren()
        {
            try { 
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
            catch (NullReferenceException ex)
            {
                MyMessageBox.display(ex.Message + "  \r\nbłąd assignChildren");
            }
        }


        private void populateTreeview()
        {
            try
            {
                foreach (ModelDirectory dir in baseTreeDirectories)
                {
                    treeView1.Nodes.Add(createDirectoryNode(dir));
                }
            }
            catch (NullReferenceException ex)
            {
                MyMessageBox.display(ex.Message + "  \r\nbłąd populateTreeview");
            }
        }

        private TreeNode createDirectoryNode( ModelDirectory dir)
        {
            var dirNode = new TreeNode(dir.name);
            try
            {
                dirNode.Name = dir.id;
                if (dir.isParent())
                {
                    foreach (var child in dir.children)
                    {
                        dirNode.Nodes.Add(createDirectoryNode(child));
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                MyMessageBox.display(ex.Message + "  \r\nbłąd createDirectoryNode");
            }
            return dirNode;
        }

    }
}
