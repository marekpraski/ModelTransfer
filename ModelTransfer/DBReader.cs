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
        private string log = "";

        public DBReader(SqlConnection dbConnection)
        {
            this.dbConnection = dbConnection;
            
        }

        public QueryData readFromDB(string sqlQuery)
        {
            log += "\r\n1  " +sqlQuery;         //dodałem zapisywanie loga dla celów debugowania
            queryData = new QueryData();

            try
            {
                log += "\r\n2";
                SqlCommand command = new SqlCommand(sqlQuery, dbConnection);
                command.CommandTimeout = ProgramSettings.commandTimeout;
                dbConnection.Open();

                log += "\r\n3";
                SqlDataReader sqlReader = command.ExecuteReader();

                int numberOfColumns = sqlReader.FieldCount;

                log += "\r\n4";

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
                log += "\r\n5";

                sqlReader.Close();
                log += "\r\n6";
                command.Dispose();
                log += "\r\n7";
                dbConnection.Close();
                log += "\r\n8";
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                MyMessageBox.display(e.Message + "\r\n" + dbConnection.ConnectionString, MessageBoxType.Error);
            }
            catch (InvalidOperationException exc)
            {
               MyMessageBox.display(exc.Message + "  \r\n DBReader - readFromDb \r\n" + log);
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
                dbConnection.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlCommand);

                da.Fill(data);

                da.Dispose();
                sqlCommand.Dispose();
                dbConnection.Close();
            }
            catch(SqlException e)
            {
                MyMessageBox.display(e.Message + "\r\n" + dbConnection.ConnectionString, MessageBoxType.Error);
            }
            catch (InvalidOperationException exc)
            {
                MyMessageBox.display(exc.Message + "  \r\n DBReader - readFromDBToDataTable \r\n" + log);
                if (dbConnection.State == ConnectionState.Open) dbConnection.Close();
            }
            return data;
        }
    }
}
