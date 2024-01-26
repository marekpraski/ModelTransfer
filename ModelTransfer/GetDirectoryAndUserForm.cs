using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DatabaseInterface;

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
            setupThisForm();
        }

        private void setupThisForm()
        {
            directoryTreeControl1.directorySelectedEvent += onDirectorySelected_TreeViewNodeSelected;
            populateUserListview();
            directoryTreeControl1.toolTipText = "nie wybieraj aby przywiązać odtwarzane katalogi do pnia";
            directoryTreeControl1.setUpThisForm(reader);
            
        }

        #region Region - zdarzenia wywołane interakcją użytkownika

        private void onDirectorySelected_TreeViewNodeSelected(object sender, MyEventArgs args)
        {
            selectedDirId = args.selectedDirectoryId;
        }


        private void InfoLabel_MouseEnter(object sender, EventArgs e)
        {
            displayTooltip();
        }


        private void acceptButton_Click(object sender, EventArgs e)
        {
            if (userListView.SelectedItems.Count > 0 && selectedDirId != "")
            {
                selectedUserId = userListView.SelectedItems[0].Name;          //multiselect jest ustawiony na false
                onAcceptButtonClick();
                this.Close();
            }
            else if (userListView.SelectedItems.Count > 0 && selectedDirId == "")
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
                MyMessageBox.display("Należy wybrać użytkownika");
            }
        }


        #endregion



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

        private void displayTooltip()
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 10000;
            toolTip1.InitialDelay = 10;
            toolTip1.ReshowDelay = 10;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text
            toolTip1.SetToolTip(infoLabel, "użytkownik który będzie właścicielem odtwarzanych modeli");
        }


    }
}
