using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ModelTransfer
{
    class DBWriter
    {
        private SqlConnection dbConnection;

        public DBWriter(SqlConnection connection)
        {
            this.dbConnection = connection;
        }

        public void writeToDB(string sqlQuery)
        {
            List<string> queries = new List<string>();
            queries.Add(sqlQuery);
            writeToDB(queries);
        }    
        
        public void writeToDB(List<string> queries)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            dbConnection.Open();
            foreach (string query in queries)
            {
                if (query != null)
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(query, dbConnection);
                        adapter.InsertCommand = command;
                        adapter.InsertCommand.ExecuteNonQuery();
                        command.Dispose();
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        MyMessageBox.display(e.Message, MessageBoxType.Error);
                    }
                    catch (InvalidOperationException ex)
                    {
                        MyMessageBox.display(ex.Message, MessageBoxType.Error);
                    }
                }
            }
            dbConnection.Close();
        }
    }
}
