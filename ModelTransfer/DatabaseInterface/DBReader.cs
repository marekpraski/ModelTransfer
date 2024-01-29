using System;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using UtilityTools;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;

namespace DatabaseInterface
{
    /// <summary>
    /// implementuje metody czytające z bazy danych sql
    /// </summary>
    public class DBReader
    {
        private SqlConnection dbConnection;
        private SqlCommand persistentSqlCommand;

        public DBReader(SqlConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        #region metody publiczne
        /// <summary>
        /// sprawdza czy przekazana kwerenda nie generuje błędu podczas uruchamiania
        /// </summary>
        public bool testQuery(string query)
        {
            try
            {
				SqlCommand sqlCommand = getSqlCommand(query);
				dbConnection.Open();
                SqlDataReader sqlReader = sqlCommand.ExecuteReader();
                return true;
			}
            catch (Exception)
            {
                return false;
            }
            finally
            {
                dbConnection.Close();
            }
        }
        /// <summary>
        /// zwraca listę nagłówków przekazanej tablicy bazodanowej bez czytania danych z tabeli;
        /// jeżeli dane zostały wcześniej przeczytane, lepiej jest użyć właściwości obiektu QueryData
        /// </summary>
        public List<string> getTableColumnNames(string tableName)
        {
            string query = "select top 0 * from " + tableName;
            QueryData qd = this.readFromDB(query);
            return qd.getHeaders();
        }

        /// <summary>
        /// zwraca wynik kwerendy w postaci obiektu DatabaseInterface.QueryData
        /// </summary>
        public QueryData readFromDB(string sqlQuery, int timeoutInSeconds = 30)
        {
            QueryData queryData = null;
            try
            {
                SqlCommand sqlCommand = getSqlCommand(sqlQuery);
                sqlCommand.CommandTimeout = timeoutInSeconds;
                dbConnection.Open();

                queryData = executeOneQuery(sqlCommand);
            }
            catch (Exception e)
            {
                if(this.dbConnection == null)
                    ErrorHandler.handleError("połączenie do bazy danych było null ", "błąd", "kwerenda: " + sqlQuery + "\r\n" + e.StackTrace);
                else
                    ErrorHandler.handleError(e.Message, "błąd", "kwerenda: " + sqlQuery + "\r\n" + e.StackTrace + "\r\n" + dbConnection.ConnectionString);
            }
            finally
            {
                if (dbConnection !=null && dbConnection.State == ConnectionState.Open) 
                    dbConnection.Close();
            }

            return queryData;
        }


        /// <summary>
        /// w przypadku prostych kwerend czas otwarcia połączenia do bazy danych może być znacząco dłuższy niż sam czas czytania z bazy danych; 
        /// ta metoda otwiera połączenie tylko raz dla wszystkich kwerend co może skrócić całkowity czas; timeout dotyczy wykonywania jednej kwerendy; 
        /// jeżeli jako jedną z kwerend przekaże się pusty string, w tym miejcu w wyniku będzie null
        /// </summary>
        public QueryData[] readFromDB(string[] sqlQueries, int timeoutInSeconds = 30)
        {
            QueryData[] results = new QueryData[sqlQueries.Length];
            try
            {
                dbConnection.Open();
                SqlCommand sqlCommand = getSqlCommand("");
                sqlCommand.CommandTimeout = timeoutInSeconds;

                for (int k = 0; k < sqlQueries.Length; k++)
                {
                    if (String.IsNullOrEmpty(sqlQueries[k]))
                        results[k] = null;
                    else
                    {
                        sqlCommand.CommandText = sqlQueries[k];
                        QueryData queryData = executeOneQuery(sqlCommand);
                        results[k] = queryData;
                    }
                }
            }
            catch (Exception e)
            {
                if (this.dbConnection == null)
                    ErrorHandler.handleError("połączenie do bazy danych było null ", "błąd", "kwerenda: " + sqlQueries + "\r\n" + e.StackTrace);
                else
                    ErrorHandler.handleError(e.Message, "błąd", "kwerenda: " + sqlQueries + "\r\n" + e.StackTrace + "\r\n" + dbConnection.ConnectionString);
            }
            finally
            {
                if (dbConnection != null && dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }

            return results;
        }

        /// <summary>
        /// zwraca wynik kwerendy w postaci obiektu C# DataTable
        /// </summary>
        public DataTable readFromDBToDataTable(string sqlQuery, int timeout = 30)
        {
			DataTable data = new DataTable("data");

            SqlCommand sqlCommand = getSqlCommand(sqlQuery);

            try
            {
                sqlCommand.CommandTimeout = timeout;
                dbConnection.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlCommand);

                da.Fill(data);

                da.Dispose();
                sqlCommand.Dispose();
            }
            catch(Exception e)
            {
                if (this.dbConnection == null)
                    ErrorHandler.handleError("połączenie do bazy danych było null ", "błąd", "kwerenda: " + sqlQuery + "\r\n" + e.StackTrace);
                else
                    ErrorHandler.handleError(e.Message, "błąd", "kwerenda: " + sqlQuery + "\r\n" + e.StackTrace + "\r\n" + dbConnection.ConnectionString);
            }
            finally
            {
                if (dbConnection != null && dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return data;
        }

        /// <summary>
        /// zwraca pierwszą kolumnę pierwszego wiersza kwerendy, wykorzystuje metodę SqlCommand.ExecuteScalar()
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public object readScalarFromDB(string sqlQuery, int timeout = 30)
        {
            SqlCommand sqlCommand = getSqlCommand(sqlQuery);

            object result = null;
            try
            {
                sqlCommand.CommandTimeout = timeout;
                dbConnection.Open();
                result = sqlCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                if (this.dbConnection == null)
                    ErrorHandler.handleError("połączenie do bazy danych było null ", "błąd", "kwerenda: " + sqlQuery + "\r\n" + e.StackTrace);
                else
                    ErrorHandler.handleError(e.Message, "błąd", "kwerenda: " + sqlQuery + "\r\n" + e.StackTrace + "\r\n" + dbConnection.ConnectionString);
            }
            finally
            {
                if (dbConnection != null && dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
                if (sqlCommand != null)
                    sqlCommand.Dispose();
            }
            return result;
        }

        /// <summary>
        /// zwraca Int a jeżeli parsowanie się nie uda, zwraca 0
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public int readIntFromDb(string sqlQuery, int timeout = 30)
        {
            object result = readScalarFromDB(sqlQuery, timeout);
            return new NumberHandler().tryGetInt(result);
        }

        #endregion

        #region obsługa komendy z parametrami
        /// <summary>
        /// stosować tylko wtedy, gdy chce się użyć komendy zaincjalizowanej parametrami; 
        /// przed uruchomieniem należy zainicjować komendę i przypisać jej parametry metodą addCommandParameter, np  dbreader.addCommandParameter("@parName", SqlDbType.VarBinary, varbinaryValue);
        /// </summary>
        public void initiateParameterizedCommand()
        {
            persistentSqlCommand = new SqlCommand(null, dbConnection);
        }

        public void defineCommmandParameter(string parameterName, SqlDbType parameterType)
        {
            this.persistentSqlCommand.Parameters.Add(parameterName, parameterType);
        }
        public void addCommmandParameterValue(string parameterName, object parameterValue)
        {
            this.persistentSqlCommand.Parameters[parameterName].Value = parameterValue;
        }

        public void addCommmandParameter(string parameterName, SqlDbType parameterType, object parameterValue)
        {
            this.persistentSqlCommand.Parameters.Add(parameterName, parameterType);
            this.persistentSqlCommand.Parameters[parameterName].Value = parameterValue;
        }

        #endregion

        #region metody prywatne

        private QueryData executeOneQuery(SqlCommand sqlCommand)
        {
            QueryData queryData = new QueryData();
            using (SqlDataReader sqlReader = sqlCommand.ExecuteReader())
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

            return queryData;
        }

        #endregion

        #region prywatne metody pomocnicze

        private string queriesAsString(string[] sqlQueries)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < sqlQueries.Length; i++)
            {
                sb.AppendLine(sqlQueries[i]);
            }
            return sb.ToString();
        }

        private SqlCommand getSqlCommand(string sqlQuery)
        {
            if (this.persistentSqlCommand == null)
                return new SqlCommand(sqlQuery, dbConnection);
            else
            {
                persistentSqlCommand.CommandText = sqlQuery;
                return this.persistentSqlCommand;
            }
        }
        #endregion
    }
}
