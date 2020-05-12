using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace ModelTransfer
{
    public class DBReader
    {
        private SqlConnection dbConnection;
        private QueryData queryData;

        public DBReader(SqlConnection dbConnection)
        {
            this.dbConnection = dbConnection;
            
        }

        public QueryData readFromDB(string sqlQuery)
        {
            queryData = new QueryData();

            try
            {
                SqlCommand command = new SqlCommand(sqlQuery, dbConnection);
                command.CommandTimeout = ProgramSettings.commandTimeout;
                dbConnection.Open();

                using (SqlDataReader sqlReader = command.ExecuteReader())
                {
                    int numberOfColumns = sqlReader.FieldCount;

                    while (sqlReader.Read())
                    {
                        object[] rowData = new object[numberOfColumns];
                        for (int i = 0; i < numberOfColumns; i++)
                        {
                            rowData[i] = sqlReader.GetValue(i).ToString();
                        }
                        queryData.addQueryData(rowData);
                    }

                    for (int i = 0; i < sqlReader.FieldCount; i++)
                    {
                        queryData.addHeader(sqlReader.GetName(i));
                        queryData.addDataType(sqlReader.GetDataTypeName(i));
                    }
                }
                command.Dispose();
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                MyMessageBox.display(e.Message + e.StackTrace + "\r\n" + dbConnection.ConnectionString, MessageBoxType.Error);
            }
            catch (InvalidOperationException exc)
            {
               MyMessageBox.display(exc.Message + exc.StackTrace + "  \r\n DBReader - readFromDb \r\n");
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open) dbConnection.Close();
            }

            return queryData;
        }

        public DataTable readFromDBToDataTable(string sqlQuery)
        {
            DataTable data = new DataTable("data");

            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlQuery, dbConnection);
                sqlCommand.CommandTimeout = ProgramSettings.commandTimeout;
                dbConnection.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlCommand);

                da.Fill(data);

                da.Dispose();
                sqlCommand.Dispose();
            }
            catch(SqlException e)
            {
                MyMessageBox.display(e.Message + e.StackTrace + "\r\n" + dbConnection.ConnectionString, MessageBoxType.Error);
            }
            catch (InvalidOperationException exc)
            {
                MyMessageBox.display(exc.Message + exc.StackTrace + "  \r\n DBReader - readFromDBToDataTable \r\n");
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open) dbConnection.Close();
            }
            return data;
        }
    }
}
