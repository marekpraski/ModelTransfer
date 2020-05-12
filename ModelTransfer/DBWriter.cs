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
                        dbConnection.Close();

                        command.Dispose();
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        MyMessageBox.display(e.Message + e.StackTrace, MessageBoxType.Error);
                    }
                    catch (InvalidOperationException ex)
                    {
                        MyMessageBox.display(ex.Message + ex.StackTrace, MessageBoxType.Error);
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
                bulkCopy.WriteToServer(data); //System.InvalidOperationException: „Podana wartość typu Object[] ze źródła danych nie może zostać przekonwertowana na typ varchar określonej kolumny docelowej.”
                                              //InvalidCastException: Obiekt musi implementować element IConvertible.

                //System.InvalidOperationException: „Podany element ColumnMapping nie jest zgodny z żadną kolumną w lokalizacji źródłowej lub docelowej.”
            }
            catch (System.InvalidOperationException ex)
            {
                MyMessageBox.display(ex.Message + "\r\n DBWriter, writeBulkDataToDB");
                if (dbConnection.State == ConnectionState.Open) dbConnection.Close();
            }
            catch (Exception exc)
            {
                MyMessageBox.display(exc.Message + "\r\n DBWriter, writeBulkDataToDB");
            }
            dbConnection.Close();


        }
    }
}
