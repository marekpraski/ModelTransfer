using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ModelTransfer
{
    public partial class GetDirectoryAndUserForm : Form
    {
        public delegate void AcceptButtonClickEventHandler(object sender, MyEventArgs args);
        public event AcceptButtonClickEventHandler acceptButtonClickedEvent;

        private DBReader reader;
        private string selectedDirId = "";
        private string selectedUserId = "";

        public GetDirectoryAndUserForm(DBReader reader)
        {
            this.reader = reader;
            InitializeComponent();
            directoryTreeControl1.directorySelectedEvent += onDirectorySelected_TreeViewNodeSelected;
            populateUserListview();
            directoryTreeControl1.setUpThisForm(reader);
        }

        private void onDirectorySelected_TreeViewNodeSelected(object sender, MyEventArgs args)
        {
            selectedDirId = args.selectedDirectoryId;
        }

        private void populateUserListview()
        {
            QueryData data = new QueryData();
            string query = SqlQueries.getUsers;
            data = reader.readFromDB(query);

            List<string[]> users = data.getQueryDataAsStrings();
            foreach (string[] user in users)
            {
                ListViewItem item = new ListViewItem();
                item.Text = user[SqlQueries.getUsers_uzytkownikIndex];
                item.Name = user[SqlQueries.getUsers_idUzytkownikIndex];
                userListView.Items.Add(item);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (userListView.SelectedItems.Count >0 && selectedDirId != "")
            {
                ListViewItem item = userListView.SelectedItems[0];          //multiselect = false
                selectedUserId = item.Name;
                onAcceptButtonClick();
                this.Close();
                this.Dispose();
            }
            else
            {
                MyMessageBox.display("Należy wybrać katalog docelowy i użytkownika");
            }
        }

        protected virtual void onAcceptButtonClick()
        {
            if (acceptButtonClickedEvent != null)
            {
                MyEventArgs args = new MyEventArgs();
                args.selectedDirectoryId = this.selectedDirId;
                args.selectedUserId = this.selectedUserId;
                if(restoreTreeRadioButton.Checked == true)
                {
                    args.restoreDirectoryTree = true;
                }
                else
                {
                    args.restoreDirectoryTree = false;
                }
                acceptButtonClickedEvent(this, args);
            }
        }

        private void restoreTreeRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void SaveToOneFolderRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
