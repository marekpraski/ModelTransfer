
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System;
using UtilityTools;

namespace DatabaseInterface
{
    public enum ConnectionDataSource { wholeConnectionStringInFile, serverAndDatabaseNamesInFile, connectionDataAsXml }

    /// <summary>
    /// singleton połączenia sql; nie korzystać, chyba że ma się pewność że został wcześniej zainicjalizowany metodą getDBConnection klasy DBConnector; w przeciwnym wypadku zwróci null
    /// </summary>
    internal sealed class DbConnection
    {
        internal static SqlConnection dbConnection;
    }

    /// <summary>
    /// służy do tworzenia połączenia do bazy danych na podstawie elementów składowych, tj. nazwa serwera, nazwa bazy, użytkownik i hasło; 
    /// testuje wygenerowane połączenie
    /// </summary>
    public class DBConnector
    {
        private string sqlQuery;
        private string configFileText;
        private string dbConnectionString;
        private string userName;
        private string userPassword;
        private bool configFileValidated = true;
        private bool configFileValidationWasDone = false;       //jest to bezpiecznik, gdybym w kodzie analizę pliku konfiguracyjnego dał zanim plik został zwalidowany, bo z metody analizującej ściągnąłem wszystkie zabezpieczenia
        private ErrorHandlingMethod errorHandlingMethod = ErrorHandlingMethod.DisplayMessageBox;

        #region właściwości publiczne
        //przykładowa linia connection string @"Data Source=laptop08\sqlexpress;Initial Catalog=dbrezerwer_test;User ID=marek;Password=root";
        //ustawienia pliku konfiguracyjnego: nazwa pliku i slowa kluczowe po których wykrywany jest serwer i baza danych

        /// <summary>
        /// ścieżka względna z której znajdowany jest plik konfiguracyjny, np @"/../Conf/". Gdy jest "" plik konfiguracyjny musi być w katalogu z którego uruchomiony jest program
        /// </summary>
        public string configFilePath { get; set; } = "";
        /// <summary>
        /// nazwa pliku konfiguracyjnego zawierającego dane połaczenia do bazy danych
        /// </summary>
        public string configFileName { get; set; } = "config.xml";
        /// <summary>
        /// znacznik xml dla pełnego connection stringa w pliku konfiguracyjnym
        /// </summary>
        public string connectionstringMarker { get; set; } = "connectionString";
        /// <summary>
        /// znacznik xml dla serwera w pliku konfiguracyjnym
        /// </summary>
        public string serverNameMarker { get; set; } = "server";
        /// <summary>
        /// znacznik xml dla serwera w pliku konfiguracyjnym
        /// </summary>
        public string databaseNameMarker { get; set; } = "db_name";

        public string serverName { get; private set; }
        public string dbName { get; private set; }

        #endregion

        public DBConnector (string userName, string userPassword)
        {
            this.userName = userName;
            this.userPassword = userPassword;
        }

        /// <summary>
        /// zamiast stosować ten konstruktor lepiej uzyskać SqlConnection metodą getDBConnection przyjmującą parametr DBConnectionData
        /// </summary>
        public DBConnector(DBConnectionData connectionData)
        {
            this.serverName = connectionData.serverName;
            this.dbName = connectionData.dbName;
            this.userName = connectionData.login;
            this.userPassword = connectionData.password;
        }

        /// <summary>
        /// stosując pusty konstruktor uważać, żeby w argumentach metody getConnection podać również użytkownika i hasło, lub pełny connectionString
        /// </summary>
        public DBConnector() { }

        /// <summary>
        /// ustawia sposób w jaki komunikowane są błędy podczas ustanawiania połączenia sql; domyślnie - DisplayErrorMessage
        /// </summary>
        public void setErrorHandlingMethod(ErrorHandlingMethod errorHandlingMethod)
        {
            this.errorHandlingMethod = errorHandlingMethod;
        }

		#region metody publiczne zwracające połączenie sql
		/// <summary>
		/// zwraca zupełnie nowe połączenie sql lub null jeżeli połączenia nie można nawiązać; ta metoda nie zwraca singletona lecz zupełnie nową instancję połączenia;
        /// source to enum określające źródło parametrów konfiguracji połączenia: 
		/// wholeConnectionStringInFile, serverAndDatabaseNamesInFile, connectionDataAsXml
		/// </summary>
		public SqlConnection getDBConnection(ConnectionDataSource source, string connectionDataAsXml, ConnectionTypes type = ConnectionTypes.sqlAuthentication)
        {
            if (DbConnection.dbConnection != null)
                return DbConnection.dbConnection;

            switch (source)
            {
                case ConnectionDataSource.wholeConnectionStringInFile:
                    readConnStringFromFile(this.serverNameMarker);
                    break;
                case ConnectionDataSource.serverAndDatabaseNamesInFile:
                    getServerNameFromFile(this.serverNameMarker);
                    getDBNameFromFile(this.databaseNameMarker);
                    generateConnectionString();
                    break;
                case ConnectionDataSource.connectionDataAsXml:
                    getConnectionStringFromXml(connectionDataAsXml);
                    break;
            }

            DbConnection.dbConnection = new SqlConnection(dbConnectionString);
            if (testConnection(DbConnection.dbConnection))
                return DbConnection.dbConnection;

            return null;
        }
		/// <summary>
		/// zwraca zupełnie nowe połączenie sql lub null jeżeli połączenia nie można nawiązać; ta metoda nie zwraca singletona lecz zupełnie nową instancję połączenia
		/// </summary>
		public SqlConnection getDBConnection(ConnectionDataSource source, ConnectionTypes type = ConnectionTypes.sqlAuthentication)
        {
            return getDBConnection(source, null, type);
        }

		/// <summary>
		/// zwraca zupełnie nowe połączenie sql lub null jeżeli połączenia nie można nawiązać; ta metoda nie zwraca singletona lecz zupełnie nową instancję połączenia
		/// </summary>
		public SqlConnection getDBConnection(DBConnectionData connectionData)
        {
            return createConnection(connectionData.connectionString);
        }


		/// <summary>
		/// zwraca zupełnie nowe połączenie sql lub null jeżeli połączenia nie można nawiązać; ta metoda nie zwraca singletona lecz zupełnie nową instancję połączenia
		/// </summary>
		public SqlConnection getNewDBConnection(DBConnectionData connectionData)
        {
            return createConnection(connectionData.connectionString);
        }

		/// <summary>
		/// zwraca zupełnie nowe połączenie sql lub null jeżeli połączenia nie można nawiązać; ta metoda nie zwraca singletona lecz zupełnie nową instancję połączenia
		/// </summary>
		public SqlConnection getDBConnection(string connectionString)
        {
            if (DbConnection.dbConnection != null)
                return DbConnection.dbConnection;
            return createConnection(connectionString);
        }
        /// <summary>
        /// zwraca utworzony wcześniej którąś ze sparametryzowanych metod getDBConnectio() singleton połączenia sql;
        /// jeżeli instancja połączenia nie została wcześniej utworzona, zwróci null
        /// </summary>
        public static SqlConnection getDBConnection()
        {
            return DbConnection.dbConnection;
        }

        #endregion

        private SqlConnection createConnection(string connectionString)
        {
            SqlConnection newDbConnection = new SqlConnection(connectionString);
            if (testConnection(newDbConnection))
            {
                DbConnection.dbConnection = newDbConnection;
				return newDbConnection;
            }
            return null;
        }

        /// <summary>
        /// wyciąga nazwę tablicy z kwerendy podanej jako parametr
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public string getTableNameFromQuery(string sqlQuery)
        {
            this.sqlQuery = sqlQuery;
            return extractTableName();
        }

        /// <summary>
        /// sprawdza poprawność pliku konfiguracyjnego według kryteriów określonych we właściwościach publicznych tej klasy
        /// </summary>
        public bool validateConfigFile(string currentPath)
        {
            configFilePath = currentPath + this.configFilePath;
            string configFile = configFilePath + @"\" + this.configFileName;
            configFileText = readTextFile(configFile);
            if (!configFileText.Equals(""))                     //plik konfiguracyjny istnieje i nie jest pusty 
            {
                TextManipulator tm = new TextManipulator();
                List<int> indexes = tm.getSubstringStartPositions(configFileText, this.serverNameMarker);

                //jeżeli w pliku jest błąd i jest za dużo lub za mało znaczników
                if (indexes.Count != 2)
                {
                    handleError("błąd pliku konfiguracyjnego " + configFile + " dla znacznika " + this.serverNameMarker);
                    configFileValidated = false;
                }
            }
            else
            {
                configFileValidated = false;       //plik jest pusty lub go nie ma
            }
            configFileValidationWasDone = true;
            return configFileValidated;             //domyślnie jest true
        }


        #region metody prywatne

        private string extractTableName()
        {
            string tableName = "";
            TextManipulator tm = new TextManipulator();
            //znajduję położenie wyrazu kluczowego "from" w kwerendzie
            List<int> keyWordFromPosition = tm.getSubstringStartPositions(sqlQuery.ToLower(), "from");
            try
            {
                //wywala bład gdy kwerenda jest na tyle bezsensowna, że nie potrafi wyłuskać sensownego wyrazu, który mógłby być nazwą bazy danych
                string textAfterFrom = sqlQuery.Substring(keyWordFromPosition[0] + 5);  //dodaję długość wyrazu from i jedną spację
                int firstSpacePosition = textAfterFrom.IndexOf(" ");
                if (firstSpacePosition == -1)   //brak spacji
                {
                    tableName = textAfterFrom;
                }
                else
                {
                    tableName = textAfterFrom.Substring(0, firstSpacePosition);
                }
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                handleError(e);
                tableName = "";
            }
            return tableName.Replace("\n","").Replace("\n","");  //gdyby po nazwie tablicy były znaczniki nowej linii, to je usuwam
        }


        private void generateConnectionString()
        {
            //przykładowy connection string
            //Data Source=laptop08\sqlexpress;Initial Catalog=dbrezerwer_test;User ID=marek;Password=root

            dbConnectionString = "Data Source=" + serverName + ";Initial Catalog=" + dbName + ";User ID=" + userName + ";Password=" + userPassword + ";Connection Timeout=30";
        }
        private bool testConnection(SqlConnection dbConnection)
        {
            if (!tryPingServer(dbConnection.DataSource))
                return false;
            try
            {
                if (String.IsNullOrEmpty(dbConnection.Database))
                {
					handleError("Brak nazwy bazy danych. Sprawdź plik konfiguracyjny");
					return false;
                }
                dbConnection.Open();
                return true;
            }
            catch (Exception e)
            {
                handleError(e);
                return false;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        private bool tryPingServer(string dataSource)
        {
            if (pingServer(dataSource))
                return true;
            string serverName = extractServerNameFromDatasource(dataSource);
            if (serverName == dataSource)   //inaczej drugi raz chce robić ping dla tej samej nazwy serwera
                return false;
            if (pingServer(serverName))
                return true;

            return false;
		}

        private bool pingServer(string serverName)
        {
            try
            {
                System.Net.NetworkInformation.IPStatus s = new System.Net.NetworkInformation.Ping().Send(serverName).Status;
                return s == System.Net.NetworkInformation.IPStatus.Success;				
			}
            catch (Exception)
            {
                return false;
            }
        }

		private string extractServerNameFromDatasource(string dataSource)
		{
			int slashPosition = dataSource.IndexOf('\\');
            string tempName = slashPosition > 0 ? dataSource.Substring(0, slashPosition) : dataSource;
            int commaPosition = tempName.IndexOf(',');
            return commaPosition > 0 ? dataSource.Substring(0, commaPosition) : tempName;
		}

		private void getConnectionStringFromXml(string connectionDataAsXml)
        {
            DBConnectionData connData = new DBConnectionData();
            connData.setConnectionDataFromXml(connectionDataAsXml);
            this.dbConnectionString = connData.connectionString;
        }

        private void getDBNameFromFile(string delimiter)
        {
            if (configFileValidationWasDone)
            {
                dbName = readStringFromFile(delimiter);
            }
            else
            {
                dbName = "";
            }
        }

        private void getServerNameFromFile(string delimiter)
        {
            if (configFileValidationWasDone)
            {
                serverName = readStringFromFile(delimiter);
            }
            else
            {
                serverName = "";
            }
        }

        /// <summary>
        /// z pliku tekstowego wyciąga połączenie do serwera na podstawie znacznika "delimiter"
        /// </summary>
        private string readStringFromFile(string delimiter)
        {
            TextManipulator tm = new TextManipulator();
            List<int> indexes = tm.getSubstringStartPositions(configFileText, delimiter);
            int startIndex = indexes[0] + delimiter.Length + 1;         //kompensuję na > po znaczniku
            int connStringLength = indexes[1] - startIndex - 2;         //kompensuję na </ przed znacznikiem
            return configFileText.Substring(startIndex, connStringLength);
        }

        private void readConnStringFromFile(string delimiter)
        {
            if (configFileValidationWasDone)
            {
                dbConnectionString = readStringFromFile(delimiter);
            }
            else
            {
                dbConnectionString = "";
                handleError("nie uruchomiono metody validateConfigFile, connectionString jest pusty");
            }
        }

        private string readTextFile(string file)
        {
            string fileText = "";
            try
            {
                fileText = File.ReadAllText(file);
                if (fileText.Equals(""))
                {
                    handleError("plik " + file + " jest pusty");
                    return "";
                }
            }
            catch (DirectoryNotFoundException exc)
            {
                handleError(exc);
                return "";
            }
            catch (FileNotFoundException exc)
            {
                handleError(exc);
                return "";
            }
            return fileText;
        }

        #endregion

        #region obsługa błędów
        private void handleError(Exception exc)
        {
            if (this.errorHandlingMethod == ErrorHandlingMethod.DisplayMessageBox)
                ErrorHandler.handleError(exc);
            else
                new TextFileTools().appendTextToFile(exc.Message + exc.StackTrace, "DBConnectorError.log");
        }

        private void handleError(string errorMessage)
        {
            if (this.errorHandlingMethod == ErrorHandlingMethod.DisplayMessageBox)
                ErrorHandler.handleError(errorMessage, "Błąd", "");
            else
                new TextFileTools().appendTextToFile(errorMessage, "DBConnectorError.log");
        } 
        #endregion
    }
}
