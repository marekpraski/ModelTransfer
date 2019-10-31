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
                        MyMessageBox.display(e.Message, MessageBoxType.Error);
                    }
                    catch (InvalidOperationException ex)
                    {
                        MyMessageBox.display(ex.Message, MessageBoxType.Error);
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
            }
            catch (Exception exc)
            {
                MyMessageBox.display(exc.Message + "\r\n DBWriter, writeBulkDataToDB");
            }
            dbConnection.Close();


        }



        public DataTable convertToDataTable<T>(List<T> items, List<string> tableHeaders, List<string> columnDataTypes)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            //PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //foreach (PropertyInfo prop in Props)
            //{
            //    //Defining type of data column gives proper data table 
            //    var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
            //    //Setting column names as Property names
            //    dataTable.Columns.Add(prop.Name, type);
            //}



            for(int i =0; i<tableHeaders.Count; i++)
            {
                string typeDescription = columnDataTypes[i];
                DataColumn col;
                switch (typeDescription)
                {
                    case "int":
                        col = new DataColumn(tableHeaders[i]);
                        col.DataType = Type.GetType("System.Int32");
                        dataTable.Columns.Add(col);
                        break;
                    case "float":
                        col = new DataColumn(tableHeaders[i]);
                        col.DataType = Type.GetType("System.Double");
                        dataTable.Columns.Add(col);
                        break;
                    case "decimal":
                        col = new DataColumn(tableHeaders[i]);
                        col.DataType = Type.GetType("System.Decimal");
                        dataTable.Columns.Add(col);
                        break;
                    case "bit":
                        col = new DataColumn(tableHeaders[i]);
                        col.DataType = Type.GetType("System.Boolean");
                        dataTable.Columns.Add(col);
                        break;
                    default:
                        col = new DataColumn(tableHeaders[i]);
                        col.DataType = Type.GetType("System.String");
                        dataTable.Columns.Add(col);
                        break;
                }
            }

            foreach (T item in items)
            {
                                //var values = new object[Props.Length];
                                //for (int i = 0; i < Props.Length; i++)
                                //{
                                //    //inserting property values to datatable rows
                                //    values[i] = Props[i].GetValue(item, null);
                                //}
                dataTable.Rows.Add(item);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}
