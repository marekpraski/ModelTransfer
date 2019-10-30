using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ModelTransfer
{
    //służy do tworzenia połaczenia do bazy danych na podstawie elementów składowych, tj. nazwa serwera, nazwa bazy, użytkownik i hasło
    //testuje wygenerowane połaczenie i przekazuje jako obiekt 

    public enum ConnectionTypes { sqlAuthorisation, windowsAuthorisation }
    public enum ConnectionSources { wholeConnectionInFile, serverNameInFile }

   
    public class DBConnector
    {
        private SqlConnection dbConnection;
        private string sqlQuery;

        private string configFilePath;
        private string configFileText;
        private string dbConnectionString;
        private string serverName;
        private string tableName;
        private string dbName;
        private string userName;
        private string userPassword;
        private bool configFileValidated = true;
        private bool configFileValidationWasDone = false;       //jest to bezpiecznik, gdybym w kodzie analizę pliku konfiguracyjnego dał zanim plik został zwalidowany, bo z metody analizującej ściągnąłem wszystkie zabezpieczenia


        public DBConnector (string userName, string userPassword)
        {
            this.userName = userName;
            this.userPassword = userPassword;
        }


        public DBConnector()
        {

        }


        private void generateConnectionString()
        {
            //przykładowy connection string
            //Data Source=laptop08\sqlexpress;Initial Catalog=dbrezerwer_test;User ID=marek;Password=root

            dbConnectionString = "Data Source=" + serverName + ";Initial Catalog=" + dbName + ";User ID=" + userName + ";Password=" + userPassword + ";Connection Timeout=" + ProgramSettings.connectionTimeout;

        }
        private bool testConnection()
        {
            try
            {
                dbConnection.Open();
                dbConnection.Close();
                return true;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                MyMessageBox.display(e.Message, MessageBoxType.Error);
            }
            return false;
        }

        public string getTableNameFromQuery(string sqlQuery)
        {
            this.sqlQuery = sqlQuery;
            TextManipulator tm = new TextManipulator();
            extractTableName(tm);
            return tableName;
        }

        //wyciąga nazwę db z kwerendy wpisanej przez użytkownika
        private void extractTableName(TextManipulator tm)
        {
            //znajduję położenie wyrazu kluczowego "from" w kwerendzie
            List<int> keyWordFromPosition = tm.getSubstringStartPositions(sqlQuery, "from");
            try
            {
                //wywala bład gdy kwerenda jest na tyle bezsensowna, że nie potrafi wyłuskać sensownego wyrazu, który mógłby być nazwą bazy danych
                string textAfterFrom = sqlQuery.Substring(keyWordFromPosition[0] + 5);  //dodaję 5 tj długość wyrazu "from" i jedną spację
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
                MyMessageBox.display("Błąd w kwerendzie", MessageBoxType.Error);
                tableName = "";
            }
        }

        public ref SqlConnection getDBConnection (ConnectionSources source, ConnectionTypes type)
        {
                switch (source)
                {
                    case ConnectionSources.wholeConnectionInFile:
                        readConnStringFromFile(ProgramSettings.connectionStringDelimiter);
                        break;
                    case ConnectionSources.serverNameInFile:
                        getServerNameFromFile(ProgramSettings.connectionStringDelimiter);
                        getDBNameFromFile(ProgramSettings.databaseNameDelimiter);
                        generateConnectionString();
                        break;
                }

            dbConnection = new SqlConnection(dbConnectionString);

            //testConnection();     //nie działa tak jak myślałem, tzn zajmuje długo, nie tak szybko jak testowanie połaczenia przy ustawianiu źródła danych w Windows
            return ref dbConnection;
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

       
        public bool validateConfigFile(string currentPath)
        {

            FileManipulator fm = new FileManipulator();
            configFilePath = currentPath + ProgramSettings.configFilePath;
            string configFile = configFilePath + @"\" + ProgramSettings.configFileName;
            configFileText = fm.readFile(configFile);
            if (!configFileText.Equals(""))                     //plik konfiguracyjny istnieje i nie jest pusty 
            {
                TextManipulator tm = new TextManipulator();
                List<int> indexes = tm.getSubstringStartPositions(configFileText, ProgramSettings.connectionStringDelimiter);

                //jeżeli w pliku jest błąd i jest za dużo lub za mało znaczników
                if (indexes.Count != 2)
                {
                    MyMessageBox.display("błąd pliku konfiguracyjnego " + configFile + " dla znacznika " + ProgramSettings.connectionStringDelimiter, MessageBoxType.Error);
                    configFileValidated= false;
                }
            }
            else
            {
                configFileValidated = false;       //plik jest pusty lub go nie ma
            }
            configFileValidationWasDone = true;
            return configFileValidated;             //domyślnie jest true
        }

        //z pliku tekstowego wyciąga połączenie do serwera na podstawie znacznika "delimiter"
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
                MyMessageBox.display("nie uruchomiono metody validateConfigFile, connectionString jest pusty", MessageBoxType.Error);
            }
        }
    }
}
