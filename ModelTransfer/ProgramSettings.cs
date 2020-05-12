using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTransfer
{
    class ProgramSettings
    {
        

        //przykładowa linia connection string @"Data Source=laptop08\sqlexpress;Initial Catalog=dbrezerwer_test;User ID=marek;Password=root";
        //ustawienia pliku konfiguracyjnego: nazwa pliku i slowa kluczowe po których wykrywany jest serwer i baza danych
        public static string configFileName = "DBTransfer.xml";
        public static string connectionStringDelimiter = "server";
        public static string databaseNameDelimiter = "db_name";

        public static int connectionTimeout = 10;      //sprawdzanie loginu i hasła nie powinno trwać dłużej niż 10 sekund
        public static int commandTimeout = 600;         //czas czynności na bazie podczas jednego połączenia
        public static int bulkCopyTimeout = 600;        //czas na ładowanie do bazy metodą bulkCopy, ładuje hurtowo dużo danych

        //ścieżka względna z której znajdowany jest plik konfiguracyjny, np @"/../Conf/". Gdy jest "" plik konfiguracyjny musi być w katalogu z którego uruchomiony jest program
        public static string configFilePath = "";


        public static string userName = "marek";
        public static string userPassword = "root";


    }
}
