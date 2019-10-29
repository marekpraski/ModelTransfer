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

        #region Region - ustawienia tej formatki

        public void setUpThisForm(DBReader reader)
        {
            getDirectories(reader);
            populateTreeview();
        }

        public void resetThisForm()
        {
            treeView1.Nodes.Clear();
            checkedDirectories.Clear();
            directoryDict.Clear();
            baseTreeDirectories.Clear();
        }

        public void turnTreeviewCheckboxesOn()
        {
            treeView1.CheckBoxes = true;
        }

        public void showUncheckAllCheckboxesLabel()
        {
            uncheckAllLabel.Visible = true;
        }

        #endregion


        #region Region - zdarzenia wywołane przez interakcję użytkownika z kontrolkami tej formatki


        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            onDirectorySelected();
        }


        private void uncheckAllButton_Click(object sender, EventArgs e)
        {

        }


        private void UncheckAllLabel_Click(object sender, EventArgs e)
        {
            if (checkedDirectories.Count > 0)
            {
                uncheckAllCheckboxes();
            }
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


        private void uncheckAllCheckboxes()
        {
            //klonuję zbiór zaznaczonych gałęzi, bo gdy czytam z głównego zbioru, to zdarzenie odznaczania gałęzi powoduje usunięcie gałęzi z tego zbioru, czyli zmianą zawartości zbioru i to wywala błąd na pętli
            Dictionary<string, ModelDirectory> copyOfCheckedDirs = new Dictionary<string, ModelDirectory>();
            foreach(ModelDirectory dir in checkedDirectories.Values)
            {
                copyOfCheckedDirs.Add(dir.id, dir);
            }

            TreeNode[] nodes;
            foreach(ModelDirectory dir in copyOfCheckedDirs.Values)
            {
                nodes = treeView1.Nodes.Find(dir.id, true);
                nodes[0].Checked = false;                   //zawsze jest tylko jedna gałąź, id jest unikalne
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
