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
        private string fileName;

        public GetDirectoryAndUserForm(DBReader reader, string fileName)
        {
            this.reader = reader;
            this.fileName = fileName;
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

        private void acceptButton_Click(object sender, EventArgs e)
        {
            if (userListView.SelectedItems.Count >0 && selectedDirId != "")
            {
                selectedUserId = userListView.SelectedItems[0].Name;          //multiselect jest ustawiony na false
                onAcceptButtonClick();
                this.Close();
                this.Dispose();
            }
            else if(userListView.SelectedItems.Count > 0 && selectedDirId == "")
            {
                if (MyMessageBox.display("Nie wybrano katalogu docelowego, będzie nim pień drzewa. Czy kontynuować?", MessageBoxType.YesNo) == MyMessageBoxResults.Yes)
                {
                    selectedUserId = userListView.SelectedItems[0].Name;          //multiselect jest ustawiony na false
                    onAcceptButtonClick();
                    this.Close();
                    this.Dispose();
                }
            }
            else
            {
                MyMessageBox.display("Należy przynajmniej wybrać użytkownika");
            }
        }

        protected virtual void onAcceptButtonClick()
        {
            if (acceptButtonClickedEvent != null)
            {
                MyEventArgs args = new MyEventArgs();
                args.selectedDirectoryId = this.selectedDirId;
                args.selectedUserId = this.selectedUserId;
                args.fileName = this.fileName;
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
    }
}
