using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using UtilityTools;

namespace DatabaseInterface
{
    /// <summary>
    /// implementuje metody umożliwiające zapisywanie do bazy danych sql
    /// </summary>
    public class DBWriter
    {
        private SqlConnection dbConnection;
        private bool isInsideTransaction = false;
        private bool transactionWasInterrupted = false;
        private SqlTransaction transaction;
        private SqlCommand persistentSqlCommand;    //inicjowana gdy kwerenda jest sparametryzowana lub zapis wykonywany jest w transakcji

        public DBWriter(SqlConnection connection)
        {
            this.dbConnection = connection;
        }

        #region metody publiczne do zapisywania do bazy danych
        /// <summary>
        /// zwraca true jeżeli kwerenda przejdzie bez błędu, false jeżeli kwerenda zwraca błąd; jeżeli metoda wykonywana jest wewnątrz transakcji, błąd kwerendy przerywa i wycofuje transakcję; 
        /// jeżeli metoda wywoływana jest jako kolejna wewnątrz transakcji a któraś z poprzednich wygenerowała błąd, metoda nie będzie wykonana i zwróci false
        /// </summary>
        public bool executeQuery(string sqlQuery, int timeout = 30)
        {
            if (this.transactionWasInterrupted)
                return false;
            bool success = false;
            SqlCommand sCommand = getSqlCommand(sqlQuery);
            try
            {
                sCommand.CommandTimeout = timeout;
                if(!this.isInsideTransaction)
                    dbConnection.Open();
                sCommand.ExecuteNonQuery();
                success = true;
            }
            catch (Exception e)
            {
                ErrorHandler.handleError(e.Message, "błąd", "kwerenda: " + sqlQuery + "\r\n" + e.StackTrace + "\r\n" + dbConnection.ConnectionString);
                if (this.isInsideTransaction)
                {
                    this.transactionWasInterrupted = true;
                    tryRollbackTransaction();
                }
            }
            finally
            {
                if (!this.isInsideTransaction && dbConnection != null && dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return success;
        }

        /// <summary>
        /// otwiera połączenie tylko raz; zwraca true jeżeli wszystkie kwerendy przejdą bez błędu, false jeżeli kwerenda zwraca błąd; 
        /// jeżeli metoda wykonywana jest wewnątrz transakcji, błąd kwerendy przerywa i wycofuje transakcję; 
        /// jeżeli metoda wywoływana jest jako kolejna wewnątrz transakcji a któraś z poprzednich wygenerowała błąd, metoda nie będzie wykonana i zwróci false
        /// </summary>
        public bool executeQuery(string[] sqlQueries, int timeout = 30)
        {
            if (this.transactionWasInterrupted)
                return false;
            bool success = false;
            SqlCommand sCommand = getSqlCommand(sqlQueries[0]);
            try
            {
                sCommand.CommandTimeout = timeout;
                if (!this.isInsideTransaction)
                    dbConnection.Open();
                for (int i = 0; i < sqlQueries.Length; i++)
                {
                    sCommand.CommandText = sqlQueries[i];
                    sCommand.ExecuteNonQuery();
                    success = true;
                }
            }
            catch (Exception e)
            {
                ErrorHandler.handleError(e.Message, "błąd", "kwerenda: " + sqlQueries + "\r\n" + e.StackTrace + "\r\n" + dbConnection.ConnectionString);
                if (this.isInsideTransaction)
                {
                    this.transactionWasInterrupted = true;
                    tryRollbackTransaction();
                }
            }
            finally
            {
                if (!this.isInsideTransaction && dbConnection != null && dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
            return success;
        }


        /// <summary>
        /// timeout dotyczy pojedynczego zapytania z listy zapytań
        /// </summary>
        public void executeQuery(List<string> queries, int timeout = 30)
        {
            foreach (string query in queries)
            {
                if (!String.IsNullOrEmpty(query))
                {
                    executeQuery(query, timeout);
                }
            }            
        }

        /// <summary>
        /// zwraca true jeżeli kwerenda przejdzie bez błędu, false jeżeli kwerenda zwraca błąd; jeżeli metoda wykonywana jest wewnątrz transakcji, błąd kwerendy przerywa i wycofuje transakcję; 
        /// jeżeli metoda wywoływana jest jako kolejna wewnątrz transakcji a któraś z poprzednich wygenerowała błąd, metoda nie będzie wykonana i zwróci false; 
        /// pola DataTable nie muzą być dokładnie w tej samej kolejności co pola w tabeli bazy danych, ale ich nazwy oraz typy danych pól DataTable i tabeli w bazie danych muszą być zgodne (wielkość liter nie ma znaczenia);
        /// </summary>
        public bool writeBulkDataToDB(DataTable data, string tableName, int timeout = 30)
        {
            if (this.transactionWasInterrupted)
                return false;
            // make sure to enable triggers

            SqlBulkCopy bulkCopy = new SqlBulkCopy(dbConnection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

            bulkCopy.DestinationTableName = tableName;
            bulkCopy.BulkCopyTimeout = timeout;
            autoMapColumns(bulkCopy, data);

            if(!this.isInsideTransaction)
                dbConnection.Open();

            try
            {
                bulkCopy.WriteToServer(data);
                return true;
            }
            catch (Exception exc)
            {
                ErrorHandler.handleError(exc);
                if(this.isInsideTransaction)
                {
                    this.transactionWasInterrupted = true;
                    tryRollbackTransaction();
                }
                return false;
            }
            finally
            {
                if (!this.isInsideTransaction && dbConnection != null && dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
        }

        #endregion

        #region obsługa kwerendy z parametrami
        /// <summary>
        /// stosować tylko wtedy, gdy chce się użyć komendy zaincjalizowanej parametrami
        /// przed uruchomieniem należy zainicjować komendę i przypisać jej parametry metodą addCommandParameter, np  dbwriter.addCommandParameter("@parName", SqlDbType.VarBinary, varbinaryValue);
        /// </summary>
        public void initiateParameterizedCommand()
        {
            this.persistentSqlCommand = new SqlCommand(null, dbConnection);
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

        #region metody do obsługi transakcji

        /// <summary>
        /// rozpoczyna transakcję sql; transakcja zamykana jest metodą transactionEnd(); jeżeli kwerenda wykonywana w transakcji wygeneruje błąd, transakcja również jest zamykana
        /// </summary>
        public void transactionStart()
        {
            if (this.persistentSqlCommand == null)
                this.persistentSqlCommand = new SqlCommand(null, this.dbConnection);
            this.dbConnection.Open();
            this.transaction = this.dbConnection.BeginTransaction();
            this.persistentSqlCommand.Transaction = this.transaction;
            isInsideTransaction = true;
        }

        /// <summary>
        /// zwraca true jeżeli kończy transakcję sql, jeżeli jest ona otwarta i wszystkie kwerendy wykonywane w transakcji przeszły bez błędu; 
        /// zwraca false jeżeli kwerenda wykonywana w transakcji wygeneruje błąd, co spowodowało wcześniejsze przerwanie transakcji lub jeżeli transakcji nie udało się zamknąć;
        /// metoda użyta bez transakcji zwraca true
        /// </summary>
        public bool transactionEnd()
        {
            if (this.transactionWasInterrupted)
                return false;
            if (this.isInsideTransaction)
                return tryCommitTransaction();
            return true;
        }

        private bool tryCommitTransaction()
        {
            this.isInsideTransaction = false;
            try
            {
                this.transaction.Commit();
                this.dbConnection.Close();
                return true;
            }
            catch (Exception)
            {
                tryRollbackTransaction();
                return false;
            }
        }

        private bool tryRollbackTransaction()
        {
            this.isInsideTransaction = false;
            try
            {
                this.transaction.Rollback();
                this.dbConnection.Close();
                return true;
            }
            catch (Exception)
            {
                this.dbConnection.Close();
                return false;
            }
        }

        #endregion

        #region pomocnicze metody prywatne
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

        #region mapowanie kolumn dla sqlBulkCopy

        private void autoMapColumns(SqlBulkCopy sbc, DataTable dt)
        {
            List<string> destinationTableHeaders = new DBReader(this.dbConnection).getTableColumnNames(sbc.DestinationTableName);
            foreach (DataColumn column in dt.Columns)
            {
                string sourceColumnName = column.ColumnName;
                int destinationColumnIndex = getDestinationColumnIndex(sourceColumnName, destinationTableHeaders);
                if (destinationColumnIndex == -1)
                    throw new InvalidOperationException("Błąd w mapowaniu kolumny " + column.ColumnName + " źródłowej DataTable z kolumnami tablicy docelowej.");
                sbc.ColumnMappings.Add(column.ColumnName, destinationColumnIndex);
            }
        }

        private int getDestinationColumnIndex(string sourceColumnName, List<string> destinationTableHeaders)
        {
            for (int i = 0; i < destinationTableHeaders.Count; i++)
            {
                if (sourceColumnName.ToLower() == destinationTableHeaders[i].ToLower())
                    return i;
            }
            return -1;
        }

        #endregion
    }
}
