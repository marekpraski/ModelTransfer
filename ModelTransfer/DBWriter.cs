using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

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
            foreach (string query in queries)
            {
                if (query != null)
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(query, dbConnection);
                        command.CommandTimeout = ProgramSettings.commandTimeout;
                        adapter.InsertCommand = command;

                        dbConnection.Open();
                        adapter.InsertCommand.ExecuteNonQuery();

                        command.Dispose();
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        MyMessageBox.display(e.Message + e.StackTrace, MessageBoxType.Error);
                    }
                    catch (InvalidOperationException ex)
                    {
                        MyMessageBox.display(ex.Message + ex.StackTrace, MessageBoxType.Error);
                    }
                    finally
                    {
                        if (dbConnection.State == ConnectionState.Open) dbConnection.Close();
                    }
                }
            }
            
        }


        public void writeBulkDataToDB(DataTable data, string tableName)
        {

            // make sure to enable triggers

            SqlBulkCopy bulkCopy = new SqlBulkCopy(dbConnection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

            bulkCopy.DestinationTableName = tableName;
            bulkCopy.BulkCopyTimeout = ProgramSettings.bulkCopyTimeout;

            dbConnection.Open();

            try
            {
                bulkCopy.WriteToServer(data); 

            }
            catch (System.InvalidOperationException ex)
            {
                MyMessageBox.display(ex.Message + ex.StackTrace + "\r\n DBWriter, writeBulkDataToDB");
            }
            catch (Exception exc)
            {
                MyMessageBox.display(exc.Message + exc.StackTrace + "\r\n DBWriter, writeBulkDataToDB");
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open) dbConnection.Close();
            }


        }
    }
}
