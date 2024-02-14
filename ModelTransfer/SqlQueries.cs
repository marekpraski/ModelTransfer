
namespace ModelTransfer
{
    public class SqlQueries
    {

        //
        //pobieranie danych 
        //


        //katalogi
        //
            
        public static string getDirectories = "select DirectoryId, DirectoryName, ParentDirectoryId from ModelDirectories where Archiwum = 0 and DirectoryId>1 ";

        public static short getDirectories_directoryIdIndex = 0;
        public static short getDirectories_directoryNameIndex = 1;
        public static short getDirectories_parentIdIndex = 2;

        public static string getMaxDirectoryId = "select max(DirectoryId) as maxDirId from ModelDirectories";


        //modele
        //

        public static string getModels = "select IDModel, NazwaModel, OpisModel, DataModel, IDUzytk, CzyArch, DirectoryId, IDUzytkWlasciciel from DefModel2D ";
        public static string getModelsByIdFilter = "where IDModel in(@iDs)";
        public static string getModelsByDirectoryFilter = "where DirectoryId =";


        public static int getModels_idModelIndex = 0;
        public static int getModels_nazwaModelIndex = 1;
        public static int getModels_opisModelIndex = 2;
        public static int getModels_dataModelIndex = 3;
        public static int getModels_idUzytkIndex = 4;
        public static int getModels_czyArchIndex = 5;
        public static int getModels_directoryIdIndex = 6;
        public static int getModels_idUzytkWlascicielIndex = 7;


        public static string getMaxModelId = "select MAX(IDModel) as maxModelId from DefModel2D";

        public static string getSumTriangles = "select  sum(ileTri) as numberOfTriangles from DefPowierzchni where IDModel in(@modelIds )";


        //
        //zapisywanie modeli
        //

        public static string insertModel = "insert into DefModel2D(NazwaModel, OpisModel, DataModel, IDUzytk, CzyArch, DirectoryId, IDUzytkWlasciciel) " + 
                                            "values (@nazwaModel, @opisModel, '@dataModel', @idUzytk, @czyArch, @directoryId, @idWlasciciel);\r\n ";


        public static string getUsers = "select IDUzytk, Uzytkownik from Uzytkownik ";

        public static short getUsers_idUzytkownikIndex = 0;
        public static short getUsers_uzytkownikIndex = 1;


        public static string insertDirectory = "insert into ModelDirectories(DirectoryName, ParentDirectoryId, Archiwum) values ('@directoryName', null, 0) ";

        public static string updateDirectoryParentId = "update ModelDirectories set ParentDirectoryId = @newParentId where DirectoryId = ";


    }
}
