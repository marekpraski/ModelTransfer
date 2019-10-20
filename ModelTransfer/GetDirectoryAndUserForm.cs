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
        private DBReader reader;

        public GetDirectoryAndUserForm(DBReader reader)
        {
            this.reader = reader;
            InitializeComponent();
            directoryTreeControl1.directorySelectedEvent += onDirectorySelected_TreeViewNodeSelected;
            populateUserListview();
            directoryTreeControl1.setUpTreeview(reader);
        }

        private void onDirectorySelected_TreeViewNodeSelected(object sender, MyEventArgs args)
        {

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
    }
}
